using ICSF9.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace ICSF9.Controllers
{
    public class GeneralAfterSaveController : Controller
    {
        #region Variable Definition

        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        static Boolean MappingChk = true;
        int vStatus = 0;
        int PostingVtype = 0;
        int AbbrId = 0;
        string PostingAbbr = "";
        string PostingVname = "";
        int iHDate = 0;
        int isBatchYes = 0;
        string GetField = "";
        string PostField = "";
        #endregion
        public ActionResult UpdateDocument(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            try
            {
                clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
                var Sys_IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                string ComputerName = Dns.GetHostName();

                BL_DB DataAcesslayer = new BL_DB();
                
                clsGeneric.createTableICS_AbbrTable(CompanyId, User, docNo);
                clsGeneric.createTableICS_AbbrMapping(CompanyId, User, docNo);
                strQry = "";
                strQry = $@"select  Abbr_Id,dAbrr,dName,dType  from ICS_AbbrTable where sType= " + vtype + "";
                DataSet ds1 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                AbbrId = 0;
                PostingAbbr = "";
                PostingVname = "";
                MappingChk = true;
                //if (ds1 != null)
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    MappingChk = false;
                    for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                    {
                        AbbrId = Convert.ToInt32(ds1.Tables[0].Rows[j][0]);
                        PostingAbbr = Convert.ToString(ds1.Tables[0].Rows[j][1]);
                        PostingVname = Convert.ToString(ds1.Tables[0].Rows[j][2]);
                        PostingVtype = Convert.ToInt32(ds1.Tables[0].Rows[j][3]);
                        isBatchYes = 1;
                    }
                }

                if (MappingChk)
                {
                    return Json(new { status = true, data = new { message = "Mapping Not Found " } });
                }


                clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
                string strValue = "";


                strValue = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                    "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
                vStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strValue));
                //0,Pending,1,Updated
                if (vStatus == 0)
                {
                    using (var clientCENT = new WebClient())
                    {
                        clientCENT.Encoding = Encoding.UTF8;
                        clientCENT.Headers.Add("fSessionId", SessionId);
                        clientCENT.Headers.Add("Content-Type", "application/json");
                        clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        var responseCENT = clientCENT.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        clsGeneric.writeLog("response CENT: " + responseCENT);

                        if (responseCENT != null)
                        {
                            var responseDataCENT = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCENT);
                            if (responseDataCENT.result == -1)
                            {
                                return Json(new { status = false, data = new { message = responseDataCENT.message } });
                            }
                            else
                            {
                                if (responseDataCENT.data.Count != 0)
                                {

                                    var extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Header"]));

                                    if (responseDataCENT.data[0]["Footer"].ToString() != "[]")
                                    {
                                        var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Footer"]));
                                    }
                                    var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Body"]));
                                    //Header Fields
                                    strQry = "";
                                    strQry = $@"select FieldName, PostFieldName from ICS_AbbrMapping  where (Abbr_Id =" + AbbrId + ") AND(PostPosition = 1)";
                                    DataSet ds2 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    Hashtable headerCBROD = new Hashtable();
                                    if (ds2 != null)
                                    {
                                        iHDate = Convert.ToInt32(extHeader["Date"]);

                                        //headerCBROD.Add("DocNo", "");
                                        headerCBROD.Add("DocNo", Convert.ToString(docNo));
                                        headerCBROD.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            GetField = "";
                                            PostField = "";
                                            GetField = Convert.ToString(ds2.Tables[0].Rows[k][0]);
                                            PostField = Convert.ToString(ds2.Tables[0].Rows[k][1]);
                                            headerCBROD.Add(PostField, extHeader[GetField]);
                                        }
                                    }
                                    //Body Field
                                    strQry = "";
                                    strQry = $@"select FieldName, PostFieldName from ICS_AbbrMapping where (Abbr_Id =" + AbbrId + ") AND(PostPosition = 2)";
                                    DataSet ds3 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);

                                    List<System.Collections.Hashtable> lstBody = new List<System.Collections.Hashtable>();
                                    Hashtable bodyCBROD = new Hashtable();
                                    if (ds3 != null)
                                    {
                                        for (int m = 0; m < ds3.Tables[0].Rows.Count; m++)
                                        {
                                            GetField = "";
                                            PostField = "";
                                            GetField = Convert.ToString(ds3.Tables[0].Rows[m][0]);
                                            PostField = Convert.ToString(ds3.Tables[0].Rows[m][1]);
                                            bodyCBROD.Add(PostField, extHeader[GetField]);
                                        }
                                    }
                                    ////Batch Update
                                    if (isBatchYes == 1)
                                    {
                                        Hashtable bodyBatch = new Hashtable
                                    {
                                        {"BatchNo",  docNo + "/"+ 1},
                                        {"MfgDate", Convert.ToInt32(extHeader["Date"])}

                                    };
                                        bodyCBROD.Add("Batch", bodyBatch);
                                    }
                                    ////Body Screen Fields
                                    strQry = "";
                                    strQry = $@"select fieldname, postfieldname from ics_abbrmapping where (abbr_id =" + AbbrId + ") and(postposition = 3)";
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

                                            bodyCBROD.Add(PostField, tt);
                                        }
                                    }


                                    lstBody.Add(bodyCBROD);

                                    System.Collections.Hashtable objHash = new System.Collections.Hashtable();
                                    objHash.Add("Body", lstBody);
                                    objHash.Add("Header", headerCBROD);

                                    List<System.Collections.Hashtable> lstHash = new List<System.Collections.Hashtable>();
                                    lstHash.Add(objHash);
                                    HashData objHashRequest = new HashData();
                                    objHashRequest.data = lstHash;
                                    string sContentCBROD = JsonConvert.SerializeObject(objHashRequest);
                                    clsGeneric.writeLog("Content CBROD: " + sContentCBROD);
                                    clsGeneric.writeLog("Upload URL: " + "http://localhost/Focus8API/Transactions/Vouchers/" + PostingVname);
                                    using (var clientCBROD = new WebClient())
                                    {
                                        clientCBROD.Encoding = Encoding.UTF8;
                                        clientCBROD.Headers.Add("fSessionId", SessionId);
                                        clientCBROD.Headers.Add("Content-Type", "application/json");
                                        var responseCBROD = clientCBROD.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + PostingVname, sContentCBROD);
                                        clsGeneric.writeLog("response CBROD: " + responseCBROD);
                                        if (responseCBROD != null)
                                        {
                                            var responseDataCBROD = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCBROD);
                                            if (responseDataCBROD.result == -1)
                                            {
                                                UpdateStatus(vtype, docNo, 0, CompanyId);
                                                return Json(new { status = false, data = new { message = responseDataCBROD.message } });
                                            }
                                            else
                                            {
                                                if (isBatchYes == 1)
                                                {
                                                    var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataCBROD.data[0]["VoucherNo"]));
                                                    UpdateBatchs(PostingVtype, iHDate, iMasterId, CompanyId);
                                                }
                                                UpdateStatus(vtype, docNo, 1, CompanyId);
                                                return Json(new { status = true, data = new { message = "Posting Successful" } });
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
                    return Json(new { status = false, data = new { message = "Posting already performed" } });
                }

                return Json(new { status = true, data = new { succes = msg.ToString() } });
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                //Get the first stack frame
                StackFrame frame = st.GetFrame(0);

                //Get the file name
                string fileName = frame.GetFileName();

                //Get the method name
                string methodName = frame.GetMethod().Name;

                //Get the line number from the stack frame
                int line = frame.GetFileLineNumber();

                //Get the column number
                int col = frame.GetFileColumnNumber();
                clsGeneric.writeLog("Exception Line Number:" + line);

                clsGeneric.writeLog("Exception occured:" + (ex.Message));
                return Json(new { status = false, data = new { message = ex.Message } });
                throw;
            }
        }
        static void UpdateBatchs(int Type, int idocDate, string vno, int CompanyId)
        {
            BL_DB DataAcesslayer = new BL_DB();
            string strErrorMessage = string.Empty;
            string strValue = "";
            strValue = $@"Update dbo.tCore_Batch_0  set dbo.tCore_Batch_0.iMfDate =" + idocDate +
                " FROM   dbo.tCore_Header_0 AS CH INNER JOIN    dbo.tCore_Data_0 AS CD ON CH.iHeaderId = CD.iHeaderId INNER JOIN " +
                "dbo.tCore_Batch_0 AS B ON CD.iBodyId = B.iBodyId  WHERE LTRIM(RTRIM(ISNULL(B.iMfDate, ''))) <> '' AND  (CH.iVoucherType =" + Type + ") AND (CH.sVoucherNo = N'" + vno + "')";
            DataAcesslayer.GetExecute(strValue, CompanyId, ref strErrorMessage);

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