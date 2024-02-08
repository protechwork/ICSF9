
using Focus.Common.DataStructs;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;

namespace ICSF9.Models
{
    public class clsGeneric
    {
        public static string DecimalCustomFormat(double Digits)
        {
            return string.Format("{0:0.00}", Digits).Replace(".00", "");

        }
        public static void writeLog(string sContent)
        {
            //if (Global.logEnabled)
                FConvert.LogFile(DateTime.Now.ToString("MMyy") + "ICSF9{0}.log", DateTime.Now.ToString() + " : " + sContent);

        }
        public static void writeLog(string FileName, string sContent)
        {
            //if (Global.logEnabled)
            FConvert.LogFile(DateTime.Now.ToString("MMyy") + "_" + FileName, sContent);

        }

        public static void RMwriteLog(string sContent)
        {
            FConvert.LogFile(DateTime.Now.ToString("MMyy") + "RMstock.log", DateTime.Now.ToString() + " : " + sContent);

        }
        public static void writeLogSavingFailed(string sContent)
        {
            FConvert.LogFile(DateTime.Now.ToString("MMyy") + "AdiswaraPostingFailed.log", DateTime.Now.ToString() + " : " + sContent);
        }
        public static void writeLogJson(string sContent)
        {
            FConvert.LogFile(DateTime.Now.ToString("MMyy") + "AdiswaraJson.log", DateTime.Now.ToString() + " : " + sContent);
        }
        public static void writeLogJson(string FileName, string sContent)
        {
            FConvert.LogFile(FileName,  sContent);
        }

        #region Function is used to get Integer Date to DateTime
        public static Date GetIntToDate(int iDate)
        {
            try
            {

                return (new Date(iDate, CalendarType.Gregorean));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //-----------------------------------------------
        #region Create table for RMQty 
        public static void createTableCollect_VendAnnxBatch(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_VendAnnxBatch'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_VendAnnxBatch (id bigint, bid bigint default 0,batchid bigint default 0,batchno varchar(200),iMfgDate bigint default 0,mRate Decimal(20,3) default 0,vDate bigint default 0,   bQty Decimal(20,3) default 0,Cbqty Decimal(20,3) default 0,loggeduser varchar(50) null)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query TableCollect_VendAnnxBatch: " + strQry);

            }
            else
            {
                strQry = $@"delete from TableCollect_VendAnnxBatch where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query TableCollect_VendAnnxBatch: " + strQry);

            }
        }

