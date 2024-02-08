using ICSF9.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ICSF9.Controllers
{
    public class Generalized_RelPO_POSController : Controller
    {
        #region Variable Definition
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        string Sys_IPAddress = "";
        int iReleaseProdnOrder = 0;
        public static Int32 iBodyId = 0;
        int FailedProdOrdCnt = 0;
        int t = 0;
        string sPostDoc = "";
        string sSONo = "";
        int iDeptId = 0;
        #endregion
        public ActionResult UpdateRelPO_POS(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            BL_DB DataAcesslayer = new BL_DB();
            try
            {

                Sys_IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
                clsGeneric.writeLog("IP Address :" + Sys_IPAddress);
                strQry = $@"SELECT tchd.ReleaseProdnOrder FROM dbo.tCore_Header_0 AS tch INNER JOIN dbo.tCore_HeaderData" + vtype + "_0 AS tchd ON tch.iHeaderId = tchd.iHeaderId " +
                     " WHERE (tch.sVoucherNo = N'" + docNo + "') AND (tch.iVoucherType = " + vtype + ")";
                iReleaseProdnOrder = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                if (iReleaseProdnOrder != 0)
                {
                    iBodyId = 0;
                    using (var RelProdOrd = new WebClient())
                    {
                        RelProdOrd.Encoding = Encoding.UTF8;
                        RelProdOrd.Headers.Add("fSessionId", SessionId);
                        RelProdOrd.Headers.Add("Content-Type", "application/json");
                        clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        var responseRelProdOrd = RelProdOrd.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                        clsGeneric.writeLog("response CENT: " + responseRelProdOrd);

                        if (responseRelProdOrd != null)
                        {
                            var rspDataRelProdOrd = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseRelProdOrd);
                            if (rspDataRelProdOrd.result == -1)
                            {
                                return Json(new { status = false, data = new { message = rspDataRelProdOrd.message } });
                            }
                            else
                            {
                                if (rspDataRelProdOrd.data.Count != 0)
                                {

                                    var extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(rspDataRelProdOrd.data[0]["Header"]));

                                    if (rspDataRelProdOrd.data[0]["Footer"].ToString() != "[]")
                                    {
                                        var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(rspDataRelProdOrd.data[0]["Footer"]));
                                    }
                                    var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(rspDataRelProdOrd.data[0]["Body"]));
                                    int tempFor1 = extBody.Count - 1;
                                    for (int i = 0; i <= tempFor1; i++)
                                    {
                                        Hashtable headerRelProdOrd = new Hashtable();
                                        HashData objHashRequest = new HashData();
                                        //iDate = Convert.ToInt32(extHeader["Date"]);
                                        sPostDoc = extBody[i]["ProdnOrderNo"].ToString();
                                        sSONo = extBody[i]["FGPlanNo"].ToString();
                                        headerRelProdOrd.Add("productionorderno", sPostDoc);
                                        headerRelProdOrd.Add("Date", clsGeneric.GetIntToDate(Convert.ToInt32(extHeader["Date"])).ToString());
                                        headerRelProdOrd.Add("sNarration", extHeader["sNarration"]);
                                        headerRelProdOrd.Add("PPCPlanDate", Convert.ToInt32(extHeader["Date"]));
                                        headerRelProdOrd.Add("PPCPlanNo", extHeader["DocNo"]);
                                        headerRelProdOrd.Add("DueDate", clsGeneric.GetIntToDate(Convert.ToInt32(extHeader["DueDate"])).ToString());
                                        headerRelProdOrd.Add("ProdnPlanNo", extHeader["DocNo"]);
                                        headerRelProdOrd.Add("ProdnPlanDate", Convert.ToInt32(extHeader["Date"]));

                                        headerRelProdOrd.Add("product__id", Convert.ToInt32(extBody[i]["Item__Id"]));
                                        headerRelProdOrd.Add("quantity", Convert.ToDecimal(extBody[i]["Quantity"]));
                                        headerRelProdOrd.Add("unit__id", Convert.ToInt32(extBody[i]["Unit__Id"]));
                                        headerRelProdOrd.Add("warehouse__id", Convert.ToInt32(extBody[i]["InvTag__Id"]));
                                        iDeptId = Convert.ToInt32(extBody[i]["FATag__Id"]);
                                        headerRelProdOrd.Add("OrderStatus", 2);
                                        headerRelProdOrd.Add("Issue Type", "Single Level");
                                        //headerProd_Ord.Add("remarks", "Remark");
                                        List<Hashtable> lstHash = new List<Hashtable>();
                                        lstHash.Add(headerRelProdOrd);
                                        objHashRequest.data = lstHash;
                                        string sContent = JsonConvert.SerializeObject(objHashRequest);
                                        clsGeneric.writeLog(" Contents :" + sContent);
                                        clsGeneric.writeLog(" URL :" + "http://localhost/Focus8API/MRP/MakeToOrder");
                                        using (var client = new WebClient())
                                        {
                                            client.Headers.Add("fSessionId", SessionId);
                                            client.Headers.Add("Content-Type", "application/json");
                                            string sUrl = "http://localhost/Focus8API/MRP/MakeToOrder";
                                            string strResponse = client.UploadString(sUrl, sContent);
                                            clsGeneric.writeLog(" strResponse :" + strResponse);
                                            var objHashResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                                            if (objHashResponse.result == -1)
                                            {

                                                FailedProdOrdCnt++;
                                            }
                                            else
                                            {
                                                FailedProdOrdCnt = 0;
                                                strErrorMessage = "";
                                                strErrorMessage = "";
                                                strQry = $@"update tMrp_ProdOrder_0  set iTagFilterId=3,sSONO='" + sSONo  + "', iTagFilterValue=" + iDeptId + ",iOrderStatus=2 where sProdOrderNo='" + sPostDoc + "'";
                                                DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);

                                                strQry = $@"update tCore_Data" + vtype + "_0  set ProdOrderStatus=1 where iBodyId=" + iBodyId;
                                                iBodyId = 0;
                                                DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);

                                            }
                                            t++;


                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //clsGeneric.createTableRMReqQty(CompanyId, User, docNo);
                //clsGeneric.writeLog("delete Query :" + strQry);
                clsGeneric.writeLog("Exception occured:" + (ex.Message));
                return Json(new { status = false, data = new { message = "Posting Failed " } });
                throw;
            }
            return Json(new { status = true, data = new { message = "Posting Successful" } });
        }

        public DateTime GetLastDayOfNextMonth(Int32 sDate)
        {
            DateTime dtDate;
            dtDate = clsGeneric.GetIntToDate(sDate);

            DateTime today = dtDate;
            today = today.AddMonths(2);
            DateTime lastDay = today.AddDays(-(today.Day));
            return lastDay;
        }
    }

}