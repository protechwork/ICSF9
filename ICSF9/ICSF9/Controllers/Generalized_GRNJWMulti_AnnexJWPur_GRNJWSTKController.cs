using ICSF9.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICSF9.Controllers
{
    public class Generalized_GRNJWMulti_AnnexJWPur_GRNJWSTKController : Controller
    {
        #region Variable Definition
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        int PostingStatus = 0;
        int iPvtype = 0;
        string sPAbbr = "GRNJWSTK";
        string sPName = "";
        int iPvtype1 = 0;
        string sPAbbr1 = "AnnexJWPur";
        string sPName1 = "";
        string sBAbbr = "";
        string sBName = "";
        int iPassGRNJWStk = 0;
        string sVNoGRNJWStk = "";
        string sVNoAnnexJW = "";
        int iPassAnnexJW = 0;
        int iPass = 0;
        int iEquipments__Id = 0;
        int iFGID = 0;
        decimal dFGQty = 0;
        int iRMID = 0;
        int iUnit__Id = 0;
        int iSACCode__Id = 0;
        decimal dPrvConstQty = 0;
        decimal dPORate = 0;
        decimal dRMBOMQty = 0;
        decimal dRMReqQty = 0;
        decimal dDCQty = 0;
        decimal dDCValue = 0;
        decimal dAnnexQty = 0;
        decimal dAnnexValue = 0;
        decimal dRtnAsIsQty = 0;
        decimal dRtnAsIsValue = 0;
        decimal dStock = 0;
        decimal dStkValue = 0;
        decimal dG2Post = 0;

        decimal dTotalCons = 0;
        int iLessStock = 0;

        int iScrapProd = 0;
        decimal dScrapQty = 0;
        #endregion
        public ActionResult UpdateGRNJWMulti_AnnexJWPur_GRNJWSTK(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {
            try
            {
                BL_DB DataAcesslayer = new BL_DB();
                bool blnCheck = false;
                int iCntGRNJWAML = 0;
                int iCntAnexVendrJWAML = 0;
                string errorM = string.Empty; ;
                var Sys_IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                var ComputerName = Dns.GetHostName();

                clsGeneric.Log_write(RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), SessionId, CompanyId, docNo, vtype, User);
                clsGeneric.writeLog("Sys Ip Address" + Sys_IPAddress);
                clsGeneric.writeLog("ComputerName" + ComputerName);

                strQry = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                   "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
                PostingStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                // 0,Pending, 1,Updated
                clsGeneric.writeLog("PostingStatus 0,Pending, 1,Updated  : " + PostingStatus);
                if (PostingStatus == 0)
                {



                    clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);


                    strQry = $@"SELECT  sAbbr  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType ='" + vtype + "' )";
                    sBAbbr = clsGeneric.ShowRecord(CompanyId, strQry);
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + vtype + " )";
                    sBName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));


                    strQry = $@"SELECT  iVoucherType  FROM  dbo.cCore_Vouchers_0 WHERE (sabbr ='" + sPAbbr + "' )";
                    iPvtype = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + iPvtype + " )";
                    sPName = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));

                    strQry = $@"SELECT  iVoucherType  FROM  dbo.cCore_Vouchers_0 WHERE (sAbbr='" + sPAbbr1 + "' )";
                    iPvtype1 = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                    strQry = $@"SELECT  sName  FROM  dbo.cCore_Vouchers_0 WHERE (iVoucherType =" + iPvtype1 + " )";
                    sPName1 = Convert.ToString(clsGeneric.ShowRecord(CompanyId, strQry));
                    iPass = 0;
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
                                    List<System.Collections.Hashtable> lstBody_GRNJWSTK = new List<System.Collections.Hashtable>();
                                    Hashtable headerGRNJWSTK = new Hashtable();
                                    HashData objHashRequest_GRNJWSTK = new HashData();

                                    var extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(responseDataCENT.data[0]["Body"]));
                                    if (extBody.Count > 0)
                                    {

                                        for (int k0 = 0; k0 <= extBody.Count - 1; k0++)
                                        {

                                            iFGID = Convert.ToInt32(extBody[k0]["Item__Id"]);
                                            System.Diagnostics.Debug.WriteLine("FGID :-  " + iFGID);
                                            strQry = $@" SELECT tci.iProduct, tci.fQuantity, tci.mRate, tci.iUnit, tci.mGross, tchd.FGQty, tchd.DuplicateBOM, tcibsd.mInput0, isnull(tcdN.ScrapProd,0), isnull(tcibsd.mInput1,0) " +
                                                      " FROM dbo.tCore_Header_0 AS tch INNER JOIN dbo.tCore_Data_0 AS tcd ON tch.iHeaderId = tcd.iHeaderId INNER JOIN " +
                                                     " dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId INNER JOIN " +
                                                     " dbo.tCore_HeaderData2306_0 AS tchd ON tcd.iHeaderId = tchd.iHeaderId INNER JOIN " +
                                                     " dbo.tCore_IndtaBodyScreenData_0 AS tcibsd ON tci.iBodyId = tcibsd.iBodyId Left JOIN " +
                                                     " dbo.tCore_Data2306_0 AS tcdN ON tcd.iBodyId = tcdN.iBodyId WHERE(tch.iVoucherType = 2306) AND(tchd.FGCode = " + iFGID + ") AND(tcd.iBookNo = " + Convert.ToInt32(extHeader["VendorAC__Id"]) + ")";
                                            DataSet ds0 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);

                                            if (ds0 != null)
                                            {
                                                for (int j1 = 0; j1 < ds0.Tables[0].Rows.Count; j1++)
                                                {
                                                    Hashtable bodyAnnexJWPur = new Hashtable();
                                                    strQry = $@" update ICS_MulLneJWReco set StkRate=(StkValue/Stock),G2Post= ((StkValue/Stock)*RMReqQty),TotalCons=(RMReqQty+PrvConstQty)";

                                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                                    if (strErrorMessage != "")
                                                    {
                                                        clsGeneric.writeLog("strQry: " + strQry);
                                                        clsGeneric.writeLog("strErrorMessage: " + strErrorMessage);
                                                    }
                                                    iEquipments__Id = Convert.ToInt32(extBody[k0]["Equipments__Id"]);

                                                    iSACCode__Id = Convert.ToInt32(extBody[k0]["SACCode__Id"]);
                                                    dFGQty = Convert.ToDecimal(extBody[k0]["Quantity"]);
                                                    iRMID = Convert.ToInt32(ds0.Tables[0].Rows[j1][0]);
                                                    iUnit__Id = Convert.ToInt32(ds0.Tables[0].Rows[j1][3]);
                                                    dRMBOMQty = Convert.ToDecimal(ds0.Tables[0].Rows[j1][1]);

                                                    dScrapQty = Convert.ToDecimal(ds0.Tables[0].Rows[j1][9]);
                                                    iScrapProd = Convert.ToInt32(ds0.Tables[0].Rows[j1][8]);

                                                    dDCQty = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.fQuantity)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 6145) AND (tci.iProduct = " + iRMID + ")"));
                                                    dDCValue = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.mGross)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 6145) AND (tci.iProduct = " + iRMID + ")"));

                                                    dAnnexQty = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.fQuantity)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 6153) AND (tci.iProduct = " + iRMID + ")"));
                                                    dAnnexValue = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.mGross)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 6153) AND (tci.iProduct = " + iRMID + ")"));

                                                    dRtnAsIsQty = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.fQuantity)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 1288) AND (tci.iProduct = " + iRMID + ")"));
                                                    dRtnAsIsValue = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, " SELECT ABS(SUM(tci.mGross)) FROM dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN  dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId WHERE (tch.iVoucherType = 1288) AND (tci.iProduct = " + iRMID + ")"));
                                                    strQry = $@"select sum(fQiss + fQrec) from tCore_ibals_0 where iProduct = " + iRMID + " AND(iInvTag = " + Convert.ToInt32(extHeader["VendorWarehouse__Id"]) + ")";
                                                    dStock = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, strQry));
                                                    strQry = $@"select sum(mval) from tCore_ibals_0 where iProduct = " + iRMID + " AND(iInvTag = " + Convert.ToInt32(extHeader["VendorWarehouse__Id"]) + ")";
                                                    dStkValue = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, strQry));

                                                    strQry = $@"Select Sum(TotalCons) from  ICS_MulLneJWReco where loggeduser='" + User + "' and sVno='" + docNo + "' and SessionId='" + SessionId + "' and RM_id=" + iRMID + " and id=(select  Max(id) from ICS_MulLneJWReco where loggeduser='" + User + "' and sVno='" + docNo + "' and SessionId='" + SessionId + "' and RM_id=" + iRMID + ")";
                                                    dPrvConstQty = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, strQry));

                                                    dPORate = Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k0]["PO Rate"]))["Input"]);
                                                    dRMReqQty = dRMBOMQty * dFGQty;

                                                    // strQry = $@" select dbo.EX_avgrate(""1,132580633,16,0,0)"

                                                    strQry = $@" insert into ICS_MulLneJWReco(SACCode__Id,iEquipments__Id,FGID,FGQty,RM_Id,PORate,RMBOMQty,RMReqQty,Stock,StkValue,PrvConstQty,loggeduser,sVno,SessionId,ScrapID,ScrapQty) values (" +
                                                        iSACCode__Id + "," + iEquipments__Id + "," + iFGID + "," + dFGQty + "," + iRMID + "," + dPORate + "," + dRMBOMQty + "," + dRMReqQty + "," + dStock + "," + dStkValue + "," + dPrvConstQty + ",'" + User + "','" + docNo + "','" + SessionId + "'," + iScrapProd + "," + dScrapQty + ")";

                                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                                    // this was commented previuosly 
                                                    strQry = $@" update ICS_MulLneJWReco set StkRate=(StkValue/Stock),G2Post= ((StkValue/Stock)*RMReqQty),TotalCons=(RMReqQty+PrvConstQty)";

                                                    DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                                    if (strErrorMessage != "")
                                                    {
                                                        clsGeneric.writeLog("strQry: " + strQry);
                                                        clsGeneric.writeLog("strErrorMessage: " + strErrorMessage);
                                                    }

                                                }
                                            }

                                        }
                                        strQry = $@" update ICS_MulLneJWReco set StkRate=(StkValue/Stock),G2Post= ((StkValue/Stock)*RMReqQty),TotalCons=(RMReqQty+PrvConstQty)";
                                        DataAcesslayer.GetExecute(strQry, CompanyId, ref strErrorMessage);
                                        if (strErrorMessage != "")
                                        {
                                            clsGeneric.writeLog("strQry: " + strQry);
                                            clsGeneric.writeLog("strErrorMessage: " + strErrorMessage);
                                        }



                                        List<System.Collections.Hashtable> lstBody_AnnexJWPur = new List<System.Collections.Hashtable>();
                                        Hashtable headerAnnexJWPur = new Hashtable();
                                        HashData objHashRequest_AnnexJWPur = new HashData();



                                        strQry = $@" Select  count(0) from ICS_MulLneJWReco where   Stock < TotalCons and SessionId = '" + SessionId + "' and sVno = '" + docNo + "' and loggedUser = '" + User + "'";
                                        iLessStock = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, strQry));
                                        if (iLessStock > 0)
                                        {
                                            //Stock <= TotalCons and
                                            strQry = $@" Select FGID,FGQTY,RM_ID,RMReqQty,stock,PrvConstQty,TotalCons from ICS_MulLneJWReco where  SessionId = '" + SessionId + "' and sVno = '" + docNo + "' and loggedUser = '" + User + "'";
                                            DataSet ds01 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                            if (ds01 != null)
                                            {
                                                msg.AppendLine("FGID,FGQTY,RM_ID,RMReqQty,stock");
                                                for (int jq = 0; jq < ds01.Tables[0].Rows.Count; jq++)
                                                {
                                                    var fgName = clsGeneric.ShowRecord(CompanyId, "select sName from mCore_Product where iMasterId=" + ds01.Tables[0].Rows[jq][0].ToString());
                                                    var rmName = clsGeneric.ShowRecord(CompanyId, "select sName from mCore_Product where iMasterId=" + ds01.Tables[0].Rows[jq][2].ToString());

                                                    msg.AppendLine(fgName + "," + ds01.Tables[0].Rows[jq][1] + "," + rmName + "," + ds01.Tables[0].Rows[jq][3] + "," + ds01.Tables[0].Rows[jq][4]); // + "," + ds01.Tables[0].Rows[jq][5] + "," + ds01.Tables[0].Rows[jq][6]);
                                                }
                                            }

                                            strQry = $@" Select RW.RM_ID,PRD.sName, RW.stock,SUM(RW.RMReqQty)[Req] from ICS_MulLneJWReco RW INNER JOIN mCore_Product PRD ON RW.RM_ID=PRD.iMasterId where RW.Stock <= RW.TotalCons and RW.SessionId = '" + SessionId + "' and RW.sVno = '" + docNo + "' and RW.loggedUser = '" + User + "'  GROUP BY RW.RM_ID,RW.stock,PRD.sName  ORDER BY RW.RM_ID ASC";
                                            ds01 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                            if (ds01 != null)
                                            {
                                                msg.AppendLine("RM Code,RM Name,Stock,Req");
                                                for (int i = 0; i < ds01.Tables[0].Rows.Count; i++)
                                                {
                                                    msg.AppendLine(ds01.Tables[0].Rows[i][1] + ","
                                                                    + ds01.Tables[0].Rows[i][1] + ","
                                                                    + ds01.Tables[0].Rows[i][2] + ","
                                                                    + ds01.Tables[0].Rows[i][3]);
                                                }
                                            }

                                            clsGeneric.writeLog(docNo + ".log", msg.ToString());

                                            return Json(new { status = false, data = new { message = "Stock Not Sufficent", text_data = msg.ToString() } });
                                        }
                                        strQry = $@" Select iEquipments__Id,FGID,FGQTY,PORate,RM_ID,RMBOMQty, abs(RMReqQty),stock,StkValue,StkRate,g2post,PrvConstQty,TotalCons,SACCode__ID,ScrapID,ScrapQty from ICS_MulLneJWReco " +
                                                  " where SessionId='" + SessionId + "' and sVno='" + docNo + "' and loggedUser = '" + User + "' Order By FGID";
                                        DataSet ds1 = DataAcesslayer.GetData(strQry, CompanyId, ref strErrorMessage);
                                        if (ds1 != null)
                                        {
                                            for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                                            {
                                                Hashtable bodyAnnexJWPur = new Hashtable();
                                                bodyAnnexJWPur.Add("Equipments__Id", Convert.ToInt32(ds1.Tables[0].Rows[j][0]));
                                                bodyAnnexJWPur.Add("Item__Id", Convert.ToInt32(ds1.Tables[0].Rows[j][4]));
                                                //bodyAnnexJWPur.Add("Unit__Id", Convert.ToInt32(ds1.Tables[0].Rows[j][13]));
                                                bodyAnnexJWPur.Add("FGItem__Id", Convert.ToInt32(ds1.Tables[0].Rows[j][1]));
                                                bodyAnnexJWPur.Add("ScrapProd__Id", Convert.ToInt32(ds1.Tables[0].Rows[j][14]));

                                                Hashtable bodyAnnexJWPur_BOMQty = new Hashtable
                                                {
                                                    {"Input", Convert.ToDecimal(ds1.Tables[0].Rows[j][5])},
                                                    {"FieldName", "BOM Qty"},
                                                    {"FieldId", 1854},
                                                    {"ColMap", 0}
                                                };
                                                bodyAnnexJWPur.Add("BOM Qty", bodyAnnexJWPur_BOMQty);
                                                Hashtable bodyAnnexJWPur_GRNJWQty = new Hashtable
                                                {
                                                    {"Input",  Convert.ToDecimal(ds1.Tables[0].Rows[j][2])},
                                                    {"FieldName", "GRN JW Qty"},
                                                    {"FieldId", 1855},
                                                    {"ColMap", 1}
                                                };
                                                bodyAnnexJWPur.Add("GRN JW Qty", bodyAnnexJWPur_GRNJWQty);

                                                Hashtable bodyAnnexJWPur_ScrapBOMQty = new Hashtable
                                                    {
                                                        {"Input",  Convert.ToDecimal(ds1.Tables[0].Rows[j][15])},
                                                        {"FieldName", "Scrap BOM Qty"},
                                                        {"FieldId", 1860},
                                                        {"ColMap", 2}
                                                    };
                                                bodyAnnexJWPur.Add("Scrap BOM Qty", bodyAnnexJWPur_ScrapBOMQty);
                                                Hashtable bodyAnnexJWPur_ScrapQty = new Hashtable
                                                {
                                                        {"Input", Convert.ToDecimal(ds1.Tables[0].Rows[j][2]) * Convert.ToDecimal(ds1.Tables[0].Rows[j][15])},
                                                        {"FieldName", "Scrap Qty"},
                                                        {"FieldId", 1861},
                                                        {"ColMap", 3}
                                                };
                                                bodyAnnexJWPur.Add("Scrap Qty", bodyAnnexJWPur_ScrapQty);


                                                bodyAnnexJWPur.Add("Quantity", Convert.ToDecimal(ds1.Tables[0].Rows[j][6]));
                                                //bodyAnnexJWPur.Add("Rate", Convert.ToDecimal(ds1.Tables[0].Rows[j][9]));
                                                bodyAnnexJWPur.Add("Gross", Convert.ToDecimal(ds1.Tables[0].Rows[j][10]));

                                                lstBody_AnnexJWPur.Add(bodyAnnexJWPur);


                                            }

                                        }

                                        headerAnnexJWPur.Add("DocNo", "");
                                        headerAnnexJWPur.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                        //headerAnnexJWPur.Add("CustomerAC__Id", Convert.ToInt32(extHeader["VendorAC__Id"]));

                                        headerAnnexJWPur.Add("CustomerAC__Id", Convert.ToInt32(extHeader["VendorAC__Id"]));
                                        headerAnnexJWPur.Add("Project__Id", Convert.ToInt32(extHeader["Project__Id"]));
                                        headerAnnexJWPur.Add("Dept__Id", Convert.ToInt32(extHeader["Dept__Id"]));
                                        // As per suggestion from Ali And Majid Sahab Dated 07-05-2023 changed "VendorWarehouse__Id" 
                                        //headerAnnexJWPur.Add("Warehouse__Id", Convert.ToInt32(extHeader["Warehouse__Id"]));

                                        //headerAnnexJWPur.Add("Warehouse__Id", Convert.ToInt32(extHeader["VendorWarehouse__Id"]));
                                        // Majid Bhai suggested Change 01-04-2023 
                                        headerAnnexJWPur.Add("Warehouse__Id", Convert.ToInt32(extHeader["VendorWarehouse__Id"]));

                                        headerAnnexJWPur.Add("Tax Code__Id", Convert.ToInt32(extHeader["Tax Code__Id"]));

                                        headerAnnexJWPur.Add("BaseDocumentNo_", docNo);
                                        headerAnnexJWPur.Add("BaseDocumentDate", extHeader["Date"]);

                                        //headerAnnexJWPur.Add("VendorWarehouse__Id", Convert.ToInt32(extHeader["Warehouse__Id"]));

                                        //headerAnnexJWPur.Add("AWarehouse__Id", Convert.ToInt32(extHeader["AWarehouse__Id"]));
                                        //headerAnnexJWPur.Add("RWarehouse__Id", Convert.ToInt32(extHeader["RWarehouse__Id"]));
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["sNarration"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("sNarration", extHeader["sNarration"].ToString());
                                        }
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["GateEntryNo"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("GateEntryNo", extHeader["GateEntryNo"].ToString());
                                        }
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["GateEntryDate"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("GateEntryDate", Convert.ToInt32(extHeader["GateEntryDate"]));
                                        }
                                        //headerAnnexJWPur.Add("PlaceofSupply__Id", Convert.ToInt32(extHeader["PlaceofSupply__Id"]));
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["DCNo"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("DCNo", extHeader["DCNo"].ToString());
                                        }
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["DCDate"]));
                                        if (!blnCheck)
                                        {

                                            headerAnnexJWPur.Add("DCDate", Convert.ToInt32(extHeader["DCDate"]));
                                        }
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["BillNo"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("BillNo", extHeader["BillNo"]);
                                        }
                                        else
                                        {
                                            headerAnnexJWPur.Add("BillNo", "");
                                        }
                                        blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["BillDate"]));
                                        if (!blnCheck)
                                        {
                                            headerAnnexJWPur.Add("BillDate", Convert.ToInt32(extHeader["BillDate"]));
                                        }
                                        bool b0 = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["VehicleNo"]));
                                        if (!b0)
                                        {
                                            headerAnnexJWPur.Add("VehicleNo", extHeader["VehicleNo"].ToString());
                                        }
                                        bool b1 = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["LRNo"]));
                                        if (!b1)
                                        {
                                            headerAnnexJWPur.Add("LRNo", extHeader["LRNo"].ToString());
                                        }
                                        bool b2 = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["LRDate"]));
                                        if (!b2)
                                        {
                                            headerAnnexJWPur.Add("LRDate", Convert.ToInt32(extHeader["LRDate"]));
                                        }




                                        System.Collections.Hashtable objHash_AnnexJWPur = new System.Collections.Hashtable();
                                        objHash_AnnexJWPur.Add("Body", lstBody_AnnexJWPur);
                                        objHash_AnnexJWPur.Add("Header", headerAnnexJWPur);
                                        List<System.Collections.Hashtable> lstHash_AnnexJWPur = new List<System.Collections.Hashtable>();
                                        lstHash_AnnexJWPur.Add(objHash_AnnexJWPur);
                                        objHashRequest_AnnexJWPur.data = lstHash_AnnexJWPur;
                                        string sContent_AnnexJWPur = JsonConvert.SerializeObject(objHashRequest_AnnexJWPur);
                                        clsGeneric.writeLog("Upload AnnexJWPur :" + "http://localhost/Focus8API/Transactions/Vouchers/" + sPName1);
                                        clsGeneric.writeLog("URL Param :" + sContent_AnnexJWPur);

                                        using (var clientAnnexJWPur = new WebClient())
                                        {
                                            clientAnnexJWPur.Encoding = Encoding.UTF8;
                                            clientAnnexJWPur.Headers.Add("fSessionId", SessionId);
                                            clientAnnexJWPur.Headers.Add("Content-Type", "application/json");
                                            var responseAnnexJWPur = clientAnnexJWPur.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + sPName1, sContent_AnnexJWPur);
                                            clsGeneric.writeLog("response AnnexJWPur: " + responseAnnexJWPur);
                                            if (responseAnnexJWPur != null)
                                            {
                                                var responseDataAnnexJWPur = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseAnnexJWPur);
                                                if (responseDataAnnexJWPur.result == -1)
                                                {
                                                    //UpdateStatus(vtype, docNo, 0, CompanyId);
                                                    //clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);
                                                    //deleteVoucher(sVNoGRNJWStk, sPAbbr, SessionId);
                                                    iPassGRNJWStk = 0;
                                                    errorM = responseDataAnnexJWPur.message;
                                                    //return Json(new { status = false, data = new { message = responseDataAnnexJWPur.message } });
                                                }
                                                else
                                                {
                                                    //if (isBatchYes == 1)
                                                    //{

                                                    var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataAnnexJWPur.data[0]["VoucherNo"]));
                                                    iPassAnnexJW = iPassAnnexJW + 1;
                                                    sVNoAnnexJW = iMasterId;
                                                    //    UpdateBatchs(iPvtype, iDate, iMasterId, CompanyId);
                                                    //}
                                                    //UpdateStatus(vtype, docNo, 1, CompanyId);
                                                    iPass = iPass++;
                                                    //clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);

                                                    string strValue = "http://localhost/focus8api/Screen/Transactions/resavevoucher/" + iPvtype1 + "/" + iMasterId;

                                                    //using (var client = new WebClient())
                                                    //{

                                                    //    //client.Encoding = Encoding.UTF8;
                                                    //    client.Headers.Add("fSessionId", SessionId);
                                                    //    client.Headers.Add("Content-Type", "application/json");
                                                    //    var response = client.DownloadString(strValue);
                                                    //    clsGeneric.writeLog("URL OF ReSave" + strValue);
                                                    //    clsGeneric.writeLog("Response of Resave" + response);
                                                    //}


                                                    //return Json(new { status = true, data = new { message = "Posting Successful" } });
                                                }
                                            }

                                        } // using (var clientAnnexJWPur = new WebClient())


                                        if (iPassAnnexJW != 0)
                                        {
                                            
                                            for (int k1 = 0; k1 <= extBody.Count - 1; k1++)
                                            {
                                                Hashtable bodyGRNJWSTK = new Hashtable();
                                                bodyGRNJWSTK.Add("Equipments__Id", Convert.ToInt32(extBody[k1]["Equipments__Id"]));
                                                bodyGRNJWSTK.Add("Item__Id", Convert.ToInt32(extBody[k1]["Item__Id"]));
                                                bodyGRNJWSTK.Add("Unit__Id", Convert.ToInt32(extBody[k1]["Unit__Id"]));

                                                Hashtable bodyGRNJWSTK_POQty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["PO Qty"]))["Input"])},
                                                {"FieldName", "PO Qty"},
                                                {"FieldId", 1830},
                                                {"ColMap", 0}
                                            };
                                                bodyGRNJWSTK.Add("PO Qty", bodyGRNJWSTK_POQty);
                                                Hashtable bodyGRNJWSTK_ChallanQty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Challan Qty"]))["Input"])},
                                                {"FieldName", "Challan Qty"},
                                                {"FieldId", 1831},
                                                {"ColMap", 1}
                                            };
                                                bodyGRNJWSTK.Add("Challan Qty", bodyGRNJWSTK_ChallanQty);

                                                Hashtable bodyGRNJWSTK_ReceivedQty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Received Qty"]))["Input"])},
                                                {"FieldName", "Received Qty"},
                                                {"FieldId", 1832},
                                                {"ColMap", 2}
                                            };
                                                bodyGRNJWSTK.Add("Received Qty", bodyGRNJWSTK_ReceivedQty);

                                                Hashtable bodyGRNJWSTK_Short_Excess_Qty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Short/Excess Qty"]))["Input"])},
                                                {"FieldName", "Short/Excess Qty"},
                                                {"FieldId", 1833},
                                                {"ColMap", 3}
                                            };
                                                bodyGRNJWSTK.Add("Short/Excess Qty", bodyGRNJWSTK_Short_Excess_Qty);


                                                Hashtable bodyGRNJWSTK_Rejected_Qty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Rejected Qty"]))["Input"])},
                                                {"FieldName", "Rejected Qty"},
                                                {"FieldId", 1834},
                                                {"ColMap", 4}
                                            };
                                                bodyGRNJWSTK.Add("Rejected Qty", bodyGRNJWSTK_Rejected_Qty);

                                                Hashtable bodyGRNJWSTK_PORate = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["PO Rate"]))["Input"])},
                                                {"FieldName", "PO Rate"},
                                                {"FieldId", 1835},
                                                {"ColMap", 5}
                                            };
                                                bodyGRNJWSTK.Add("PO Rate", bodyGRNJWSTK_PORate);

                                                Hashtable bodyGRNJWSTK_ChallanRate = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Challan Rate"]))["Input"])},
                                                {"FieldName", "Challan Rate"},
                                                {"FieldId", 1836},
                                                {"ColMap", 6}
                                            };
                                                bodyGRNJWSTK.Add("Challan Rate", bodyGRNJWSTK_ChallanRate);

                                                Hashtable bodyGRNJWSTK_PassRate = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Pass Rate"]))["Input"])},
                                                {"FieldName", "Pass Rate"},
                                                {"FieldId", 1851},
                                                {"ColMap", 21}
                                            };
                                                bodyGRNJWSTK.Add("Pass Rate", bodyGRNJWSTK_PassRate);

                                                Hashtable bodyGRNJWSTK_GrossAmount = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Gross Amount"]))["Input"])},
                                                {"FieldName", "Gross Amount"},
                                                {"FieldId", 1837},
                                                {"ColMap",7}
                                            };
                                                bodyGRNJWSTK.Add("Gross Amount", bodyGRNJWSTK_GrossAmount);

                                                Hashtable bodyGRNJWSTK_Discount = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Discount"]))["Input"])},
                                                {"FieldName", "Discount"},
                                                {"FieldId", 1838},
                                                {"ColMap",8}
                                            };
                                                bodyGRNJWSTK.Add("Discount", bodyGRNJWSTK_Discount);


                                                Hashtable bodyGRNJWSTK_PackForwd = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Pack Forwd"]))["Input"])},
                                                {"FieldName", "Pack Forwd"},
                                                {"FieldId", 1839},
                                                {"ColMap",9}
                                            };
                                                bodyGRNJWSTK.Add("Pack Forwd", bodyGRNJWSTK_PackForwd);

                                                Hashtable bodyGRNJWSTK_FreightGST = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Freight GST"]))["Input"])},
                                                {"FieldName", "Freight GST"},
                                                {"FieldId", 1840},
                                                {"ColMap",10}
                                            };
                                                bodyGRNJWSTK.Add("Freight GST", bodyGRNJWSTK_FreightGST);

                                                Hashtable bodyGRNJWSTK_Add_Less_Other = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Add/Less Other"]))["Input"])},
                                                {"FieldName", "Add/Less Other"},
                                                {"FieldId", 1841},
                                                {"ColMap",11}
                                            };
                                                bodyGRNJWSTK.Add("Add/Less Other", bodyGRNJWSTK_Add_Less_Other);

                                                Hashtable bodyGRNJWSTK_Taxable_Value = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Taxable Value"]))["Input"])},
                                                {"FieldName", "Taxable Value"},
                                                {"FieldId", 1842},
                                                {"ColMap",12}
                                            };
                                                bodyGRNJWSTK.Add("Taxable Value", bodyGRNJWSTK_Taxable_Value);

                                                Hashtable bodyGRNJWSTK_CGST = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["CGST"]))["Input"])},
                                                {"FieldName", "CGST"},
                                                {"FieldId", 1843},
                                                {"ColMap",13}
                                            };
                                                bodyGRNJWSTK.Add("CGST", bodyGRNJWSTK_CGST);

                                                Hashtable bodyGRNJWSTK_SGST = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["SGST"]))["Input"])},
                                                {"FieldName", "SGST"},
                                                {"FieldId", 1844},
                                                {"ColMap",14}
                                            };
                                                bodyGRNJWSTK.Add("SGST", bodyGRNJWSTK_SGST);


                                                Hashtable bodyGRNJWSTK_IGST = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["IGST"]))["Input"])},
                                                {"FieldName", "IGST"},
                                                {"FieldId", 1845},
                                                {"ColMap",15}
                                            };
                                                bodyGRNJWSTK.Add("IGST", bodyGRNJWSTK_IGST);

                                                Hashtable bodyGRNJWSTK_Cess = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Cess"]))["Input"])},
                                                {"FieldName", "Cess"},
                                                {"FieldId", 1846},
                                                {"ColMap",16}
                                            };
                                                bodyGRNJWSTK.Add("Cess", bodyGRNJWSTK_Cess);

                                                strQry = $@" SELECT SUM(tci.mGross) AS mgross FROM     dbo.tCore_Data_0 AS tcd INNER JOIN dbo.tCore_Header_0 AS tch ON tcd.iHeaderId = tch.iHeaderId INNER JOIN " +
                                                    " dbo.tCore_Indta_0 AS tci ON tcd.iBodyId = tci.iBodyId INNER JOIN dbo.tCore_Data6153_0 AS tcdn ON tci.iBodyId = tcdn.iBodyId WHERE  (tch.iVoucherType = 6153) AND (tch.sVoucherNo = N'" + sVNoAnnexJW + "') AND (tcdn.FGItem = "+ Convert.ToInt32(extBody[k1]["Item__Id"]) + ")";
                                                clsGeneric.writeLog("strQry: " + strQry);
                                                var aa = Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId, strQry));
                                                clsGeneric.writeLog("mGross: " + aa);
                                                Hashtable bodyGRNJWSTK_RMSTKValue = new Hashtable
                                                {
                                                    /*{"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["RM Stock Value"]))["Input"])},*/
                                                    //{"Input", Convert.ToDecimal(clsGeneric.ShowRecord(CompanyId,"Select sum(g2pOST) from ICS_MulLneJWReco where loggeduser='" + User +"' and sVno='"+ docNo +"' and SessionId='"+ SessionId +"' and Fgid=" + Convert.ToInt32(extBody[k1]["Item__Id"]) ))},
                                                   
                                                { "Input", aa},
                                                { "FieldName", "RM Stock Value"},
                                                {"FieldId", 1847},
                                                {"ColMap",17}
                                            };
                                                bodyGRNJWSTK.Add("RM Stock Value", bodyGRNJWSTK_RMSTKValue);
                                                Hashtable bodyGRNJWSTK_LessGross = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Less Gross"]))["Input"])},
                                                {"FieldName", "Less Gross"},
                                                {"FieldId", 1848},
                                                {"ColMap",18}
                                            };
                                                bodyGRNJWSTK.Add("Less Gross", bodyGRNJWSTK_LessGross);

                                                Hashtable bodyGRNJWSTK_DebitQty = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Debit Qty"]))["Input"])},
                                                {"FieldName", "Debit Qty"},
                                                {"FieldId", 1849},
                                                {"ColMap",19}
                                            };
                                                bodyGRNJWSTK.Add("Debit Qty", bodyGRNJWSTK_DebitQty);

                                                Hashtable bodyGRNJWSTK_DebitRate = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Debit Rate"]))["Input"])},
                                                {"FieldName", "Debit Rate"},
                                                {"FieldId", 1850},
                                                {"ColMap",20}
                                            };
                                                bodyGRNJWSTK.Add("Debit Rate", bodyGRNJWSTK_DebitRate);

                                                Hashtable bodyGRNJWSTK_StkRate = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Stk Rate"]))["Input"])},
                                                {"FieldName", "Stk Rate"},
                                                {"FieldId", 1852},
                                                {"ColMap",22}
                                            };
                                                bodyGRNJWSTK.Add("Stk Rate", bodyGRNJWSTK_StkRate);

                                                Hashtable bodyGRNJWSTK_StkValue = new Hashtable
                                            {
                                                {"Input", Convert.ToDecimal(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extBody[k1]["Stk Value"]))["Input"])},
                                                {"FieldName", "Stk Value"},
                                                {"FieldId", 1853},
                                                {"ColMap",23}
                                            };
                                                bodyGRNJWSTK.Add("Stk Value", bodyGRNJWSTK_StkValue);

                                                bodyGRNJWSTK.Add("SACCode__Id", Convert.ToInt32(extBody[k1]["SACCode__Id"]));
                                                bodyGRNJWSTK.Add("Quantity", Convert.ToDecimal(extBody[k1]["Quantity"]));
                                                bodyGRNJWSTK.Add("Rate", Convert.ToDecimal(extBody[k1]["Rate"]));
                                                bodyGRNJWSTK.Add("Gross", Convert.ToDecimal(extBody[k1]["Gross"]));
                                                bool b01 = clsGeneric.TstNullOrEmpty(Convert.ToString(extBody[k1]["sRemarks"]));
                                                if (!b01)
                                                {
                                                    bodyGRNJWSTK.Add("sRemarks", extBody[k1]["sRemarks"].ToString());
                                                }
                                                //bool b1 = string.IsNullOrEmpty(extBody[k1]["PONo"].ToString());

                                                bool b001 = clsGeneric.TstNullOrEmpty(extBody[k1]["PONo"].ToString());
                                                if (!b001)
                                                {
                                                    bodyGRNJWSTK.Add("PONo", extBody[k1]["PONo"].ToString());
                                                }



                                                bodyGRNJWSTK.Add("PODate", Convert.ToInt32(extBody[k1]["PODate"]));
                                                //bool b2 = clsGeneric.TstNullOrEmpty(extBody[k1]["GRNNo"].ToString());
                                                //if (!b2)
                                                //{
                                                //    bodyGRNJWSTK.Add("GRNNo", extBody[k1]["GRNNo"].ToString());
                                                //}
                                                //bodyGRNJWSTK.Add("GRNDate", Convert.ToInt32(extBody[k1]["GRNDate"]));

                                                lstBody_GRNJWSTK.Add(bodyGRNJWSTK);
                                            }

                                            headerGRNJWSTK.Add("DocNo", "");
                                            headerGRNJWSTK.Add("Date", Convert.ToInt32(extHeader["Date"]));
                                            headerGRNJWSTK.Add("VendorAC__Id", Convert.ToInt32(extHeader["VendorAC__Id"]));
                                            headerGRNJWSTK.Add("Project__Id", Convert.ToInt32(extHeader["Project__Id"]));
                                            headerGRNJWSTK.Add("Dept__Id", Convert.ToInt32(extHeader["Dept__Id"]));

                                            //headerGRNJWSTK.Add("Warehouse__Id", Convert.ToInt32(extHeader["VendorWarehouse__Id"]));
                                            // Majid Bhai suggested Change 01-04-2023 


                                            //headerGRNJWSTK.Add("Warehouse__Id", Convert.ToInt32(extHeader["Warehouse__Id"]));

                                            //As per suggestion By Ali Bhai @ 06-05-2023 
                                            headerGRNJWSTK.Add("Warehouse__Id", Convert.ToInt32(extHeader["Warehouse__Id"]));

                                            headerGRNJWSTK.Add("BaseDocumentNo_", docNo);
                                            headerGRNJWSTK.Add("BaseDocumentDate", extHeader["Date"]);
                                            headerGRNJWSTK.Add("Tax Code__Id", Convert.ToInt32(extHeader["Tax Code__Id"]));
                                            headerGRNJWSTK.Add("GateEntryNo", (extHeader["GateEntryNo"].ToString()));
                                            headerGRNJWSTK.Add("GateEntryDate", Convert.ToInt32(extHeader["GateEntryDate"]));
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["DCNo"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("DCNo", (extHeader["DCNo"].ToString()));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("DCNo", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["DCDate"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("DCDate", Convert.ToInt32(extHeader["DCDate"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("DCDate", 0);
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["BillNo"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("BillNo", (extHeader["BillNo"].ToString()));

                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("BillNo", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["BillDate"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("BillDate", Convert.ToInt32(extHeader["BillDate"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("BillDate", 0);
                                            }
                                            bool b101 = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["VehicleNo"]));
                                            if (!b101)
                                            {
                                                headerGRNJWSTK.Add("VehicleNo", (extHeader["VehicleNo"].ToString()));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("VehicleNo", "");
                                            }
                                            bool b102 = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["LRNo"]));
                                            if (!b102)
                                            {
                                                headerGRNJWSTK.Add("LRNo", (extHeader["LRNo"].ToString()));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("LRNo", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["LRDate"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("LRDate", Convert.ToInt32(extHeader["LRDate"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("LRDate", 0);
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["RemarksStore"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("RemarksStore", (extHeader["RemarksStore"].ToString()));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("RemarksStore", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["sNarration"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("sNarration", (extHeader["sNarration"].ToString()));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("sNarration", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["RemarksQC"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("RemarksQC", (extHeader["RemarksQC"].ToString()));

                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("RemarksQC", "");
                                            }
                                            headerGRNJWSTK.Add("PlaceofSupply__Id", Convert.ToInt32(extHeader["PlaceofSupply__Id"]));
                                            headerGRNJWSTK.Add("DeliveryExpectedDt", Convert.ToInt32(extHeader["DeliveryExpectedDt"]));
                                            headerGRNJWSTK.Add("IsIGST", Convert.ToInt32(extHeader["IsIGST"]));
                                            headerGRNJWSTK.Add("POAdvReq", Convert.ToInt32(extHeader["POAdvReq"]));
                                            headerGRNJWSTK.Add("POAdvAmt", Convert.ToDecimal(extHeader["POAdvAmt"]));
                                            headerGRNJWSTK.Add("Attachment", null);
                                            headerGRNJWSTK.Add("AnnexureStatus", 0);
                                            headerGRNJWSTK.Add("CreditDayOption", Convert.ToInt32(extHeader["CreditDayOption"]));
                                            headerGRNJWSTK.Add("CreditDay", Convert.ToInt32(extHeader["CreditDay"]));


                                            headerGRNJWSTK.Add("VendorWarehouse__Id", Convert.ToInt32(extHeader["VendorWarehouse__Id"]));


                                            headerGRNJWSTK.Add("AWarehouse__Id", Convert.ToInt32(extHeader["AWarehouse__Id"]));
                                            headerGRNJWSTK.Add("RWarehouse__Id", Convert.ToInt32(extHeader["RWarehouse__Id"]));
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Freight Amt"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Freight Amt", Convert.ToDecimal(extHeader["Freight Amt"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Freight Amt", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Insurance Amt"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Insurance Amt", Convert.ToDecimal(extHeader["Insurance Amt"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Insurance Amt", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Other Charges"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Other Charges", Convert.ToDecimal(extHeader["Other Charges"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Other Charges", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Round Off"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Round Off", Convert.ToDecimal(extHeader["Round Off"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Round Off", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Bill Amt"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Bill Amt", Convert.ToDecimal(extHeader["Bill Amt"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Bill Amt", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["RM Cost F"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("RM Cost F", Convert.ToDecimal(extHeader["RM Cost F"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("RM Cost F", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["Process Cost F"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("Process Cost F", Convert.ToDecimal(extHeader["Process Cost F"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("Process Cost F", "");
                                            }
                                            blnCheck = clsGeneric.TstNullOrEmpty(Convert.ToString(extHeader["FG Rate F"]));
                                            if (!blnCheck)
                                            {
                                                headerGRNJWSTK.Add("FG Rate F", Convert.ToDecimal(extHeader["FG Rate F"]));
                                            }
                                            else
                                            {
                                                headerGRNJWSTK.Add("FG Rate F", "");
                                            }
                                            //headerGRNJWSTK.Add("AdvBankAcct__Id", Convert.ToInt32(extHeader["AdvBankAcct__Id"]));
                                            Hashtable footerGRNJWSTK = new Hashtable();
                                           

                                            System.Collections.Hashtable objHash_GRNJWSTK = new System.Collections.Hashtable();
                                            objHash_GRNJWSTK.Add("Body", lstBody_GRNJWSTK);
                                            objHash_GRNJWSTK.Add("Header", headerGRNJWSTK);
                                            List<System.Collections.Hashtable> lstHash_GRNJWSTK = new List<System.Collections.Hashtable>();
                                            lstHash_GRNJWSTK.Add(objHash_GRNJWSTK);
                                            objHashRequest_GRNJWSTK.data = lstHash_GRNJWSTK;
                                            string sContent_GRNJWSTK = JsonConvert.SerializeObject(objHashRequest_GRNJWSTK);
                                            clsGeneric.writeLog("Upload GRNJWSTK :" + "http://localhost/Focus8API/Transactions/Vouchers/" + sPName);
                                            clsGeneric.writeLog("URL Param :" + sContent_GRNJWSTK);
                                            iPassGRNJWStk = 0;
                                            sVNoGRNJWStk = string.Empty;
                                            using (var clientGRNJWSTK = new WebClient())
                                            {
                                                clientGRNJWSTK.Encoding = Encoding.UTF8;
                                                clientGRNJWSTK.Headers.Add("fSessionId", SessionId);
                                                clientGRNJWSTK.Headers.Add("Content-Type", "application/json");
                                                var responseGRNJWSTK = clientGRNJWSTK.UploadString("http://localhost/Focus8API/Transactions/Vouchers/" + sPName, sContent_GRNJWSTK);
                                                clsGeneric.writeLog("response GRNJWSTK: " + responseGRNJWSTK);
                                                if (responseGRNJWSTK != null)
                                                {
                                                    var responseDataGRNJWSTK = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseGRNJWSTK);
                                                    if (responseDataGRNJWSTK.result == -1)
                                                    {
                                                        iPassGRNJWStk = 0;
                                                        //errorM = responseDataGRNJWSTK.message;
                                                        deleteVoucher(sVNoAnnexJW, sPAbbr1, SessionId);
                                                        UpdateStatus(vtype, docNo, 0, CompanyId);
                                                        clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);
                                                        return Json(new { status = false, data = new { message = responseDataGRNJWSTK.message } });
                                                    }
                                                    else
                                                    {
                                                        sVNoGRNJWStk = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataGRNJWSTK.data[0]["VoucherNo"]));
                                                        iPassGRNJWStk = 1;


                                                        //        //if (isBatchYes == 1)
                                                        //        //{
                                                        //        //    var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(responseDataCBROD.data[0]["VoucherNo"]));
                                                        //        //    UpdateBatchs(iPvtype, iDate, iMasterId, CompanyId);
                                                        //        //}
                                                        clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);
                                                        UpdateStatus(vtype, docNo, 1, CompanyId);
                                                        return Json(new { status = true, data = new { message = "Posting Successful" } });
                                                    }
                                                }

                                            } // using (var clientGRNJWSTK = new WebClient())

                                        }
                                        else
                                        {
                                            return Json(new { status = true, data = new { message = errorM } });
                                        }

                                    } //if (extBody.Count - 1 > 0)

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
                clsGeneric.createTableRMReqQty(CompanyId, User, docNo);
                clsGeneric.createTableICS_MulLneJWReco(CompanyId, SessionId, User, docNo);
                deleteVoucher(sVNoGRNJWStk, sPAbbr, SessionId);
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

        static bool deleteVoucher(string ldocNo, string vName, string iSessionId)
        {
            using (var client = new WebClient())
            {

                client.Encoding = Encoding.UTF8;
                client.Headers.Add("fSessionId", iSessionId);
                client.Headers.Add("Content-Type", "application/json");
                string url = "http://localhost/Focus8API/Transactions/" + vName + "/" + ldocNo;
                clsGeneric.writeLog("url of " + vName + " :" + url);
                var response = client.UploadString(url, "DELETE", "");
                clsGeneric.writeLog("Response form " + vName + ":" + response);
                if (response != null)
                {
                    var response_DEL_Data = JsonConvert.DeserializeObject<APIResponse.PostResponse>(response);
                    if (response_DEL_Data.result == -1)
                    {
                        return false;

                    }
                    else
                    {
                        return true;

                    }
                }
            }
            return false;
        }

    }
}