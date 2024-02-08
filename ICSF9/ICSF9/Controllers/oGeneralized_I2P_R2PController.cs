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
    public class oGeneralized_I2P_R2PController : Controller
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
        string sPAbbr = "RptPro";
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

                    strQry = $@"SELECT  iVoucherType  FROM  dbo.cCore_Vouchers_0 WHERE (sAbbr ='" + sPAbbr + "' )";
                    iPvtype = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + iPvtype + " )";
                    sPName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));

                    strQry = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType ='" + vtype + "' )";
                    sBAbbr = clsGeneric.ShowRecord(CompanyId, strQry);
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + " )";
                    sBName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));

                    //strQry = $@"Select * from tblI2PR2P_HOPIOP";
                    //DataSet ds = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                    //if (ds != null)
                    //{
                    //    if (ds.Tables[0].Rows.Count > 0)
                    //    {
                    //        //foreach (DataColumn column in ds.Tables[0].Columns)
                    //        //{

                    //        //    column.ColumnName
                    //        //}

                    //        foreach (DataRow dr in ds.Tables[0].Rows)
                    //        {
                    //            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    //            {
                    //                var strCol = ds.Tables[0].Columns[i].ColumnName;
                    //                var strColvalue =  dr[ds.Tables[0].Columns[i].ColumnName].ToString();

                    //            }
                    //        }
                    //    }
                    //}

                    strQry = $@"Select Abbr_Id from ICS_I2P2RFP_HEAD where sAbrr='" + sBAbbr + "' and dAbrr='" + sPAbbr + "'";
                    iAbbr_Id = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));

                    strQry = $@"Select Id,FieldName,PostPosition,PostFieldName,ColMap from ICS_I2P2RFP_Fields where Abbr_Id=" + iAbbr_Id;
                    DataSet ds = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                    sinputStr = "";
                    soutputStr = "";
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                            {
                                string sIPstr = Convert.ToString(ds.Tables[0].Rows[k][1]);
                                var sOPstr = ds.Tables[0].Rows[k][3];

                                if (sinputStr != "")
                                {
                                    sinputStr = sinputStr + "," + sIPstr;
                                    soutputStr = soutputStr + "," + sOPstr;
                                }
                                else
                                {
                                    soutputStr = "," + sOPstr;
                                    sinputStr = "," + sIPstr;
                                }

                            }
                        }
                    }

                    clsGeneric.createTable_tblI2PR2P(CompanyId, User, docNo);
                    strQry = $@" SELECT tch.iProdOrderId, tch.fProdSize, tcfdN.sNarration,tcfdN.VariantID,ABS(tchn.fNet), tchn.iDate, tcfdN.OutputWarehouse" + sinputStr + " FROM dbo.tCore_Header_0 AS tchn INNER JOIN " +
                           " dbo.tCore_Data_0 AS tcd ON tchn.iHeaderId = tcd.iHeaderId INNER JOIN dbo.tCore_Header_2_0 AS tch ON " +
                         " tchn.iHeaderId = tch.iHeaderId INNER JOIN dbo.tCore_HeaderData" + vtype + "_0 AS tcfdN ON tchn.iHeaderId = tcfdN.iHeaderId " +
                          " WHERE (tchn.iVoucherType = " + vtype + ") AND (tchn.sVoucherNo = N'" + docNo + "') GROUP BY tch.iProdOrderId, tch.fProdSize, tcfdN.sNarration,tcfdN.VariantID,ABS(tchn.fNet), tchn.iDate, tcfdN.OutputWarehouse" + sinputStr;
                    DataSet ds1 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                    soutputStr = soutputStr == "" ? soutputStr : soutputStr = soutputStr.Substring(1);
                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            iProdOrderId = Convert.ToInt32(Convert.ToString(ds1.Tables[0].Rows[0][0]));
                            fProdSize = Convert.ToDouble(Convert.ToString(ds1.Tables[0].Rows[0][1]));
                            snarration = Convert.ToString(ds1.Tables[0].Rows[0][2]);
                            fNet = Convert.ToDouble(Convert.ToString(ds1.Tables[0].Rows[0][4]));
                            iDate = Convert.ToInt32(Convert.ToString(ds1.Tables[0].Rows[0][5]));
                            iOutputWarehouse = Convert.ToInt32(Convert.ToString(ds1.Tables[0].Rows[0][6]));

                            ipvalues = sinputStr.Split(',');
                            var b = ipvalues.Length;
                            opvalues = soutputStr.Split(',');
                            var b1 = opvalues.Length;

                            HRptPro.Clear();
                            if (soutputStr != "")
                            {
                                for (int i = 0; i <= opvalues.Length - 1; i++)
                                {
                                    HRptPro.Add(opvalues[i].ToString(), ds1.Tables[0].Rows[0][7 + i]);
                                }
                            }
                        }
                    }
                    strQry = $@" insert into tblI2PR2P (iVariantId, iBomBodyId, iSize, iProductId, fQty, bInput, bMainOutPut, iRowIndex, iInvTagValue,iRate,I2PPrdnQty,[R4PValue],[R4PGross],[UserName],[LoginId]) " +
                                      " SELECT     TOP (100) PERCENT mmBB.iVariantId, mmBB.iBomBodyId, mmH.iSize, mmBB.iProductId, mmBB.fQty, mmBB.bInput, mmBB.bMainOutPut, mmBB.iRowIndex, mmBB.iInvTagValue , case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) end *-1," + fProdSize + ",case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1  else " + fNet + " end , abs(case when (mmBB.bInput =0 and mmBB.bMainOutPut=0)  then  (" + fProdSize + " *mmBB.fQty/mmH.iSize ) * (Select ScrapRate  from muCore_Product_Settings where iMasterId=mmBB.iProductId) *-1 else " + fNet + " end) ,'" + User + "'," + LoginId + " FROM  dbo.mMRP_BOMBody AS mmBB " +
                                      " INNER JOIN  dbo.mMRP_BomVariantHeader AS mmbvh ON mmBB.iVariantId = mmbvh.iVariantId INNER JOIN dbo.mMRP_BomHeader AS mmH ON mmbvh.iBomId = mmH.iBomId " +
                                      " WHERE   ((bInput =0 and bMainOutPut=0) OR  (bInput =0 and bMainOutPut=1)) and (mmBB.iVariantId = " + iVariantID + ") ORDER BY mmBB.iBomBodyId, mmBB.bMainOutPut DESC";

                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);

                    strQry = $@"Update tblI2PR2P set R4PGross =(Select sum(R4PValue) from tblI2PR2P where [UserName] ='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID + ")" +
                             " where (bInput =0 and bMainOutPut=1) and [UserName] ='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID;
                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);

                    HashData objHashRequest_RptPro = new HashData();
                    Hashtable headerRptPro = new Hashtable();
                    headerRptPro.Add("DocNo", docNo);
                    headerRptPro.Add("Date", iDate);
                    headerRptPro.Add("ProdQty", fProdSize);
                    headerRptPro.Add("BatchNo", iProdOrderId);
                    foreach (DictionaryEntry element in HRptPro)
                    {
                        headerRptPro.Add(element.Key, element.Value);
                    }
                    List<System.Collections.Hashtable> lstBody_RptPro = new List<System.Collections.Hashtable>();
                    lstBody_RptPro.Clear();
                    strQry = $@"Select iBomBodyId, iSize, iProductId,(fqty/isize)  *I2PPrdnQty ,fQty, bInput, bMainOutPut, iRowIndex, iInvTagValue,iRate,I2PPrdnQty,[R4PValue],[R4PGross] from tblI2PR2P where UserName='" + User + "' and LoginId=" + LoginId + " and iVariantId=" + iVariantID + " Order by iRowIndex ";
                    DataSet ds2 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                    clsGeneric.writeLog("Getting from Voucher:" + (ds2));
                    if (ds2 != null)
                    {
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            for (int k1 = 0; k1 < ds2.Tables[0].Rows.Count; k1++)
                            {
                                Hashtable bodyRptPro = new Hashtable();
                                bodyRptPro.Add("Item__Id", Convert.ToInt32(ds2.Tables[0].Rows[k1][2]));

                                //bodyRptPro.Add("Warehouse__Id", iOutputWarehouse);
                                iInput = Convert.ToInt32(ds2.Tables[0].Rows[k1][5]);
                                iOutput = Convert.ToInt32(ds2.Tables[0].Rows[k1][6]);
                                if (iOutput == 1 && iInput == 0)
                                {
                                    bodyRptPro.Add("Warehouse__Id", iOutputWarehouse);
                                }
                                else if (iOutput == 0 && iInput == 0)
                                {
                                    bodyRptPro.Add("Warehouse__Id", clsGeneric.ShowRecord(CompanyId, "Select ScrapWarehouse  from muCore_Product_Settings where iMasterId=" + Convert.ToInt32(ds2.Tables[0].Rows[k1][2])));
                                }

                                bodyRptPro.Add("Quantity", Convert.ToInt32(ds2.Tables[0].Rows[k1][3]));
                                //bodyRptPro.Add("Rate", Convert.ToInt32(ds2.Tables[0].Rows[k1][3]));
                                bodyRptPro.Add("Gross", Convert.ToInt32(ds2.Tables[0].Rows[k1][12]));

                                Hashtable bodyBatchRptPro = new Hashtable
                        {
                            {"BatchNo",  docNo + "/" + Convert.ToInt32( k1 + 1 ) },
                            {"MfgDate", Convert.ToInt32(iDate)},
                            {"BatchRate",  Convert.ToInt32(ds2.Tables[0].Rows[k1][3])},
                            {"Qty", clsGeneric.DecimalCustomFormat(Convert.ToInt32(ds2.Tables[0].Rows[k1][3]))}
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
                                lstBody_RptPro.Add(bodyRptPro);
                            }
                            System.Collections.Hashtable objHash_RptPro = new System.Collections.Hashtable();
                            objHash_RptPro.Add("Body", lstBody_RptPro);
                            objHash_RptPro.Add("Header", headerRptPro);
                            List<System.Collections.Hashtable> lstHash_RptPro = new List<System.Collections.Hashtable>();
                            lstHash_RptPro.Add(objHash_RptPro);
                            objHashRequest_RptPro.data = lstHash_RptPro;
                            string sContent_RptPro = JsonConvert.SerializeObject(objHashRequest_RptPro);
                            clsGeneric.writeLog("Upload RptPro :" + "http://localhost/Focus8API/Transactions/Vouchers/" + sPName);
                            clsGeneric.writeLog("URL Param :" + sContent_RptPro);
                            using (var clientDel_RptPro = new WebClient())
                            {

                                clientDel_RptPro.Encoding = Encoding.UTF8;
                                clientDel_RptPro.Headers.Add("fSessionId", SessionId);
                                clientDel_RptPro.Headers.Add("Content-Type", "application/json");
                                string url = "http://localhost/Focus8API/Transactions/" + sPName + "/" + docNo;
                                clsGeneric.writeLog("url  Delete RptPro : " + url);
                                var responseDel_RptPro = clientDel_RptPro.UploadString(url, "DELETE", "");
                                clsGeneric.writeLog("Response form Delete RptPro :" + (responseDel_RptPro));
                            }
                            clsGeneric.writeLog("Upload URL Of RptPro :" + ("http://localhost/Focus8API/Transactions/Vouchers/" + sPName));
                            using (var clientRptPro = new WebClient())
                            {
                                clientRptPro.Encoding = Encoding.UTF8;
                                clientRptPro.Headers.Add("fSessionId", SessionId);
                                clientRptPro.Headers.Add("Content-Type", "application/json");

                                var responseRptPro = clientRptPro.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + sPName, sContent_RptPro);
                                clsGeneric.writeLog("Response form RptPro :" + (responseRptPro));
                                if (responseRptPro != null)
                                {
                                    var responseDataRptPro = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseRptPro);
                                    if (responseDataRptPro.result == -1)
                                    {
                                        UpdateStatus(vtype, docNo, 0, CompanyId);
                                        return Json(new { status = false, data = new { message = responseDataRptPro.message } });
                                        //return false;
                                    }
                                    else
                                    {
                                        //var a = Math.Abs((bnet - sumofScrapValue) / bProdQty);

                                        var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataRptPro.data[0]["VoucherNo"]));
                                        UpdateStatus(vtype, docNo, 1, CompanyId);



                                        //var iHeaderId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataRptPro.data[0]["HeaderId"]));
                                        //strQry = $@" insert into dbo.tCore_Header_2_0 (fProdSize,iProdOrderId,iLCNo,iPaymentTerms,iRctIss,iProcessId,iHeaderId) " +
                                        //         "values (" + bProdQty + "," + bProdOrderId + ",0,0,0," + bProdOrderId + "," + iHeaderId + ")";
                                        //DataAcesslayer.GetExecute(strQry, miCompanyId, ref strErrorMessage);
                                        ////UpdateStatus(mivtype, msdocNo, 1, miCompanyId);

                                        //strQry = $@" update dbo.tCore_IndtaBodyScreenData_0   set mInput1 =" + a + ",mVal1=" + a + ", mInput2=" + (a * bProdQty) + ", mVal2=" + (a * bProdQty) +
                                        //    " FROM dbo.tCore_Header_0 AS TCH INNER JOIN dbo.tCore_Data_0 AS TCD ON TCH.iHeaderId = TCD.iHeaderId INNER JOIN " + // ,mInput3=TCI.mGross ,mVal3=TCI.mGross
                                        //    " dbo.tCore_Indta_0 AS TCI ON TCD.iBodyId = TCI.iBodyId INNER JOIN dbo.tCore_IndtaBodyScreenData_0 AS TCIBSD ON TCI.iBodyId = TCIBSD.iBodyId " +
                                        //    " WHERE (TCH.iVoucherType =" + vPRptProVtype + ") AND (TCH.sVoucherNo = N'" + iMasterId + "') AND (TCI.iProduct = " + bMainOPItem + ") ";
                                        ////DataAcesslayer.GetExecute(strQry, miCompanyId, ref strErrorMessage);

                                        //strQry = $@" update dbo.tCore_IndtaBodyScreenData_0 set mInput1=p.ScrapRate,mVal1=p.ScrapRate, mInput2=(p.ScrapRate * TCI.fQuantity),mVal2=(p.ScrapRate * TCI.fQuantity) " + // ,mInput3=TCI.mGross, mVal3=TCI.mGross
                                        //    " FROM dbo.tCore_Header_0 AS TCH INNER JOIN  dbo.tCore_Data_0 AS TCD ON TCH.iHeaderId = TCD.iHeaderId INNER JOIN " +
                                        //    "dbo.tCore_Indta_0 AS TCI ON TCD.iBodyId = TCI.iBodyId INNER JOIN dbo.tCore_IndtaBodyScreenData_0 AS TCIBSD ON TCI.iBodyId = TCIBSD.iBodyId INNER JOIN " +
                                        //    "dbo.muCore_Product_Settings AS p ON TCI.iProduct = p.iMasterId WHERE (TCH.iVoucherType = " + vPRptProVtype + ") AND (TCH.sVoucherNo = N'" + iMasterId + "') AND (TCI.iProduct <> " + bMainOPItem + ")";
                                        ////DataAcesslayer.GetExecute(strQry, miCompanyId, ref strErrorMessage);

                                        ////strQry = $@" update dbo.tCore_Indta_0 set mRate = " + a + " FROM dbo.tCore_Header_0 AS TCH INNER JOIN " +
                                        ////   " dbo.tCore_Data_0 AS TCD ON TCH.iHeaderId = TCD.iHeaderId INNER JOIN dbo.tCore_Indta_0 AS TCI ON TCD.iBodyId = TCI.iBodyId " +
                                        ////  " WHERE(TCH.iVoucherType = " + vPRptProVtype + ") AND(TCH.sVoucherNo = N'" + iMasterId + "') AND (TCI.iProduct = " + bMainOPItem + ")";
                                        //strQry = $@" update dbo.tCore_IndtaBodyScreenData_0 set mInput1=" + a + ",mVal1=" + a + ", mInput2=" + (a * bProdQty) + ",mVal2=" + (a * bProdQty) +  //,mInput3=TCI.mGross, mVal3=TCI.mGross
                                        //    " FROM dbo.tCore_Header_0 AS TCH INNER JOIN  dbo.tCore_Data_0 AS TCD ON TCH.iHeaderId = TCD.iHeaderId INNER JOIN " +
                                        //    "dbo.tCore_Indta_0 AS TCI ON TCD.iBodyId = TCI.iBodyId INNER JOIN dbo.tCore_IndtaBodyScreenData_0 AS TCIBSD ON TCI.iBodyId = TCIBSD.iBodyId INNER JOIN " +
                                        //    "dbo.muCore_Product_Settings AS p ON TCI.iProduct = p.iMasterId WHERE (TCH.iVoucherType = " + vPRptProVtype + ") AND (TCH.sVoucherNo = N'" + iMasterId + "') AND (TCI.iProduct = " + bMainOPItem + ")";
                                        // DataAcesslayer.GetExecute(strQry, miCompanyId, ref strErrorMessage);
                                        return Json(new { status = true, data = new { message = "Posting Successful" } });



                                        //if (blnPendingIssProQC(miCompanyId, msSessionId, msUser, msLoginId, mivtype, msdocNo, vPRptProVtype, Convert.ToString(iMasterId), Convert.ToInt32(iHeaderId), mlBodyData))
                                        //{
                                        //    msg.Clear();
                                        //    msg.Append("Posting Successful");
                                        //    return true;
                                        //}


                                    }
                                } // end  if (responseRptPro != null)

                            } // end using (var clientRptPro = new WebClient())

                        } // if (ds2.Tables[0].Rows.Count > 0)
                    } //end if (ds != null)
                    return Json(new { status = true, data = new { succes = docNo.ToString() + "/" + iVariantID.ToString() } });
                }
                else
                {
                    return Json(new { status = false, data = new { message = "Posting already performed" } });
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

        } // public ActionResult UpdateI2P_R2P
        static void UpdateStatus(int iType, string vno, int iPostingStatus, int iCompanyId)
        {
            BL_DB DataAcesslayer = new BL_DB();
            string strErrorMessage = string.Empty;
            string strValue = "";
            strValue = $@"Update dbo.tCore_HeaderData" + iType + "_0  set dbo.tCore_HeaderData" + iType + "_0.PostingStatus=" + iPostingStatus +
                " from  dbo.tCore_HeaderData" + iType + "_0 AS CHD INNER JOIN  dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId " +
                " WHERE      (CH.iVoucherType =" + iType + ") AND (CH.sVoucherNo = N'" + vno + "')";
            clsGeneric.writeLog("Query :" + strValue);
            DataAcesslayer.GetExecute(strValue, iCompanyId, ref strErrorMessage);
            if (strErrorMessage != "")
            {
                clsGeneric.writeLog("strErrorMessage :" + strErrorMessage);
            }

        }

    } // Generalized_I2P_R2PController
} //namespace ICSF9.Controllers