        public static void createTableCollect_VendAnnxBody(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_VendAnnxBody'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_VendAnnxBody (bid bigint, itemid bigint default 0, Qty Decimal(20,3) default 0,Cqty Decimal(20,3) default 0,loggeduser varchar(50) null)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query TableCollect_VendAnnxBody: " + strQry);

            }
            else
            {
                strQry = $@"delete from TableCollect_VendAnnxBody where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query TableCollect_VendAnnxBody: " + strQry);

            }
        }

        public static void createTableCollect_grpVendAnnxBody(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_grpVendAnnxBody'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_grpVendAnnxBody (id bigint, itemid bigint default 0, Qty Decimal(20,3) default 0,Cqty Decimal(20,3) default 0,loggeduser varchar(50) null)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query TableCollect_grpVendAnnxBody: " + strQry);

            }
            else
            {
                strQry = $@"delete from TableCollect_grpVendAnnxBody where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query TableCollect_grpVendAnnxBody: " + strQry);

            }
        }

        public static void createICSgtEntRfPndngLst(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICSgtEntRfPndngLst'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"CREATE TABLE ICSgtEntRfPndngLst([id] [int] IDENTITY(1,1) Primary key NOT NULL,[VoucherNo] [varchar](100) NULL,[iDate] int NULL,[sName] [varchar](1000) NULL, [AbbrNo] [varchar](150) NULL, [Abbr] [varchar](100) NULL, vno varchar(100), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("Query : " + strQry);
                clsGeneric.writeLog("s1");

            }
            else
            {
                strQry = $@"delete from ICSgtEntRfPndngLst where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query : " + strQry);
                clsGeneric.writeLog("s2");

            }
        }
        public static void createTableStatusBS_GRNJWPur(int companyId, string User, string Vno)
        {
            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='StatusBS_GRNJWPur'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table StatusBS_GRNJWPur([Status] [bit] Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query StatusBS_GRNJWPur: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from StatusBS_GRNJWPur where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query StatusBS_GRNJWPur: " + strQry);
                

            }
        }


        public static void createTableCollect_FGPlan(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_FGPlan'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_FGPlan (id bigint primary key identity(1,1), iVariantId bigint default 0, iProductId bigint default 0, BOMQty Decimal(20,3) default 0,PlanQty Decimal(20,3) default 0,SFGStock Decimal(20,3) default 0,SFGPReq Decimal(20,3) default 0,vSFGStock Decimal(20,3) default 0,salQty Decimal(20,3) default 0,BSFGReq Decimal(20,3) default 0,Q2P Decimal(20,3) default 0,iLevel bigint default 0, ParentProductId bigint default 0,FGID bigint default 0,rowNo bigint default 0,loggeduser varchar(50) null)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query createTableCollect_FGPlan: " + strQry);

            }
            else
            {
                strQry = $@"delete from TableCollect_FGPlan where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query createTableCollect_FGPlan: " + strQry);

            }
        }

        public static void createTable_tblI2PR2P(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='tblI2PR2P'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table tblI2PR2P (iVariantId int, iBomBodyId int, iSize decimal(18,2),iProductId int,fQty decimal(18,2), bInput tinyint,bMainOutPut tinyint,iRowIndex int,iInvTagValue int,I2PPrdnQty decimal(18,2) default 0,R4PQty decimal(18,2) default 0, iRate decimal(18,2) default 0,R4PRate decimal(18,2) default 0, R4PValue decimal(18,2) default 0,R4PGross decimal(18,2) default 0, [UserName] varchar(100),[LoginId] int default 0)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query tblI2PR2P: " + strQry);

            }
            else
            {
                strQry = $@"delete from tblI2PR2P where UserName='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query tblI2PR2P: " + strQry);

            }
        }
        public static void createTableCollect_SFGPlan(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_SFGPlan'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_SFGPlan (id bigint primary key identity(1,1), iProductId bigint default 0,ParentProductId bigint default 0,FGID bigint default 0, BOMQty decimal(20,9) default 0,PlanQty decimal(20,9) default 0,SFGREQ decimal(20,9) default 0,RMReq decimal(20,9) default 0, sUnit varchar(100),loggeduser varchar(100)) ";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query createTableCollect_SFGPlan: " + strQry);

            }
            else
            {
                strQry = $@"delete from TableCollect_SFGPlan where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query createTableCollect_FGPlan: " + strQry);

            }
        }

        public static void createTableCollectSFG_PPCPlan(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollectSFG_PPCPlan'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollectSFG_PPCPlan (id bigint primary key identity(1,1), iVariantId bigint default 0, iProductId bigint default 0, planQty Decimal(20,3) default 0,BOMQty Decimal(20,3) default 0,SFGReqQty Decimal(20,3) default 0,ParentProductId bigint default 0,iLevel bigint default 0, sUnit varchar(100), iRowIndex bigint default 0, iInvTagValue bigint default 0,BranchId bigint default 0,WCId bigint default 0,FGID bigint default 0,loggeduser varchar(50) null)";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query createTableCollectSFG_PPCPlan: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from TableCollectSFG_PPCPlan where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query createTableCollectSFG_PPCPlan: " + strQry);
                

            }
        }

        public static void TableCollect_FGBOMSFG(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollect_FGBOMSFG'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollect_FGBOMSFG (id bigint primary key identity(1,1),Row bigint default 0,ParentId bigint default 0,FgId bigint default 0,VariantId bigint default 0, SfgId bigint default 0, BOMQuantity decimal(18,2)  default 0,PlanQty decimal(18,2)  default 0,Stock decimal(18,2) default 0, SFGPReq decimal(18,2)  default 0,PrvReq decimal(18,2)  default 0,vSTK decimal(18,2) default 0,Q2P decimal(18,2) default 0,level int default 0,iFATag int default 0,iInvtag int default 0,FGPlanNO varchar(100),[User] varchar(200))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query TableCollect_FGBOMSFG: " + strQry);


            }
            else
            {
                strQry = $@"delete from TableCollect_FGBOMSFG where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query TableCollect_FGBOMSFG: " + strQry);


            }
        }


        // ics_abbrTable 
        public static void createTableICS_AbbrTable(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_AbbrTable'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table ICS_AbbrTable(Abbr_Id int identity(1,1) primary key,sAbrr  varchar(20) null,sName  varchar(100) null,sType int default 0, dAbrr  varchar(20) null,dName  varchar(100) null,dType int default 0 )";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query createTableICS_AbbrTable: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // ICS_AbbrMapping 
        public static void createTableICS_AbbrMapping(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_AbbrMapping'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"Create table ICS_AbbrMapping (Id int identity(1,1) primary key,FieldName  varchar(100) null,PostPosition int default 0,PostFieldName varchar(100) null,Abbr_Id int  default 0 FOREIGN KEY REFERENCES ICS_AbbrTable(Abbr_Id),ColMap int default 0)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query createTableICS_AbbrMapping: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // start PPCPlanRMReq
        // I2P 2 RFP
        // ICS_I2P2RFP_HEAD 
        public static void createTableICS_I2P2RFP_HEAD(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_I2P2RFP_HEAD'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table ICS_I2P2RFP_HEAD(Abbr_Id int identity(1,1) primary key,sAbrr  varchar(20) null,sName  varchar(100) null,sType int default 0, dAbrr  varchar(20) null,dName  varchar(100) null,dType int default 0 )";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query ICS_I2P2RFP_HEAD: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // ICS_I2P2RFP_Fields 
        public static void createTableICS_I2P2RFP_Fields(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_I2P2RFP_Fields'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"Create table ICS_I2P2RFP_Fields (Id int identity(1,1) primary key,FieldName  varchar(100) null,PostPosition int default 0,PostFieldName varchar(100) null,Abbr_Id int  default 0 FOREIGN KEY REFERENCES ICS_I2P2RFP_HEAD(Abbr_Id),ColMap int default 0)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query ICS_I2P2RFP_Fields: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // End I2P 2 RFP 
        // start PPCPlan 2 PPSfgReqd
        // PPCPlan 2 PPSfgReqd
        // ICS_PPCPlan2PPSfgReqd_HEAD 

        // ICS_MulLneJWReco_Fields 
        public static void createTableICS_MulLneJWReco(int scompanyId,string  sSession, string sUser, string sVno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_MulLneJWReco'";
            dt = objDB.GetData(strQry, scompanyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                //strQry = $@"Create table ICS_MulLneJWReco(Id int identity(1,1) primary key,FGID bigint default 0,PORate Decimal(18,2), RM_Id int  default 0,RMBOMQty decimal(18,2),RMReqQty decimal(18,2),DCQty decimal(18,2),DCValue decimal(18,2),AnnexQty decimal(18,2),AnnexValue decimal(18,2),RtnAsIsQty decimal(18,2),RtnAsIsValue decimal(18,2),Stock as (DCQty - AnnexQty -RtnAsIsQty),StkValue as (DCValue-AnnexValue-RtnAsIsValue),StkRate decimal(18,2),G2Post decimal(18,2),PrvConstQty decimal(18,2),TotalCons decimal(18,2),loggeduser varchar(10), sVno varchar(10), SessionId varchar(500))";
                //strQry = $@"Create table ICS_MulLneJWReco(Id int identity(1,1) primary key,SACCode__Id bigint default 0,iEquipments__Id bigint default 0,FGID bigint default 0,FGQty Decimal(18,2) default 0,PORate Decimal(18,2) default 0, RM_Id int  default 0,RMBOMQty decimal(18,2),RMReqQty decimal(18,2),Stock Decimal(18,2),StkValue Decimal(18,2),StkRate as (StkValue/Stock),G2Post as ((StkValue/Stock)*RMReqQty),PrvConstQty decimal(18,2) default 0,TotalCons as (RMReqQty+PrvConstQty) ,ScrapQty decimal(10,6) default 0, ScrapID int default 0,loggeduser varchar(10), sVno varchar(10), SessionId varchar(500))";
                strQry = $@"Create table ICS_MulLneJWReco(Id int identity(1,1) primary key,SACCode__Id bigint default 0,iEquipments__Id bigint default 0,FGID bigint default 0,FGQty Decimal(18,2) default 0,PORate Decimal(18,2) default 0, RM_Id int  default 0,RMBOMQty decimal(18,2),RMReqQty decimal(18,2),Stock Decimal(18,2),StkValue Decimal(18,2),StkRate Decimal(18,2),G2Post Decimal(18,2),PrvConstQty decimal(18,2) default 0,TotalCons decimal(18,2),ScrapQty decimal(10,6) default 0, ScrapID int default 0,loggeduser varchar(10), sVno varchar(10), SessionId varchar(500))";
                objDB.GetExecute(strQry, scompanyId, ref error);
                clsGeneric.writeLog("create Query ICS_MulLneJWReco: " + strQry);


            }
            else
            {
                strQry = $@"delete from ICS_MulLneJWReco where loggeduser='" + sUser + "' and SessionId='" + sSession + "' and sVno='" + sVno + "'" ;
                objDB.GetExecute(strQry, scompanyId, ref error);

                clsGeneric.writeLog("delete Query ICS_MulLneJWReco: " + strQry);


            }
        }
        public static void createTableICS_PPCPlan2PPSfgReqd_HEAD(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_PPCPlan2PPSfgReqd_HEAD '";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table ICS_PPCPlan2PPSfgReqd_HEAD(Abbr_Id int identity(1,1) primary key,sAbrr  varchar(20) null,sName  varchar(100) null,sType int default 0, dAbrr  varchar(20) null,dName  varchar(100) null,dType int default 0 )";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query ICS_PPCPlan2PPSfgReqd_HEAD: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // ICS_I2P2RFP_Fields 
        public static void createTableICS_PPCPlan2PPSfgReqd_Fields(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='ICS_PPCPlan2PPSfgReqd_Fields'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"Create table ICS_PPCPlan2PPSfgReqd_Fields (Id int identity(1,1) primary key,FieldName  varchar(100) null,PostPosition int default 0,PostFieldName varchar(100) null,Abbr_Id int  default 0 FOREIGN KEY REFERENCES ICS_PPCPlan2PPSfgReqd_HEAD (Abbr_Id),ColMap int default 0)";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query ICS_PPCPlan2PPSfgReqd_Fields: " + strQry);


            }
            //else
            //{
            //    strQry = $@"delete from ICS_AbbrTable where loggeduser='" + User + "'";
            //    objDB.GetExecute(strQry, companyId, ref error);

            //    clsGeneric.writeLog("delete Query createTableICS_AbbrTable: " + strQry);


            //}
        }
        // End PPCPlan 2 PPSfgReqd 
        public static void createTableCollectSFG_PPCPlanRMReq(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TableCollectSFG_PPCPlanRMReq'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TableCollectSFG_PPCPlanRMReq(id bigint primary key identity(1,1), iVariantId bigint default 0, iProductId bigint default 0, planQty Decimal(20,3) default 0,BOMQty Decimal(20,3) default 0,SFGReqQty Decimal(20,3) default 0,ParentProductId bigint default 0,iLevel bigint default 0, sUnit varchar(100), iRowIndex bigint default 0, iInvTagValue bigint default 0,BranchId bigint default 0,WCId bigint default 0,FGID bigint default 0,vno varchar(50) null,loggeduser varchar(50) null )";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query TableCollectSFG_PPCPlanRMReq: " + strQry);


            }
            else
            {
                strQry = $@"delete from TableCollectSFG_PPCPlanRMReq where loggeduser='" + User + "' and vno='"  + Vno + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query TableCollectSFG_PPCPlanRMReq: " + strQry);


            }
        }
        public static void createTableRMRequisition_PPCPlanRMReq(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMRequisition_PPCPlanRMReq'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@" create table RMRequisition_PPCPlanRMReq(id bigint identity(1,1) primary key,refid bigint,PlanMonth varchar(50), Branch bigint default 0, WorksCenter bigint default 0,Warehouse bigint default 0,Product bigint default 0,ParentID bigint default 0,MParentID bigint default 0,Fqty DECIMAL(14,6) Default 0,RMQty DECIMAL(14,6) Default 0, PlanQty DECIMAL(14,6) Default 0, SalQty DECIMAL(14,6) Default 0, RMStock DECIMAL(14,6) Default 0, PReq DECIMAL(14,6) Default 0,VStock DECIMAL(14,6) Default 0, BRMReq DECIMAL(14,6) Default 0, Q2P DECIMAL(14,6) Default 0,iBodyId bigint default 0,vno varchar(50) null, loggeduser varchar(50) null) ";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query RMRequisition_PPCPlanRMReq: " + strQry);


            }
            else
            {
                strQry = $@"delete from RMRequisition_PPCPlanRMReq where loggeduser='" + User + "' and vno='" + Vno + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMRequisition_PPCPlanRMReq: " + strQry);


            }
        }
        // end of PPCPlanRMReq


        public static void createTableRMReqQty(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMReqQty'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table RMReqQty(ID INT IDENTITY(1, 1) primary key,FgId int,FgQty DECIMAL(14,6) Default 0,GRNJW_FGQty DECIMAL(14,6) Default 0,rmid int, rmqty DECIMAL(14,6) Default 0,ScrapId int,ScrapQty DECIMAL(14,6) Default 0,batchid int, batchname varchar(100), bMfDate int, bfrate DECIMAL(14,6) Default 0 ,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query RMReqQty: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from RMReqQty where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMReqQty: " + strQry);
                

            }
        }
        public static void createTableRMReqQtySAL(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMReqQtySAL'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table RMReqQtySAL(ID INT IDENTITY(1, 1) primary key,FgId int,FgQty DECIMAL(14,6) Default 0,GRNJW_FGQty DECIMAL(14,6) Default 0,rmid int, rmqty DECIMAL(14,6) Default 0,ScrapId int,ScrapQty DECIMAL(14,6) Default 0,batchid int, batchname varchar(100), bMfDate int, bfrate DECIMAL(14,6) Default 0,TaxCode bigint Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query RMReqQtySAL: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from RMReqQtySAL where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMReqQtySAL: " + strQry);
                

            }
        }

        public static void createTableTempRMFG_GRNJWPur(int companyId, string iuser, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TempRMFG_GRNJWPur'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TempRMFG_GRNJWPur (id bigint identity(1,1),iRowId bigint Default 0,FGRowNo bigint Default 0,GrdRowNo bigint Default 0,iBodyId bigint Default 0,RMId bigint Default 0,BoMQty DECIMAL(14,6) Default 0,FgId bigint,FgQty DECIMAL(14,6) Default 0,LinkQty DECIMAL(14,6) Default 0,reqQty DECIMAL(14,6) Default 0,RMMaxQty DECIMAL(14,6) Default 0,FGMaxQty DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("error " + error);
                clsGeneric.writeLog("Create Query TempRMFG_GRNJWPur: " + strQry);


            }
            else
            {
                strQry = $@"delete from TempRMFG_GRNJWPur where loggeduser='" + iuser + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("error " + error);
                clsGeneric.writeLog("delete Query TempRMFG_GRNJWPur: " + strQry);


            }
        }


        public static void createTableTempRMFG_DCJWSal(int companyId, string iuser, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='TempRMFG_DCJWSal'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table TempRMFG_DCJWSal (id bigint identity(1,1),iRowId bigint Default 0,FGRowNo bigint Default 0,GrdRowNo bigint Default 0,iBodyId bigint Default 0,RMId bigint Default 0,BoMQty DECIMAL(14,6) Default 0,FgId bigint,FgQty DECIMAL(14,6) Default 0,LinkQty DECIMAL(14,6) Default 0,reqQty DECIMAL(14,6) Default 0,RMMaxQty DECIMAL(14,6) Default 0,FGMaxQty DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("error " + error);
                clsGeneric.writeLog("Create Query TempRMFG_DCJWSal: " + strQry);


            }
            else
            {
                strQry = $@"delete from TempRMFG_DCJWSal where loggeduser='" + iuser + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("error " + error);
                clsGeneric.writeLog("delete Query TempRMFG_DCJWSal: " + strQry);


            }
        }


        public static void createTableFgMaxBS_GRNJWPur(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='FgMaxBS_GRNJWPur'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table FgMaxBS_GRNJWPur (FgId bigint,FgMaxQty DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("Create Query FgMaxBS_GRNJWPur: " + strQry);


            }
            else
            {
                strQry = $@"delete from FgMaxBS_GRNJWPur where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query FgMaxBS_GRNJWPur: " + strQry);


            }
        }

        public static void createTableRMReqQtyBS_GRNJWPur(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMReqQtyBS_GRNJWPur'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table RMReqQtyBS_GRNJWPur (RMId bigint,RMReqQty DECIMAL(14,6) Default 0,FgId bigint,FgQty DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("Create Query RMReqQtyBS_GRNJWPur: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from RMReqQtyBS_GRNJWPur where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMReqQtyBS_GRNJWPur: " + strQry);
                

            }
        }
        public static void createTableRMLinkQtyBS_GRNJWPur(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMLinkQtyBS_GRNJWPur'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table RMLinkQtyBS_GRNJWPur (RMId bigint primary Key,LinkQty DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query RMLinkQtyBS_GRNJWPur: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from RMLinkQtyBS_GRNJWPur where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMLinkQtyBS_GRNJWPur: " + strQry);
                

            }
        }

        public static void createTableRMRequisition(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='RMRequisition'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@" create table RMRequisition (id bigint identity(1,1) primary key,PlanMonth varchar(50), Branch bigint default 0, WorksCenter bigint default 0,Warehouse bigint default 0,Product bigint default 0,ParentID bigint default 0,MParentID bigint default 0,Fqty DECIMAL(14,6) Default 0,RMQty DECIMAL(14,6) Default 0, PlanQty DECIMAL(14,6) Default 0, SalQty DECIMAL(14,6) Default 0, RMStock DECIMAL(14,6) Default 0, PReq DECIMAL(14,6) Default 0,VStock DECIMAL(14,6) Default 0, BRMReq DECIMAL(14,6) Default 0, Q2P DECIMAL(14,6) Default 0,iBodyId bigint default 0,vno varchar(50), loggeduser varchar(50)) ";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query RMRequisition: " + strQry);


            }
            else
            {
                strQry = $@"delete from RMRequisition where loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("delete Query RMRequisition: " + strQry);


            }
        }
        #endregion

        //-----------------------------------------------
        #region IsnullEmpty
        public static bool TstNullOrEmpty(string s)
        {
           
            //Thread.Sleep(TimeSpan.FromMilliseconds(2));
            //result = s == null || s == string.Empty;
            if (s.Equals(null) || s.Length > 0 )
            {
                return false;
            }
            else
            {
                return true;
            }
           // Thread.Sleep(TimeSpan.FromMilliseconds(2));
            
        }
        #endregion

        #region ShowRecord 
        public static string ShowRecord(int companyId, string strquery)
        {
            BL_DB objDB = new BL_DB();
            string error = "";
            DataSet dt0 = objDB.GetData(strquery, companyId, ref error);
            if (dt0 != null  && dt0.Tables.Count > 0 && dt0.Tables[0].Rows.Count > 0)
            {
                bool b1 = string.IsNullOrEmpty(dt0.Tables[0].Rows[0][0].ToString());
                if (!b1)
                    return dt0.Tables[0].Rows[0][0].ToString();
            }
            return "0";
        }

        public static void Log_write (string fControllerName, string fMethodName, string fSessionId, int fCompanyId, string fdocNo, int fvtype, string fUser)
        {
            clsGeneric.writeLog(" ------------------ " + fControllerName +  " starts here {" + fUser + "}-------------------");
            clsGeneric.writeLog("fSessionid : " + fSessionId);
            clsGeneric.writeLog("Company Id : " + fCompanyId);
            clsGeneric.writeLog("Doc No : " + fdocNo);
            clsGeneric.writeLog("v type : " + fvtype);
            clsGeneric.writeLog("Controller Name :" + fControllerName  );
            clsGeneric.writeLog("Method Name:" + fMethodName);
        }

        #endregion
        #region Update AnnexureStatus 

        public static void Update_AnnexureStatus(int companyId, int vtype, int HeaderId, int Status = 0)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = $@"update tCore_HeaderData" + vtype + "_0 set AnnexureStatus = " + Status + " where iHeaderId=" + HeaderId;
            objDB.GetExecute(strQry, companyId, ref error);

            clsGeneric.writeLog("Update AnnexureStatus - tCore_HeaderData" + vtype + "_0 Query : " + strQry);

        }
        public static void Update_AnnexureStatusSAL(int companyId, int vtype, int HeaderId, int Status = 0)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = $@"update tCore_HeaderData" + vtype + "_0 set AnnexureStatus = " + Status + " where iHeaderId=" + HeaderId;
            objDB.GetExecute(strQry, companyId, ref error);

            clsGeneric.writeLog("Update AnnexureStatus - tCore_HeaderData" + vtype + "_0 Query : " + strQry);

        }

        public static void ReSaveVoucher(string sSessionId, int Vouchertype, string VoucherNo)
        {
            BL_DB objDB = new BL_DB();
            
            string strVal = string.Empty;
            
            strVal = "http://localhost/focus8api/Screen/Transactions/resavevoucher/" + Vouchertype + "/" + VoucherNo;
            clsGeneric.writeLog("URL OF ReSave :-" + strVal);
            using (var client = new WebClient())
            {

                //client.Encoding = Encoding.UTF8;
                client.Headers.Add("fSessionId", sSessionId);
                client.Headers.Add("Content-Type", "application/json");
                var response = client.DownloadString(strVal);
                clsGeneric.writeLog("Response of Resave :- " + response);

            }
        }

        #endregion

        #region Create table for linkinfo 

        public static void createTablelinkinfo(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='linkinfo'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table linkinfo (liid int primary Key identity(1,1), iHeaderId int, iDate bigint, sVoucherNo varchar(100), Balance DECIMAL(14,6) Default 0, iTransactionId int , iProduct int, mrate DECIMAL(14,6) Default 0, iRefId int, iInvTag int, iBookNo int, VendorWarehouse int, iVoucherType int, ConsumedQty DECIMAL(14,6) Default 0,BatchId int,BatchNo varchar(100) ,BMfDate int,Brate DECIMAL(14,6) Default 0,Process int,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query linkinfo: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from linkinfo where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query linkinfo: " + strQry);
                

            }
        }
        public static void createTablelinkinfoSAL(int companyId, string User, string Vno)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='linkinfoSAL'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table linkinfoSAL (liid int primary Key identity(1,1), iHeaderId int Default 0, iDate bigint Default 0, sVoucherNo varchar(100), Balance DECIMAL(14,6) Default 0, iTransactionId int Default 0, iProduct int Default 0, mrate DECIMAL(14,6) Default 0, iRefId int Default 0, iInvTag int Default 0, iBookNo int Default 0, VendorWarehouse int, iVoucherType int, ConsumedQty DECIMAL(14,6) Default 0,BatchId int Default 0,BatchNo varchar(100) ,BMfDate int,Brate DECIMAL(14,6) Default 0,Process int default 0, WorksCenter int default 0,Dept int default 0, TaxCode int default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("create Query linkinfoSAL : " + strQry);
                

            }
            else
            {
                strQry = $@"delete from linkinfoSAL where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query linkinfoSAL : " + strQry);
                

            }
        }
        #endregion

        #region Create table for linkrmused 

        public static void createTablelinkrmused(int companyId, string User, string Vno)
        {

            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='linkrmused'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table linkrmused(liid int, UsedQty DECIMAL(14,6) Default 0, RunningLDocQty DECIMAL(14,6) Default 0,fgid int, fgqty DECIMAL(14,6) Default 0,fgrate DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query linkrmused : " + strQry);
                

            }
            else
            {
                strQry = $@"delete from linkrmused where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query linkrmused: " + strQry);
                

            }
        }
        public static void createTablelinkrmusedSAL(int companyId, string User, string Vno)
        {

            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='linkrmusedSAL'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"create table linkrmusedSAL(liid int, UsedQty DECIMAL(14,6) Default 0, RunningLDocQty DECIMAL(14,6) Default 0,fgid int, fgqty DECIMAL(14,6) Default 0,fgrate DECIMAL(14,6) Default 0,vno varchar(50), loggeduser varchar(50))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("create Query linkrmusedSAL: " + strQry);
                

            }
            else
            {
                strQry = $@"delete from linkrmusedSAL where vno='" + Vno + "' and loggeduser='" + User + "'";
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("delete Query linkrmusedSAL: " + strQry);
                

            }
        }
        #endregion


        //-----------------------------------------------



        #region Create Function for Posting Status

        public static void createTablePost(int companyId)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='EX_Posting_Status'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                // strQry = $@"Create Table EX_Posting_Status(PostingDate integer,PostDocType varchar(100),PostDocNo varchar(100),
                //     Status varchar(100),Reason varchar(500))";


                strQry = $@"Create Table EX_Posting_Status(BaseDocType varchar(100),BaseDocNo varchar(100),BaseDocDate integer,PostingDate integer,PostDocType varchar(100),
                                 PostDocNo varchar(100),  Status varchar(100),Reason varchar(500))";
                objDB.GetExecute(strQry, companyId, ref error);

                clsGeneric.writeLog("Query : " + strQry);
                clsGeneric.writeLog("s1");

            }
        }
        #endregion
        #region Delete Function for tmp table
        public static void deleteTempTable(string tableName, int companyId)
        {


            BL_DB objDB = new BL_DB();
            string strQry = "";
            string error = "";
            DataTable dt = new DataTable();
            strQry = "select * from sysObjects where name='" + tableName + "'";
            dt = objDB.GetData(strQry, companyId, ref error).Tables[0];
            if (dt.Rows.Count <= 0)
            {

                strQry = $@"drop Table" + tableName;
                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("Delete Temp Table starts");
                clsGeneric.writeLog("Delete Temp table Query : " + strQry);
                clsGeneric.writeLog("Delete Table Creation ends");

            }
            else
            {
                clsGeneric.writeLog("No Temp Table Found");
            }
        }
        #endregion
        #region Create Function for tmp table
        public static void createTempTable(string strQry, int companyId)
        {


            BL_DB objDB = new BL_DB();
            string error = "";
            try
            {

                objDB.GetExecute(strQry, companyId, ref error);
                clsGeneric.writeLog("Temp Table starts");
                clsGeneric.writeLog("Temp table Query : " + strQry);
                clsGeneric.writeLog("Temp Table Creation ends");
            }
            catch (Exception ex)
            {
                clsGeneric.writeLog("Temp table creation exception occured:" + (ex.Message));

            }


        }
        #endregion

        #region Function is used to get Date to Integer
        public static int GetDateToInt(DateTime dt)
        {
            int val;
            val = Convert.ToInt16(dt.Year) * 65536 + Convert.ToInt16(dt.Month) * 256 + Convert.ToInt16(dt.Day);
            return val;
        }
        #endregion

        #region Get Desired default Value of Master
        public static string getValue(string sTableName, int Companyid, string strFldName, string fldValue, string returnValue)
        {
            BL_DB objDb = new BL_DB();
            string strErrorMessage = default(string);
            //int fieldid = default(int);
            string sqlsticks = string.Empty;

            sqlsticks = "select sName from cCore_Vouchers_0 where ivoucherType = " + fldValue + "";
            DataSet dsFld = new DataSet();
            dsFld = objDb.GetData(sqlsticks, Companyid, ref strErrorMessage);

            clsGeneric.writeLog("Query : " + sqlsticks);

            if (dsFld.Tables[0].Rows.Count <= 0)
                return "";
            return dsFld.Tables[0].Rows[0][0].ToString();

        }
        #endregion



    }
}