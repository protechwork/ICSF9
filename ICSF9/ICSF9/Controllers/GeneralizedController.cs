using ICSF9.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
// Kale Group - 070 - 0C0 - 

namespace ICSF9.Controllers
{

    public class GeneralizedController : Controller
    {
        #region Variable Definition
        int HeaderId = 0;
        int HeaderBatchId = 0;
        int Docdate = 0;
        Decimal RMQty = 0;
        //Decimal RMRate = 0;
        Decimal RMGross = 0;
        Decimal AvgRMRate = 0;
        int WSId = 0;
        int BranchId = 0;
        int iBookNo = 0;
        int iCode = 0;
        int DeptId = 0;
        int BlankId = 0;
        //int BlankBatchId = 0;
        int BlankWSId = 0;
        decimal BlankQty = 0;
        decimal BlankRate = 0;
        int TransactionId = 0;
        int WCId = 0;
        int RMReusableId = 0;
        //int RMRBatchId = 0;
        decimal RMRQty = 0;
        int EndPieceId = 0;
        //int EndPieceBatchId = 0;
        decimal EndPieceQty = 0;
        //decimal EndPieceRate = 0;
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        int vStatus = 0;
        string vAbbr = "";
        string vAbbrPosting = "";
        string vSnamePosting = "";
        #endregion
        

