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
    public class Generalized_PPCPlan_PPSfgReqdController : Controller
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
        string sFATagMap = "";
        string sInvTagMap = "";
        int iBranch__Id = 0;
        int iSFGWarehouse__Id = 0;
        int iPvtype = 7947;
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
            BL_DB DataAcesslayer = new BL_DB();

            Sys_IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
            clsGeneric.writeLog("IP Address :" + Sys_IPAddress);
            //clsGeneric.createTableCollectSFG_PPCPlan(CompanyId, User, docNo);
            clsGeneric.TableCollect_FGBOMSFG(CompanyId, User, docNo);

            strQry = $@" Delete  from TableCollect_FGBOMSFG where [User]='" + User + "'";
            DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
            strQry = $@" DBCC CHECKIDENT ('[TableCollect_FGBOMSFG]', RESEED,0)";
            DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
           
            PostingStatus = 0;
            try
            {
              
                strQry = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + iPvtype + ")";
                sPAbbr = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));
                strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + iPvtype + ")";
                sPName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));
                strQry = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + ")";
                sBAbbr = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));
                strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + ")";
                sBName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));
                
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
                        clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/'" + sBName + "'/" + docNo);
                        var responsePPCPlan = clientPPCPlan.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + sBName + "/" + docNo);
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
                                    strQry = $@" Delete  from TableCollect_FGBOMSFG where [User]='" + User + "'";
                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                    strQry = $@" DBCC CHECKIDENT ('[TableCollect_FGBOMSFG]', RESEED,0)";
                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                    int tempFor1 = extBody.Count - 1;
                                    for (int i = 0; i <= tempFor1; i++)
                                    {
                                        dFGQty = Convert.ToDouble(extBody[i]["Quantity"]);
                                        iFgId = Convert.ToInt32(extBody[i]["Item__Id"]);
                                        strQry = $@"exec spICS_FGBOMSFG " + dFGQty + "," + Convert.ToInt32(extHeader["Date"]) + "," + Convert.ToInt32(extHeader["SFGStockReqd"]) + "," + Convert.ToInt32(extHeader["SFGWarehouse__Id"]) + "," + iFgId + "," + iFgId + "," + iFgId + ",0,1,'" + extBody[i]["FGPlanNo"] +  "','" + User + "'";

                                        DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);


                                    }

                                    //Header Fields

                                    Hashtable headerPPCPlan = new Hashtable();
                                    HashData objHashRequest_PPSfgReqd = new HashData();
                                    iDate = Convert.ToInt32(extHeader["Date"]);
                                    headerPPCPlan.Add("DocNo", "");
                                    headerPPCPlan.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                    headerPPCPlan.Add("sNarration", extHeader["sNarration"]);
                                    headerPPCPlan.Add("ProdnPlanNo", extHeader["DocNo"]);
                                    headerPPCPlan.Add("ProdnPlanDate", Convert.ToInt32(extHeader["Date"]));
                                    iBranch__Id = Convert.ToInt32(extHeader["Branch__Id"]);
                                    iSFGWarehouse__Id = Convert.ToInt32(extHeader["SFGWarehouse__Id"]);
                                    sFATagMap = extHeader["FATagMap"].ToString();
                                    sInvTagMap = extHeader["InvTagMap"].ToString(); 

                                    //headerPPCPlan.Add("PPCPlanNo", extHeader["DocNo"]);
                                    //headerPPCPlan.Add("PPCPlanDate", Convert.ToInt32(extHeader["Date"]));
                                    //headerPPCPlan.Add("DueDate", extHeader["DueDate"]);

                                    List<System.Collections.Hashtable> lstBody_PPCPlan = new List<System.Collections.Hashtable>();
                                    lstBody_PPCPlan.Clear();
                                    
                                    //strQry = $@" SELECT TOP (100) PERCENT id, iVariantId, iProductId, planQty,BOMQty,SFGReqQty, iInvTagValue,BranchId,WCId,FGId,ParentProductId, un.iDefaultBaseUnit FROM dbo.TableCollectSFG_PPCPlan AS tm INNER JOIN muCore_Product_Units un ON  tm.iProductId=un.iMasterId" +
                                    //  " WHERE (loggeduser = '" + User + "') ORDER BY FGId,iLevel,id DESC";
                                    //ORDER BY FGId, id DESC, iLevel";
                                    strQry =$@"Select id,VariantId,SfgId,PlanQty,BOMQuantity,Q2P,FGId,ParentId,[User],iFATag,iInvtag,FGPlanNO  from TableCollect_FGBOMSFG order by level";

                                    DataSet ds2 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j <= ds2.Tables[0].Rows.Count - 1; j++)
                                        {
                                            Hashtable bodyPPCPlan = new Hashtable();

                                            //bodyPPCPlan.Add(sFATagMap + "__Id", iBranch__Id);
                                            //bodyPPCPlan.Add(sInvTagMap + "__Id", iSFGWarehouse__Id);

                                            bodyPPCPlan.Add(sFATagMap + "__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][9]));
                                            bodyPPCPlan.Add(sInvTagMap + "__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][10]));
                                            bodyPPCPlan.Add("FATag__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][9]));
                                            bodyPPCPlan.Add("InvTag__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][10]));
                                            
                                            bodyPPCPlan.Add("FGPlanNo", Convert.ToString(ds2.Tables[0].Rows[j][11]));

                                            bodyPPCPlan.Add("Item__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][2]));
                                            //bodyPPCPlan.Add("Unit__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][11]));
                                            Hashtable bodyPlanQtyPPCPlan = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][3])},
                                                {"FieldName","Plan Qty"},
                                                {"FieldId",1053},
                                                {"ColMap", 7},
                                                {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][3])}
                                            };
                                            bodyPPCPlan.Add("Plan Qty", bodyPlanQtyPPCPlan);
                                            Hashtable bodyBOMQtyPPCPlan = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(ds2.Tables[0].Rows[j][4])},
                                                {"FieldName","BOM Qty"},
                                                {"FieldId",1054},
                                                {"ColMap", 8},
                                                {"Value", Convert.ToDecimal(ds2.Tables[0].Rows[j][4])}
                                            };
                                            bodyPPCPlan.Add("BOM Qty", bodyBOMQtyPPCPlan);
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
                                            bodyPPCPlan.Add("SFG Reqd Qty", bodySFGReqQtyPPCPlan);
                                            bodyPPCPlan.Add("Quantity", Convert.ToDecimal(ds2.Tables[0].Rows[j][3]) * Convert.ToDecimal(ds2.Tables[0].Rows[j][4]));
                                            bodyPPCPlan.Add("FGCode__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][6]));
                                            // 29-03-2022 added by Azhar Requested By Majid Sir 
                                            bodyPPCPlan.Add("ParentFGCode__Id", Convert.ToInt32(ds2.Tables[0].Rows[j][7]));
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