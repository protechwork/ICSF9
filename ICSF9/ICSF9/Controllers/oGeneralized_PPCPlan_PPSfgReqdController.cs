using ICSF9.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ICSF9.Controllers
{
    public class oGeneralized_PPCPlan_PPSfgReqdController : Controller
    {
        #region Variable Definition
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        int iProdOrderId = 0;
        int iFgId = 0;
        double dFGQty = 0;
        string bHPlanMonth = "";
        double dR4PGross = 0;

        double fProdSize = 0;
        double fNet = 0;
        string snarration = "";
        int iDate = 0;
        int iOutputWarehouse = 0;

        int iPvtype = 0;
        string sPAbbr = "";
        string sPName = "";
        string sBAbbr = "";
        string sBName = "";
        string sinputStr = "";
        string soutputStr = "";
        int iAbbr_Id = 0;
        string[] ipvalues;
        string[] opvalues;
        int PostingStatus;
        int iInput = 0;
        int iOutput = 0;
        int isBatchYes = 0;
        string GetField = "";
        string PostField = "";
        string Sys_IPAddress = "";
        #endregion
        public ActionResult UpdatePPCPlan_PPSfgReqd(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            Sys_IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
            clsGeneric.writeLog("IP Address :" + Sys_IPAddress);
            clsGeneric.createTableCollectSFG_PPCPlan(CompanyId, User, docNo);
            clsGeneric.createTableICS_PPCPlan2PPSfgReqd_HEAD(CompanyId, User, docNo);
            clsGeneric.createTableICS_PPCPlan2PPSfgReqd_Fields(CompanyId, User, docNo);
            PostingStatus = 0;
            try
            {


                BL_DB DataAcesslayer = new BL_DB();
                strQry = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                   "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
                PostingStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                // 0,Pending, 1,Updated
                clsGeneric.writeLog("PostingStatus 0,Pending, 1,Updated  : " + PostingStatus);
                if (PostingStatus == 0)
                {
                    /// Open API form Document 
                    using (var clientPPCPlan = new WebClient())
                    {
                        clientPPCPlan.Encoding = Encoding.UTF8;
                        clientPPCPlan.Headers.Add("fSessionId", SessionId);
                        clientPPCPlan.Headers.Add("Content-Type", "application/json");
                        clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        var responsePPCPlan = clientPPCPlan.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        clsGeneric.writeLog("response CENT: " + responsePPCPlan);

                        if (responsePPCPlan != null)
                        {
                            var rspDataPPCPlan = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responsePPCPlan);
                            if (rspDataPPCPlan.result == -1)
                            {
                                return Json(new { status = false, data = new { message = rspDataPPCPlan.message } });
                            }
                            else
                            {
                                if (rspDataPPCPlan.data.Count != 0)
                                {

                                    var extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(rspDataPPCPlan.data[0]["Header"]));

                                    if (rspDataPPCPlan.data[0]["Footer"].ToString() != "[]")
                                    {
                                        var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(rspDataPPCPlan.data[0]["Footer"]));
                                    }
                                    var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(rspDataPPCPlan.data[0]["Body"]));

                                    strQry = $@"select  Abbr_Id,dAbrr,dName,dType  from ICS_PPCPlan2PPSfgReqd_HEAD where sType= " + vtype + "";
                                    DataSet ds1 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    iAbbr_Id = 0;
                                    sPAbbr = "";
                                    sPName = "";

                                    if (ds1.Tables[0].Rows.Count > 0)
                                    {

                                        for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                                        {
                                            iAbbr_Id = Convert.ToInt32(ds1.Tables[0].Rows[j][0]);
                                            sPAbbr = Convert.ToString(ds1.Tables[0].Rows[j][1]);
                                            sPName = Convert.ToString(ds1.Tables[0].Rows[j][2]);
                                            iPvtype = Convert.ToInt32(ds1.Tables[0].Rows[j][3]);
                                            isBatchYes = 1;
                                        }

                                    }
                                    int tempFor1 = extBody.Count - 1;
                                    for (int i = 0; i <= tempFor1; i++)
                                    {
                                        dFGQty = Convert.ToDouble(extBody[i]["Quantity"]);
                                        iFgId = Convert.ToInt32(extBody[i]["Item__Id"]);
                                        bHPlanMonth = "Jan2023";
                                        strQry = $@"exec spICS_CalBomSFG " + dFGQty + "," + iFgId + ",'" + bHPlanMonth + "',1, " + iFgId + ",'" + User + "'";
                                        DataSet ds = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    }

                                    //Header Fields

                                    Hashtable headerPPCPlan = new Hashtable();
                                    HashData objHashRequest_PPSfgReqd = new HashData();
                                    iDate = Convert.ToInt32(extHeader["Date"]);
                                    headerPPCPlan.Add("DocNo", "");
                                    headerPPCPlan.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                    headerPPCPlan.Add("sNarration", extHeader["sNarration"]);
                                    headerPPCPlan.Add("PPCPlanDate", Convert.ToInt32(extHeader["Date"]));
                                    headerPPCPlan.Add("PPCPlanNo", extHeader["DocNo"]);
                                    headerPPCPlan.Add("DueDate", extHeader["DueDate"]);
                                    headerPPCPlan.Add("ProdnPlanNo", extHeader["DocNo"]);
                                    headerPPCPlan.Add("ProdnPlanDate", Convert.ToInt32(extHeader["Date"]));
                                    List<System.Collections.Hashtable> lstBody_PPCPlan = new List<System.Collections.Hashtable>();
                                    lstBody_PPCPlan.Clear();
                                    strQry = "";
                                    strQry = $@"select FieldName, PostFieldName from ICS_PPCPlan2PPSfgReqd_Fields where (Abbr_Id =" + iAbbr_Id + ") AND(PostPosition = 1)";
                                    DataSet ds22 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    if (ds22 != null)
                                    {

                                        for (int k = 0; k < ds22.Tables[0].Rows.Count; k++)
                                        {
                                            GetField = "";
                                            PostField = "";
                                            GetField = Convert.ToString(ds22.Tables[0].Rows[k][0]);
                                            PostField = Convert.ToString(ds22.Tables[0].Rows[k][0]);
                                            headerPPCPlan.Add(PostField, extHeader[GetField]);
                                        }
                                    }

                                    strQry = $@" SELECT TOP (100) PERCENT id, iVariantId, iProductId, planQty,BOMQty,SFGReqQty, iInvTagValue,BranchId,WCId,FGId,ParentProductId, un.iDefaultBaseUnit FROM dbo.TableCollectSFG_PPCPlan AS tm INNER JOIN muCore_Product_Units un ON  tm.iProductId=un.iMasterId" +
                                      " WHERE (loggeduser = '" + User + "') ORDER BY FGId,iLevel,id DESC";
                                    //ORDER BY FGId, id DESC, iLevel";
                                    DataSet ds2 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j <= ds2.Tables[0].Rows.Count - 1; j++)
                                        {
                                            Hashtable bodyPPCPlan = new Hashtable();
                                            bodyPPCPlan.Add("Item__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][2]));

                                            //bodyPPCPlan.Add("Quantity", Convert.ToDecimal(Convert.ToDecimal(ds2.Tables[0].Rows[j][5])));



                                            //bodyPPCPlan.Add("Department__Id", ds2.Tables[0].Rows[j][7]);
                                            //bodyPPCPlan.Add("Department__Id", extHeader["Department__Id"].ToString());
                                            //bodyPPCPlan.Add("Department__Id", Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, "Select department  from muCore_Product_Settings where iMasterId =" + Convert.ToInt32(ds2.Tables[0].Rows[j][2]))));
                                            bodyPPCPlan.Add("Unit Location__Id", Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, "Select department  from muCore_Product_Settings where iMasterId =" + Convert.ToInt32(ds2.Tables[0].Rows[j][2]))));

                                            //bodyPPCPlan.Add("Warehouse__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][6])/*iWIPWH*/);

                                            strQry = "select WIPWH from mucore_department where iMasterId=(Select department  from muCore_Product_Settings where iMasterId = " + Convert.ToInt32(ds2.Tables[0].Rows[j][2]) + ")";

                                            bodyPPCPlan.Add("Warehouse__Id", Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry))); /*as Mukram and Majid bhai changes ware house as per branch in dated 13-02-2023 */

                                            bodyPPCPlan.Add("Unit__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][11]));

                                            bodyPPCPlan.Add("FGCode", Convert.ToInt32(ds2.Tables[0].Rows[j][9]));
                                            Hashtable bodyBOMQtyPPCPlan = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][4])},
                                                {"FieldName","BOM Qty"},
                                                {"FieldId",1054},
                                                {"ColMap", 8},
                                                {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][4])}
                                            };
                                            Hashtable bodySFGReqQtyPPCPlan = new Hashtable
                                            {
                                               // {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][5]) },
                                                {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][4]) * Convert.ToDecimal(ds2.Tables[0].Rows[j][3])},
                                                {"FieldName","SFG Reqd Qty"},
                                                {"FieldId",1058},
                                                {"ColMap", 9},
                                                // {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][5])}
                                                {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][4]) * Convert.ToDecimal(ds2.Tables[0].Rows[j][3])}
                                            };
                                            bodyPPCPlan.Add("Quantity", Convert.ToDecimal(ds2.Tables[0].Rows[j][4]) * Convert.ToDecimal(ds2.Tables[0].Rows[j][3]));
                                            Hashtable bodyPlanQtyPPCPlan = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][3])},
                                                {"FieldName","Plan Qty"},
                                                {"FieldId",1053},
                                                {"ColMap", 7},
                                                {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][3])}
                                            };
                                            bodyPPCPlan.Add("Plan Qty", bodyPlanQtyPPCPlan);
                                            bodyPPCPlan.Add("SFG Reqd Qty", bodySFGReqQtyPPCPlan);
                                            bodyPPCPlan.Add("BOM Qty", bodyBOMQtyPPCPlan);
                                            // 29-03-2022 added by Azhar Requested By Majid Sir 
                                            bodyPPCPlan.Add("ParentFGCode__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][10]));

                                            // might required change
                                            strQry = "";
                                            strQry = $@"select FieldName, PostFieldName from ICS_PPCPlan2PPSfgReqd_Fields  where (Abbr_Id =" + iAbbr_Id + ") AND(PostPosition = 2)";
                                            DataSet ds3 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                            if (ds3 != null)
                                            {
                                                for (int m = 0; m < ds3.Tables[0].Rows.Count; m++)
                                                {
                                                    GetField = "";
                                                    PostField = "";
                                                    GetField = Convert.ToString(ds3.Tables[0].Rows[m][0]);
                                                    PostField = Convert.ToString(ds3.Tables[0].Rows[m][1]);
                                                    bodyPPCPlan.Add(PostField, extHeader[GetField]);
                                                }
                                            }
                                            ////Body Screen Fields
                                            strQry = "";
                                            strQry = $@"select fieldname, postfieldname from ICS_PPCPlan2PPSfgReqd_Fields where (abbr_id =" + iAbbr_Id + ") and(postposition = 3)";
                                            DataSet ds4 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);

                                            if (ds4 != null)
                                            {
                                                for (int n = 0; n < ds4.Tables[0].Rows.Count; n++)
                                                {
                                                    GetField = "";
                                                    PostField = "";
                                                    GetField = Convert.ToString(ds4.Tables[0].Rows[n][0]);
                                                    PostField = Convert.ToString(ds4.Tables[0].Rows[n][1]);
                                                    //bodyCBROD.Add(PostField, extHeader[GetField]);
                                                    // tt = "body" + postfield;
                                                    Hashtable tt = new Hashtable
                                                    {
                                                        {"input",Convert.ToDecimal( extHeader[GetField])},
                                                        {"fieldname", PostField},
                                                        {"colmap", n},
                                                        {"value",Convert.ToDecimal( extHeader[GetField])}

                                                    };
                                                    bodyPPCPlan.Add(PostField, tt);

                                                }
                                            }
                                            lstBody_PPCPlan.Add(bodyPPCPlan);

                                            //break;
                                        }
                                    }

                                    System.Collections.Hashtable objHash = new System.Collections.Hashtable();
                                    objHash.Add("Body", lstBody_PPCPlan);
                                    objHash.Add("Header", headerPPCPlan);
                                    List<System.Collections.Hashtable> lstHash = new List<System.Collections.Hashtable>();
                                    lstHash.Add(objHash);
                                    objHashRequest_PPSfgReqd.data = lstHash;
                                    string sContentPPSfgReqd = JsonConvert.SerializeObject(objHashRequest_PPSfgReqd);
                                    clsGeneric.writeLog("Content of PPSfgReqd :" + sContentPPSfgReqd);
                                    clsGeneric.writeLog("URL of PPSfgReqd :" + "http://localhost/Focus8API/Transactions/Vouchers/" + iPvtype);
                                    using (var clientPPSfgReqd = new WebClient())
                                    {
                                        clientPPSfgReqd.Encoding = Encoding.UTF8;
                                        clientPPSfgReqd.Headers.Add("fSessionId", SessionId);
                                        clientPPSfgReqd.Headers.Add("Content-Type", "application/json");

                                        var responsePPSfgReqd = clientPPSfgReqd.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + iPvtype, sContentPPSfgReqd);
                                        clsGeneric.writeLog("Response form PPSfgReqd :" + (responsePPSfgReqd));
                                        if (responsePPSfgReqd != null)
                                        {
                                            var responseDataPPSfgReqd = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responsePPSfgReqd);
                                            if (responseDataPPSfgReqd.result == -1)
                                            {
                                                clsGeneric.createTableCollectSFG_PPCPlan(CompanyId, User, docNo);
                                                UpdateStatus(vtype, docNo, 0, CompanyId);
                                                return Json(new { status = false, data = new { message = responseDataPPSfgReqd.message } });
                                            }
                                            else
                                            {
                                                clsGeneric.createTableCollectSFG_PPCPlan(CompanyId, User, docNo);
                                                //clsGeneric.createTableCollectSFG_PPCPlanRMReq(CompanyId, User, docNo);
                                                //clsGeneric.createTableRMRequisition_PPCPlanRMReq(CompanyId, User, docNo);
                                                //if (sUpdatePPCRMReq(CompanyId, SessionId, User, LoginId, vtype, docNo, BodyData))
                                                //{
                                                UpdateStatus(vtype, docNo, 1, CompanyId);
                                                //clsGeneric.createTableCollectSFG_PPCPlanRMReq(CompanyId, User, docNo);
                                                //clsGeneric.createTableRMRequisition_PPCPlanRMReq(CompanyId, User, docNo);
                                                return Json(new { status = true, data = new { message = "Posting Successful" } });

                                                //}
                                                //else
                                                //{
                                                //    UpdateStatus(vtype, docNo, 0, CompanyId);
                                                //    clsGeneric.createTableCollectSFG_PPCPlanRMReq(CompanyId, User, docNo);
                                                //    clsGeneric.createTableRMRequisition_PPCPlanRMReq(CompanyId, User, docNo);
                                                //    return Json(new { status = false, data = new { message = "Posting failed" } });
                                                //}
                                            }

                                        }

                                    }

                                }
                            }
                        }

                    }

                }
                else
                {
                    return Json(new { status = true, data = new { message = "Document Already Posted" } });
                }
            }
            catch (Exception ex)
            {
                //clsGeneric.createTableRMReqQty(CompanyId, User, docNo);
                //clsGeneric.writeLog("delete Query :" + strQry);
                clsGeneric.writeLog("Exception occured:" + (ex.Message));
                return Json(new
                {
                    status = false,
                    data = new { message = "Posting Failed " }
                });
                throw;
            }
            return Json(new { status = true, data = new { message = "Posting Successful" } });

        }

        static void UpdateStatus(int Type, string vno, int PostingStatus, int CompanyId)
        {
            BL_DB DataAcesslayer = new BL_DB();
            string strErrorMessage = string.Empty;
            string strValue = "";
            strValue = $@"Update dbo.tCore_HeaderData" + Type + "_0  set dbo.tCore_HeaderData" + Type + "_0.PostingStatus=" + PostingStatus +
                " from  dbo.tCore_HeaderData" + Type + "_0 AS CHD INNER JOIN  dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId " +
                "WHERE      (CH.iVoucherType =" + Type + ") AND (CH.sVoucherNo = N'" + vno + "')";
            clsGeneric.writeLog("Query :" + strValue);
            DataAcesslayer.GetExecute(strValue, CompanyId, ref strErrorMessage);
            if (strErrorMessage != "")
            {
                clsGeneric.writeLog("strErrorMessage :" + strErrorMessage);
            }
        }
    }
}