        public ActionResult GenerilizedAfterSave(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            BL_DB focus_db = new BL_DB();
            

            string strQuery;
            string _source;
            string _destiny;
            string _parent_id = "";
            DataSet ds;
            string error_msg = "";

            Hashtable headerCBROD = new Hashtable();


            string create_if_not_exist = ""
              + "IF OBJECT_ID(N'dbo.ICS_ProductMap', N'U') IS NULL "
              + "begin"
              + "    CREATE TABLE[dbo].[ICS_ProductMap]( "
              + "		[id] "
              + "        [int] IDENTITY(1,1) NOT NULL, "
              + "        [source] [nvarchar](max) NULL, "
              + "		 [destination] [nvarchar](max) NULL "
              + "	) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY] "
              + " "
              + "    INSERT INTO ICS_ProductMap(source, destination) VALUES('','sName') "
              + "    INSERT INTO ICS_ProductMap(source, destination) VALUES('','sCode') "
              + "    INSERT INTO ICS_ProductMap(source, destination) VALUES('','iDefaultBaseUnit__Id') "
              + "    INSERT INTO ICS_ProductMap(source, destination) VALUES('','ParentId') "
              + "end";


            int ret = focus_db.GetExecute(create_if_not_exist, CompanyId, ref strErrorMessage);

            if(ret > 0)
            {
                clsGeneric.writeLog("ICS_ProductMap Table Not Found Table Is Created...");
                clsGeneric.writeLog("ICS_ProductMap Table Insert " + create_if_not_exist);
                return Json(new { status = false, data = new { message = "Table Not Created Table is created Please Enter Mapping." } });
            }






            strQuery = "select * from ICS_ProductMap where destination IN ('sName', 'sCode', 'iDefaultBaseUnit__Id', 'ParentId')";
            ds = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

            if (ds.Tables[0].Rows.Count < 4)
            {
                error_msg = "Mandatory Fields Not Match!";
                return Json(new { status = false, data = new { message = error_msg } });
            }




            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("fSessionId", SessionId);
                webClient.Headers.Add("Content-Type", "application/json");
                clsGeneric.writeLog("Download Generalized URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                var resDoc = webClient.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                clsGeneric.writeLog("response Generalized: " + resDoc);
                if (resDoc != null)
                {
                    var resDataDoc = JsonConvert.DeserializeObject<APIResponse.PostResponse>(resDoc);
                    if (resDataDoc.result == -1)
                    {
                        return Json(new { status = false, data = new { message = resDataDoc.message } });
                    }
                    else
                    {
                        if (resDataDoc.data.Count != 0)
                        {
                            var extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(resDataDoc.data[0]["Header"]));
                            if (resDataDoc.data[0]["Footer"].ToString() != "[]")
                            {
                                var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(resDataDoc.data[0]["Footer"]));
                            }
                            var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(resDataDoc.data[0]["Body"]));

                            bool iApproved = Convert.ToBoolean(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extHeader["Flags"]))["Approved"]);
                            if (!iApproved)
                            {
                                return Json(new { status = true, data = new { message = "Source Document is not Authorized" } });
                            }
                        }
                    }
                }
            }




            strQuery = "SELECT * FROM ICS_ProductMap";
            ds = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);


            if (ds != null)
            {
                for (int ir = 0; ir < ds.Tables[0].Rows.Count; ir++)
                {
                    _source = Convert.ToString(ds.Tables[0].Rows[ir]["source"]);
                    _destiny = Convert.ToString(ds.Tables[0].Rows[ir]["destination"]);



                    //strQuery = "SELECT * FROM tCore_HeaderData" + vtype + "_0";
                    strQuery = " SELECT CHD.* FROM  tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN  " +
                               " dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE(CH.iVoucherType = " + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";


                    DataSet ds2 = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

                    if (_destiny.Equals("ParentId"))
                    {
                        _parent_id = Convert.ToString(ds2.Tables[0].Rows[0][_source]);
                    }

                    if (ds2 != null)
                    {
                        //int iProduct = Convert.ToInt32(ds2.Tables[0].Rows[0][_source]);
                        string valInSource = Convert.ToString(ds2.Tables[0].Rows[0][_source]);
                        headerCBROD.Add(_destiny, valInSource);
                    }
                }
            }

            if (!_parent_id.Equals(""))
            {
                string master_type = "";
                string master_valuation_method = "";

                strQuery = "select * from vCore_Product where iMasterId=" + _parent_id;


                DataSet ds2 = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

                if (ds2 != null)
                {
                    for (int ir = 0; ir < ds2.Tables[0].Rows.Count; ir++)
                    {
                        master_type = Convert.ToString(ds2.Tables[0].Rows[ir]["iProductType"]);
                        master_valuation_method = Convert.ToString(ds2.Tables[0].Rows[ir]["iValuationMethod"]);
                    }
                }

                headerCBROD.Add("iProductType", master_type);
                headerCBROD.Add("iValuationMethod", master_valuation_method);
            }



            string strValue = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
            vStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strValue));
            //0,Pending,1,Updated
            if (vStatus == 0)
            {
                HashData objHashRequest = new HashData();

                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(headerCBROD);
                objHashRequest.data = lstHash;
                string sContentCustVendCr = JsonConvert.SerializeObject(objHashRequest);
                clsGeneric.writeLog("Upload URL :- " + "http://localhost/Focus8API/Masters/Core__Product");
                clsGeneric.writeLog("Content to upload Product Master :- " + sContentCustVendCr);
                using (var clt = new WebClient())
                {
                    clt.Headers.Add("fSessionId", SessionId);
                    clt.Headers.Add("Content-Type", "application/json");
                    var strResponse = clt.UploadString("http://localhost/Focus8API/Masters/Core__Product", sContentCustVendCr);
                    clsGeneric.writeLog("Response Product Master :- " + strResponse);
                    if (strResponse != null)
                    {
                        var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                        if (DataResponse.result == -1)
                        {
                            UpdateStatus(vtype, docNo, 0, CompanyId);

                            return Json(new { status = false, data = new { message = DataResponse.message } });
                        }
                        else
                        {
                            var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(DataResponse.data[0]["MasterId"]));
                            //UpdateStatus(vtype, docNo, 1, Convert.ToInt32(iMasterId), sCode, CompanyId);
                            UpdateStatus(vtype, docNo, 1, CompanyId);
                            return Json(new { status = true, data = new { message = "Posting Successful" } });
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

        public ActionResult Generilized_Account_AfterSave(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            BL_DB focus_db = new BL_DB();
            string strQuery;
            string _source;
            string _destiny;
            string _parent_id = "";
            DataSet ds;
            string error_msg = "";
            Hashtable headerCBROD = new Hashtable();

            string create_if_not_exist = ""
                + "IF OBJECT_ID(N'dbo.ICS_AccountMap', N'U') IS NULL "
                + "begin"
                + "    CREATE TABLE [dbo].[ICS_AccountMap]( "
                + "		[id] [int] IDENTITY(1,1) NOT NULL, "
                + "     [source] [nvarchar](max) NULL, "
                + "		 [destination] [nvarchar](max) NULL "
                + "	) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY] "
                + " "
                + "    INSERT INTO ICS_AccountMap(source, destination) VALUES('','sName') "
                + "    INSERT INTO ICS_AccountMap(source, destination) VALUES('','sCode') "
                + "    INSERT INTO ICS_AccountMap(source, destination) VALUES('','ParentId') "
                + "end";


            int ret = focus_db.GetExecute(create_if_not_exist, CompanyId, ref strErrorMessage);

            if (ret > 0)
            {
                clsGeneric.writeLog("ICS_AccountMap Table Not Found Table Is Created...");
                return Json(new { status = false, data = new { message = "Table Not Created Table is created Please Enter Mapping." } });
            }



            strQuery = "select * from ICS_AccountMap where destination IN ('sName', 'sCode', 'ParentId')";
            ds = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

            if (ds.Tables[0].Rows.Count < 3)
            {
                error_msg = "Mandatory Fields Not Match!";
                return Json(new { status = false, data = new { message = error_msg } });
            }

            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("fSessionId", SessionId);
                webClient.Headers.Add("Content-Type", "application/json");
                clsGeneric.writeLog("Download Generalized URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                var resDoc = webClient.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                clsGeneric.writeLog("response Generalized: " + resDoc);
                if (resDoc != null)
                {
                    var resDataDoc = JsonConvert.DeserializeObject<APIResponse.PostResponse>(resDoc);
                    if (resDataDoc.result == -1)
                    {
                        return Json(new { status = false, data = new { message = resDataDoc.message } });
                    }
                    else
                    {
                        if (resDataDoc.data.Count != 0)
                        {
                            var extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(resDataDoc.data[0]["Header"]));
                            if (resDataDoc.data[0]["Footer"].ToString() != "[]")
                            {
                                var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(resDataDoc.data[0]["Footer"]));
                            }
                            var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(resDataDoc.data[0]["Body"]));

                            bool iApproved = Convert.ToBoolean(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extHeader["Flags"]))["Approved"]);
                            if (!iApproved)
                            {
                                return Json(new { status = true, data = new { message = "Source Document is not Authorized" } });
                            }
                        }
                    }
                }
            }




            strQuery = "SELECT * FROM ICS_AccountMap";
            ds = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);


            if (ds != null)
            {
                for (int ir = 0; ir < ds.Tables[0].Rows.Count; ir++)
                {
                    _source = Convert.ToString(ds.Tables[0].Rows[ir]["source"]);
                    _destiny = Convert.ToString(ds.Tables[0].Rows[ir]["destination"]);


                    strQuery = " SELECT CHD.* FROM  tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN  " +
                               " dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE(CH.iVoucherType = " + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";


                    DataSet ds2 = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

                    if (_destiny.Equals("ParentId"))
                    {
                        _parent_id = Convert.ToString(ds2.Tables[0].Rows[0][_source]);
                    }

                    if (ds2 != null)
                    {
                        string valInSource = Convert.ToString(ds2.Tables[0].Rows[0][_source]);
                        headerCBROD.Add(_destiny, valInSource);
                    }
                }
            }

            if (!_parent_id.Equals(""))
            {
                string master_type = "";
                strQuery = "select * from vCore_Account where iMasterId=" + _parent_id;

                DataSet ds2 = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

                if (ds2 != null)
                {
                    for (int ir = 0; ir < ds2.Tables[0].Rows.Count; ir++)
                    {
                        master_type = Convert.ToString(ds2.Tables[0].Rows[ir]["iAccountType"]);
                    }
                }
                headerCBROD.Add("iAccountType", master_type);
            }



            string strValue = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
            vStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strValue));
            //0,Pending,1,Updated
            if (vStatus == 0)
            {
                HashData objHashRequest = new HashData();

                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(headerCBROD);
                objHashRequest.data = lstHash;
                string sContentCustVendCr = JsonConvert.SerializeObject(objHashRequest);
                clsGeneric.writeLog("Upload URL :- " + "http://localhost/Focus8API/Masters/Core__Account");
                clsGeneric.writeLog("Content to upload Account Master :- " + sContentCustVendCr);
                using (var clt = new WebClient())
                {
                    clt.Headers.Add("fSessionId", SessionId);
                    clt.Headers.Add("Content-Type", "application/json");
                    var strResponse = clt.UploadString("http://localhost/Focus8API/Masters/Core__Account", sContentCustVendCr);
                    clsGeneric.writeLog("Response Account Master :- " + strResponse);
                    if (strResponse != null)
                    {
                        var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                        if (DataResponse.result == -1)
                        {
                            UpdateStatus(vtype, docNo, 0, CompanyId);
                            return Json(new { status = false, data = new { message = DataResponse.message } });
                        }
                        else
                        {
                            var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(DataResponse.data[0]["MasterId"]));
                            //UpdateStatus(vtype, docNo, 1, Convert.ToInt32(iMasterId), sCode, CompanyId);
                            UpdateStatus(vtype, docNo, 1, CompanyId);
                            return Json(new { status = true, data = new { message = "Posting Successful" } });
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


        public ActionResult UpdateStock_bak(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {

            BL_DB DataAcesslayer = new BL_DB();

            clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
            string strValue = "";
            strValue = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + " )";
            vAbbr = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strValue));
            vAbbrPosting = "CBROD";
            strValue = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (sAbbr ='" + vAbbrPosting + "' )";
            vSnamePosting = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strValue));
            strValue = "";


            strValue = $@"SELECT        tci.iProduct, tci.fQuantity, tci.mRate, CIB.mVal1, s.sRemarks
                FROM            dbo.tCore_Data_0 AS tcd INNER JOIN
                     
                    dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN
                                         dbo.tCore_IndtaBodyScreenData_0 AS CIB ON tcd.iBodyId = CIB.iBodyId INNER JOIN
                                         dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId /*AND CIB.mVal1 < tci.fQuantity*/
                inner join dbo.tCore_Data5383_0 s ON s.iBodyId=tcd.iBodyId
                WHERE        (tch.sVoucherNo = '3122-2310200475') AND (tch.iVoucherType = 5383)";
            vStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strValue));



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
                    BranchId = 0;
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
                                BlankRate = 0;
                                if (responseDataCENT.data[0]["Footer"].ToString() != "[]")
                                {
                                    var extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Footer"]));
                                    BlankRate = Convert.ToDecimal(extFooter[7]["Input"]);
                                }
                                var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Body"]));

                                HeaderId = Convert.ToInt32(extHeader["HeaderId"]);
                                HeaderBatchId = Convert.ToInt32(extHeader["Batch"]);
                                Docdate = Convert.ToInt32(extHeader["Date"]);
                                BranchId = Convert.ToInt32(extHeader["Branch__Id"]);
                                DeptId = Convert.ToInt32(extHeader["Dept__Id"]);
                                WCId = Convert.ToInt32(extHeader["Works Center__Id"]);
                                BlankId = Convert.ToInt32(extHeader["BlankName__Id"]);
                                BlankQty = Convert.ToDecimal(extHeader["BlankQtyNos"]);
                                BlankWSId = Convert.ToInt32(extHeader["BlankWH__Id"]);
                                RMReusableId = Convert.ToInt32(extHeader["RMReusable__Id"]);
                                EndPieceId = Convert.ToInt32(extHeader["EndPiece__Id"]);
                                EndPieceQty = Convert.ToDecimal(extHeader["EndPieceQtyKgs"]);
                                WSId = Convert.ToInt32(extBody[0]["Warehouse__Id"]);
                                RMRQty = Convert.ToDecimal(extHeader["RMRQtyKgs"]);

                                strValue = $@"SELECT CD.iCode, CD.iBookNo FROM tCore_Data_0 AS CD  " +
                                    " Where (CD.iHeaderId =" + HeaderId + ")";
                                DataSet dt0 = DataAcesslayer.GetData(strValue, CompanyId, ref strErrorMessage);
                                if (dt0 != null && dt0.Tables.Count > 0 && dt0.Tables[0].Rows.Count > 0)
                                {
                                    bool b1 = string.IsNullOrEmpty(dt0.Tables[0].Rows[0][0].ToString());
                                    if (!b1)
                                        iCode = Convert.ToInt32(dt0.Tables[0].Rows[0][0].ToString());
                                    bool b2 = string.IsNullOrEmpty(dt0.Tables[0].Rows[0][1].ToString());
                                    if (!b2)
                                        iBookNo = Convert.ToInt32(dt0.Tables[0].Rows[0][1].ToString());

                                }

                                //BlankBatchId = 355;
                                //RMRBatchId = 354;
                                //EndPieceBatchId = 355;

                                RMQty = 0;
                                //RMRate = 0;
                                AvgRMRate = 0;
                                RMGross = 0;
                                int tempFor1 = extBody.Count - 1;
                                for (int i = 0; i <= tempFor1; i++)
                                {
                                    //if (i == 0)
                                    //    RMRBatchId = Convert.ToInt32(extBody[i]["Batch"]);
                                    TransactionId = Convert.ToInt32(extBody[i]["TransactionId"]);
                                    RMQty = RMQty + Convert.ToDecimal(extBody[i]["Quantity"]);
                                    RMGross = RMGross + Convert.ToDecimal(extBody[i]["Gross"]);

                                }

                                AvgRMRate = RMGross / RMQty;
                                Hashtable headerCBROD = new Hashtable();
                                headerCBROD.Add("DocNo", docNo);
                                headerCBROD.Add("Date", Docdate);
                                headerCBROD.Add("Branch__Id", BranchId);
                                headerCBROD.Add("Dept__Id", DeptId);
                                headerCBROD.Add("Works Center__Id", WCId); //4
                                headerCBROD.Add("sNarration", "");
                                headerCBROD.Add("PartyAC__Id", iBookNo);
                                headerCBROD.Add("PurchaseAC__Id", iCode);
                                List<System.Collections.Hashtable> lstBody = new List<System.Collections.Hashtable>();
                                Hashtable bodyCBROD = new Hashtable();
                                bodyCBROD.Add("Warehouse__Id", BlankWSId);
                                bodyCBROD.Add("Item__Id", Convert.ToInt32(BlankId));
                                bodyCBROD.Add("Quantity", Convert.ToDecimal(BlankQty));
                                bodyCBROD.Add("Rate", Convert.ToDecimal(BlankRate));
                                bodyCBROD.Add("CutEntryNo", docNo);
                                bodyCBROD.Add("CutEntryDt", Docdate);

                                Hashtable bodyBatchCBROD = new Hashtable
                                {
                                    {"BatchNo",  docNo + "/" + "1" },
                                    {"MfgDate", Convert.ToInt32(Docdate)},
                                    {"BatchRate", Convert.ToDecimal(BlankRate)},
                                    {"Qty", Convert.ToDecimal(BlankQty)}


                                };
                                List<System.Collections.Hashtable> lstBatch = new List<System.Collections.Hashtable>();
                                lstBatch.Add(bodyBatchCBROD);
                                bodyCBROD.Add("Batch", bodyBatchCBROD);
                                lstBody.Add(bodyCBROD);
                                Hashtable bodyCBROD1 = new Hashtable();
                                bodyCBROD1.Add("Warehouse__Id", WSId);
                                bodyCBROD1.Add("Item__Id", Convert.ToInt32(EndPieceId));
                                bodyCBROD1.Add("Quantity", Convert.ToDecimal(EndPieceQty));
                                bodyCBROD1.Add("Rate", Convert.ToDecimal(AvgRMRate));
                                bodyCBROD1.Add("CutEntryNo", docNo);
                                bodyCBROD1.Add("CutEntryDt", Docdate);

                                Hashtable bodyBatchCBROD1 = new Hashtable
                                {
                                    {"BatchNo", docNo + "/" + "2" },
                                    {"MfgDate", Convert.ToInt32(Docdate)},
                                    {"BatchRate", Convert.ToDecimal(AvgRMRate)},
                                    {"Qty", Convert.ToDecimal(EndPieceQty)}

                                };
                                List<System.Collections.Hashtable> lstBatch1 = new List<System.Collections.Hashtable>();
                                lstBatch1.Add(bodyBatchCBROD1);
                                bodyCBROD1.Add("Batch", bodyBatchCBROD1);
                                lstBody.Add(bodyCBROD1);
                                Hashtable bodyCBROD2 = new Hashtable();
                                bodyCBROD2.Add("Warehouse__Id", WSId);
                                bodyCBROD2.Add("Item__Id", Convert.ToInt32(RMReusableId));
                                bodyCBROD2.Add("Quantity", Convert.ToDecimal(RMRQty));
                                bodyCBROD2.Add("Rate", Convert.ToDecimal(AvgRMRate));
                                bodyCBROD2.Add("CutEntryNo", docNo);
                                bodyCBROD2.Add("CutEntryDt", Docdate);
                                //bodyCBROD2.Add("BDate", Docdate);

                                Hashtable bodyBatchCBROD2 = new Hashtable
                                {
                                    //{"BatchId", Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, "SELECT TOP 1  Btc.iBatchId FROM dbo.tCore_Batch_0 AS Btc INNER JOIN tCore_Indta_0 AS ind ON Btc.iBodyId = ind.iBodyId  WHERE (ind.iProduct = " + RMReusableId +") ORDER BY Btc.iMfDate DESC"))},
                                    {"BatchNo", clsGeneric.ShowRecord(CompanyId, "SELECT TOP (1) B.sBatchNo FROM tCore_Batch_0 AS B INNER JOIN tCore_Data_0 AS CD ON B.iBodyId = CD.iBodyId WHERE      (CD.iBodyId ="  + TransactionId + ") ORDER BY B.iMfDate DESC")}
                                    //{"MfgDate", Convert.ToInt32(Docdate)},
                                    //{"BatchRate", Convert.ToDecimal(AvgRMRate)},
                                    //{"Qty", Convert.ToDecimal(RMRQty)}
                                };
                                List<System.Collections.Hashtable> lstBatch2 = new List<System.Collections.Hashtable>();
                                lstBatch2.Add(bodyBatchCBROD2);
                                bodyCBROD2.Add("Batch", bodyBatchCBROD2);

                                lstBody.Add(bodyCBROD2);
                                System.Collections.Hashtable objHash = new System.Collections.Hashtable();
                                objHash.Add("Body", lstBody);
                                objHash.Add("Header", headerCBROD);

                                List<System.Collections.Hashtable> lstHash = new List<System.Collections.Hashtable>();
                                lstHash.Add(objHash);
                                HashData objHashRequest = new HashData();
                                objHashRequest.data = lstHash;
                                string sContentCBROD = JsonConvert.SerializeObject(objHashRequest);
                                clsGeneric.writeLog("Content CBROD: " + sContentCBROD);
                                clsGeneric.writeLog("Upload URL: " + "http://localhost/Focus8API/Transactions/Vouchers/" + vSnamePosting);
                                using (var clientCBROD = new WebClient())
                                {
                                    clientCBROD.Encoding = Encoding.UTF8;
                                    clientCBROD.Headers.Add("fSessionId", SessionId);
                                    clientCBROD.Headers.Add("Content-Type", "application/json");
                                    var responseCBROD = clientCBROD.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + vSnamePosting, sContentCBROD);
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
                                            var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataCBROD.data[0]["VoucherNo"]));
                                            UpdateBatchs(vtype, Docdate, iMasterId, CompanyId);
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
        public static void UpdateStatus(int Type, string vno, int PostingStatus, int CompanyId)
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
        static void UpdateBatchs(int Type, int idocDate, string vno, int CompanyId)
        {
            BL_DB DataAcesslayer = new BL_DB();
            string strErrorMessage = string.Empty;
            string strValue = "";
            strValue = $@"Update dbo.tCore_Batch_0  set dbo.tCore_Batch_0.iMfDate =" + idocDate +
                " FROM   dbo.tCore_Header_0 AS CH INNER JOIN    dbo.tCore_Data_0 AS CD ON CH.iHeaderId = CD.iHeaderId INNER JOIN " +
                "dbo.tCore_Batch_0 AS B ON CD.iBodyId = B.iBodyId  WHERE LTRIM(RTRIM(ISNULL(B.iMfDate, ''))) <> '' AND  (CH.iVoucherType =2051) AND (CH.sVoucherNo = N'" + vno + "')";
            clsGeneric.writeLog("Query :" + strValue);
            DataAcesslayer.GetExecute(strValue, CompanyId, ref strErrorMessage);
            if (strErrorMessage != "")
            {
                clsGeneric.writeLog("strErrorMessage :" + strErrorMessage);
            }

        }

    }
}