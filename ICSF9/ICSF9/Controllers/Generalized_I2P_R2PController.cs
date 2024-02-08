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
    public class Generalized_I2P_R2PController : Controller
    {
        #region Variable Definition
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        int iProdOrderId = 0;
        int iFgId = 0;
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
        #endregion
        public ActionResult UpdateI2P_R2P(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, int iVariantID, List<PEBody> BodyData)
        {
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

                    Hashtable HRptPro = new Hashtable();
                    clsGeneric.createTableICS_I2P2RFP_HEAD(CompanyId, User, docNo);
                    clsGeneric.createTableICS_I2P2RFP_Fields(CompanyId, User, docNo);
                    clsGeneric.createTable_tblI2PR2P(CompanyId, User, docNo);


                    strQry = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType ='" + vtype + "' )";
                    sBAbbr = clsGeneric.ShowRecord(CompanyId, strQry);
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + " )";
                    sBName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));

                    /// Open API form Document 

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

                                    strQry = $@"select  Abbr_Id,dAbrr,dName,dType  from ICS_I2P2RFP_HEAD where sType= " + vtype + "";
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



                                    //Header Fields
                                    strQry = "";
                                    strQry = $@"select FieldName, PostFieldName from ICS_I2P2RFP_Fields where (Abbr_Id =" + iAbbr_Id + ") AND(PostPosition = 1)";

                                    DataSet ds2 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    Hashtable headerCBROD = new Hashtable();
                                    HashData objHashRequest_RptPro = new HashData();
                                    iDate = Convert.ToInt32(extHeader["Date"]);
                                    headerCBROD.Add("DocNo", "");
                                    headerCBROD.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                    headerCBROD.Add("BatchNo", Convert.ToInt32(extHeader["BatchNo"]));
                                    iOutputWarehouse = Convert.ToInt32(extHeader["OutputWarehouse__Id"]);
                                    headerCBROD.Add("ProdQty", Convert.ToDecimal(extHeader["ProdQty"]));
                                    fProdSize = Convert.ToDouble(extHeader["ProdQty"]);
                                    headerCBROD.Add("sNarration", extHeader["sNarration"]);
                                    headerCBROD.Add("Net", Math.Abs(Convert.ToDecimal(extHeader["Net"])));
                                    fNet = Math.Abs(Convert.ToDouble(extHeader["Net"]));
                                    if (ds2 != null)
                                    {

                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            GetField = "";
                                            PostField = "";
                                            GetField = Convert.ToString(ds2.Tables[0].Rows[k][0]);
                                            PostField = Convert.ToString(ds2.Tables[0].Rows[k][1]);
                                            headerCBROD.Add(PostField, extHeader[GetField]);
                                        }
                                    }

                                    // As per suggestion by Majid and Kha Sahab @ 07-05-2023 
                                    strQry = $@" insert into tblI2PR2P (iVariantId, iBomBodyId, iSize, iProductId, fQty, bInput, bMainOutPut, iRowIndex, iInvTagValue,iRate,I2PPrdnQty,[R4PValue],[R4PGross],[UserName],[LoginId]) " +
                                    " SELECT     TOP (100) PERCENT mmBB.iVariantId, mmBB.iBomBodyId, mmBB.fQty, mmBB.iProductId, mmBB.fQty, mmBB.bInput, mmBB.bMainOutPut, mmBB.iRowIndex, mmBB.iInvTagValue , case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) end *-1," + fProdSize + ",case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1  else " + fNet + " end , abs(case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1 else " + fNet + " end) ,'" + User + "'," + LoginId + " FROM  dbo.mMRP_BOMBody AS mmBB " +
                                    " INNER JOIN  dbo.mMRP_BomVariantHeader AS mmbvh ON mmBB.iVariantId = mmbvh.iVariantId INNER JOIN dbo.mMRP_BomHeader AS mmH ON mmbvh.iBomId = mmH.iBomId " +
                                    " WHERE   ((bInput =0 and bMainOutPut=0) OR  (bInput =0 and bMainOutPut=1)) and (mmBB.iVariantId = " + iVariantID + ") ORDER BY mmBB.iBomBodyId, mmBB.bMainOutPut DESC";
                                    // Reverted the changes after the checking
                                    strQry = "";
                                    strQry = $@" insert into tblI2PR2P (iVariantId, iBomBodyId, iSize, iProductId, fQty, bInput, bMainOutPut, iRowIndex, iInvTagValue,iRate,I2PPrdnQty,[R4PValue],[R4PGross],[UserName],[LoginId]) " +
                                      " SELECT     TOP (100) PERCENT mmBB.iVariantId, mmBB.iBomBodyId, mmH.iSize, mmBB.iProductId, mmBB.fQty, mmBB.bInput, mmBB.bMainOutPut, mmBB.iRowIndex, mmBB.iInvTagValue , case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) end *-1," + fProdSize + ",case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1  else " + fNet + " end , abs(case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1 else " + fNet + " end) ,'" + User + "'," + LoginId + " FROM  dbo.mMRP_BOMBody AS mmBB " +
                                      " INNER JOIN  dbo.mMRP_BomVariantHeader AS mmbvh ON mmBB.iVariantId = mmbvh.iVariantId INNER JOIN dbo.mMRP_BomHeader AS mmH ON mmbvh.iBomId = mmH.iBomId " +
                                      " WHERE   ((bInput =0 and bMainOutPut=0) OR  (bInput =0 and bMainOutPut=1)) and (mmBB.iVariantId = " + iVariantID + ") ORDER BY mmBB.iBomBodyId, mmBB.bMainOutPut DESC";
                                    

                                   


                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);

                                    strQry = $@"Update tblI2PR2P set R4PGross =(Select sum(R4PValue) from tblI2PR2P where [UserName] ='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID + ")" +
                                             " where (bInput =0 and bMainOutPut=1) and [UserName] ='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID;
                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                    //Body Field

                                    List<System.Collections.Hashtable> lstBody_RptPro = new List<System.Collections.Hashtable>();
                                    lstBody_RptPro.Clear();
                                    strQry = $@"Select iBomBodyId, iSize, iProductId,(fqty/isize)  *I2PPrdnQty ,fQty, bInput, bMainOutPut, iRowIndex, iInvTagValue,iRate,I2PPrdnQty,[R4PValue],[R4PGross] from tblI2PR2P where UserName='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID + " Order by iRowIndex ";
                                    DataSet ds22 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                    clsGeneric.writeLog("Getting from Voucher:" + (ds2));
                                    if (ds22 != null)
                                    {

                                        if (ds22.Tables[0].Rows.Count > 0)
                                        {
                                            for (int k1 = 0; k1 < ds22.Tables[0].Rows.Count; k1++)
                                            {
                                                Hashtable bodyRptPro = new Hashtable();
                                                bodyRptPro.Add("Item__Id", Convert.ToInt32(ds22.Tables[0].Rows[k1][2]));

                                                //bodyRptPro.Add("Warehouse__Id", iOutputWarehouse);
                                                iInput = Convert.ToInt32(ds22.Tables[0].Rows[k1][5]);
                                                iOutput = Convert.ToInt32(ds22.Tables[0].Rows[k1][6]);
                                                if (iOutput == 1 && iInput == 0)
                                                {
                                                    bodyRptPro.Add("Warehouse__Id", iOutputWarehouse);
                                                }
                                                else if (iOutput == 0 && iInput == 0)
                                                {
                                                    // As per discussion over phone by Majid Bhai and Kha Sahab concept change 
                                                    // required to consider the warehous of BOM @ 09-03-2023 
                                                    // bodyRptPro.Add("Warehouse__Id", clsGeneric.ShowRecord(CompanyId, "Select ScrapWarehouse  from muCore_Product_Settings where iMasterId=" + Convert.ToInt32(ds22.Tables[0].Rows[k1][2])));
                                                     bodyRptPro.Add("Warehouse__Id", Convert.ToInt32(ds22.Tables[0].Rows[k1][8]));
                                                }

                                                bodyRptPro.Add("Quantity", Convert.ToDouble(ds22.Tables[0].Rows[k1][3]));
                                                //bodyRptPro.Add("Rate", Convert.ToInt32(ds22.Tables[0].Rows[k1][3]));
                                                bodyRptPro.Add("Gross", Convert.ToDouble(ds22.Tables[0].Rows[k1][12]));

                                                Hashtable bodyBatchRptPro = new Hashtable
                                                {
                                                    {"BatchNo",  docNo + "/" + Convert.ToInt32( k1 + 1 ) },
                                                    {"MfgDate", Convert.ToInt32(iDate)},
                                                    {"BatchRate",  Convert.ToDouble(ds22.Tables[0].Rows[k1][3])},
                                                    {"Qty", clsGeneric.DecimalCustomFormat(Convert.ToDouble(ds22.Tables[0].Rows[k1][3]))}
                                                };

                                                Hashtable bodyRejectedRptPro = new Hashtable
                                                {
                                                        {"Input",  0},
                                                        {"FieldId", 2},
                                                        {"ColMap",  0},
                                                        {"Value", 0}
                                                };

                                                bodyRptPro.Add("Rejected", bodyRejectedRptPro);
                                                bodyRptPro.Add("Batch", bodyBatchRptPro);
                                                // might required change
                                                strQry = "";
                                                strQry = $@"select FieldName, PostFieldName from ICS_I2P2RFP_Fields  where (Abbr_Id =" + iAbbr_Id + ") AND(PostPosition = 2)";
                                                DataSet ds3 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                                if (ds3 != null)
                                                {
                                                    for (int m = 0; m < ds3.Tables[0].Rows.Count; m++)
                                                    {
                                                        GetField = "";
                                                        PostField = "";
                                                        GetField = Convert.ToString(ds3.Tables[0].Rows[m][0]);
                                                        PostField = Convert.ToString(ds3.Tables[0].Rows[m][1]);
                                                        bodyRptPro.Add(PostField, extHeader[GetField]);
                                                    }
                                                }
                                                ////Body Screen Fields
                                                strQry = "";
                                                strQry = $@"select fieldname, postfieldname from ICS_I2P2RFP_Fields where (abbr_id =" + iAbbr_Id + ") and(postposition = 3)";
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
                                                        bodyRptPro.Add(PostField, tt);

                                                    }

                                                    lstBody_RptPro.Add(bodyRptPro);


                                                }

                                            }


                                            System.Collections.Hashtable objHash_RptPro = new System.Collections.Hashtable();
                                            objHash_RptPro.Add("Body", lstBody_RptPro);
                                            objHash_RptPro.Add("Header", headerCBROD);
                                            List<System.Collections.Hashtable> lstHash_RptPro = new List<System.Collections.Hashtable>();
                                            lstHash_RptPro.Add(objHash_RptPro);
                                            objHashRequest_RptPro.data = lstHash_RptPro;
                                            string sContent_RptPro = JsonConvert.SerializeObject(objHashRequest_RptPro);
                                            clsGeneric.writeLog("Upload RptPro :" + "http://localhost/Focus8API/Transactions/Vouchers/" + sPName);
                                            clsGeneric.writeLog("URL Param :" + sContent_RptPro);
                                            using (var clientCBROD = new WebClient())
                                            {
                                                clientCBROD.Encoding = Encoding.UTF8;
                                                clientCBROD.Headers.Add("fSessionId", SessionId);
                                                clientCBROD.Headers.Add("Content-Type", "application/json");
                                                var responseCBROD = clientCBROD.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + sPName, sContent_RptPro);
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
                                                            UpdateBatchs(iPvtype, iDate, iMasterId, CompanyId);
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
    }
}