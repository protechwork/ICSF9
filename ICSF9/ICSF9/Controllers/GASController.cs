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

namespace ICSF9.Controllers
{
    public class GASController : Controller
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
        public ActionResult GenerilizedMasterAfterSave(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {

            //clsGeneric.ReSaveVoucher(SessionId, vtype, docNo);
            //return Json(new { status = false, data = new { message = "Document is Resaved please check." } });

            if (StartUpdate(vtype, docNo, CompanyId, SessionId) > 0 )
            {
                return Json(new { status = false, data = new { message = "Data is updated" } });
            }

            try
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
                  + "IF OBJECT_ID(N'dbo.ICS_MasterMap', N'U') IS NULL "
                  + "begin"
                  + "    CREATE TABLE[dbo].[ICS_MasterMap]( "
                  + "		[id] "
                  + "        [int] IDENTITY(1,1) NOT NULL, "
                  + "        [source] [nvarchar](max) NULL, "
                  + "		 [destination] [nvarchar](max) NULL, "
                  + "		 [type] [int] NULL, "
                  + "        [abbr] [nvarchar](max) NULL, "
                  + "	) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY] "
                  + " "
                  + "    INSERT INTO ICS_MasterMap(source, destination, type, abbr) VALUES('M2GName','sName', 0, 'Core__Tool') "
                  + "    INSERT INTO ICS_MasterMap(source, destination, type, abbr) VALUES('M2GCode','sCode', 0, 'Core__Tool') "
                  + "    INSERT INTO ICS_MasterMap(source, destination, type, abbr) VALUES('M2CName','sName', 1, 'Core__Tool') "
                  + "    INSERT INTO ICS_MasterMap(source, destination, type, abbr) VALUES('M2CCode','sCode', 1, 'Core__Tool') "
                  + "    INSERT INTO ICS_MasterMap(source, destination, type, abbr) VALUES('MainParent','ParentId', 1, 'Core__Tool') "
                  //+ "    INSERT INTO ICS_MasterMap(source, destination, type) VALUES('','iDefaultBaseUnit__Id', 0) "
                  //+ "    INSERT INTO ICS_MasterMap(source, destination, type) VALUES('','ParentId', 0) "
                  + "end";


                int ret = focus_db.GetExecute(create_if_not_exist, CompanyId, ref strErrorMessage);

                if (ret > 0)
                {
                    clsGeneric.writeLog("ICS_MasterMap Table Not Found Table Is Created...");

                    clsGeneric.writeLog("ICS_MasterMap Table Inserting data" + create_if_not_exist);
                    return Json(new { status = false, data = new { message = "Table Not Created Table is created Please Enter Mapping." } });
                }


                DataSet dynamicMasterDS;
                string masterName = "";
                string parentId = "0";
                Boolean makeParent = true;
                string m2GName = "";
                string m2GCode = "";

                var extHeader = new Hashtable();
                var extBody = new List<System.Collections.Hashtable>();
                var extFooter = new List<System.Collections.Hashtable>();

                using (var clt = new WebClient())
                {
                    clt.Headers.Add("fSessionId", SessionId);
                    clt.Headers.Add("Content-Type", "application/json");
                    clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                    var responseCENT = clt.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                    clsGeneric.writeLog("response CENT: " + responseCENT);

                    if (responseCENT != null)
                    {
                        var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCENT);
                        if (DataResponse.result == -1)
                        {
                            return Json(new { status = false, data = new { message = DataResponse.message } });
                        }
                        else
                        {
                            extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(DataResponse.data[0]["Header"]));
                            extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(DataResponse.data[0]["Body"]));

                            for (int iBody = 0; iBody < extBody.Count; iBody++)
                            {
                                foreach (DictionaryEntry entry in extHeader)
                                {
                                    extBody[iBody][entry.Key] = entry.Value;
                                    //Console.WriteLine("{0}, {1}", entry.Key, entry.Value);
                                }
                            }

                            if (DataResponse.data[0]["Footer"].ToString() != "[]")
                            {
                                extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(DataResponse.data[0]["Footer"]));
                            }
                        }
                    }
                }

                // If not approved authorize then exit
                bool iApproved = Convert.ToBoolean(JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(extHeader["Flags"]))["Approved"]);
                if (!iApproved)
                {
                    return Json(new { status = true, data = new { message = "Source Document is not Authorized" } });
                }


                /*string dynamicMasterHeaderQuery = " SELECT CH.sVoucherNo,CH.iDate, CHD.* FROM  tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN  " +
                " dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE(CH.iVoucherType = " + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
                clsGeneric.writeLog("Executing Query:" + dynamicMasterHeaderQuery);
                dynamicMasterDS = focus_db.GetData(dynamicMasterHeaderQuery, CompanyId, ref strErrorMessage);

                if (dynamicMasterDS != null)
                {
                    for (int iDynamicMaster = 0; iDynamicMaster < dynamicMasterDS.Tables[0].Rows.Count; iDynamicMaster++)
                    {
                    */
                masterName = Convert.ToString(extHeader["MasterName"]);
                m2GName = Convert.ToString(extHeader["M2GName"]);
                m2GCode = Convert.ToString(extHeader["M2GCode"]);

                if (m2GName.Equals(""))
                {
                    m2GName = docNo;
                    m2GCode = docNo;
                }

                string dynamicMasterMapIsParentQuery = "SELECT * FROM ICS_MasterMap WHERE source='MainParent' AND type=1 AND abbr='" + masterName + "'";
                DataSet dynamicMasterMapIsParentDS = focus_db.GetData(dynamicMasterMapIsParentQuery, CompanyId, ref strErrorMessage);
                if (dynamicMasterMapIsParentDS != null)
                {
                    for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterMapIsParentDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                    {
                        string _sourceInDynamicMap = Convert.ToString(dynamicMasterMapIsParentDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                        string _destinyInDynamicMap = Convert.ToString(dynamicMasterMapIsParentDS.Tables[0].Rows[iDynamicMasterMap]["destination"]);
                        if (_destinyInDynamicMap.Equals(""))
                        {
                            makeParent = false;
                            continue;
                        }
                    }
                }



                string dynamicMasterMapQuery = "SELECT * FROM ICS_MasterMap WHERE type=0 AND abbr='" + masterName + "'";
                DataSet dynamicMasterMapDS = focus_db.GetData(dynamicMasterMapQuery, CompanyId, ref strErrorMessage);
                if (dynamicMasterMapDS != null)
                {
                    for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterMapDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                    {
                        string _sourceInDynamicMap = Convert.ToString(dynamicMasterMapDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                        string _destinyInDynamicMap = Convert.ToString(dynamicMasterMapDS.Tables[0].Rows[iDynamicMasterMap]["destination"]);

                        string valInSourceDynamicMaster = Convert.ToString(extHeader[_sourceInDynamicMap]);
                        if (_sourceInDynamicMap.Equals("M2GName") || _sourceInDynamicMap.Equals("M2GCode"))
                        {
                            if (valInSourceDynamicMaster.Equals(""))
                            {
                                valInSourceDynamicMaster = docNo;
                            }
                        }

                        headerCBROD.Add(_destinyInDynamicMap, valInSourceDynamicMaster);
                    }
                    headerCBROD.Add("IsGroup", true);
                }
                //}

                /* Add Name by Name Logic In Header */
                if (!Convert.ToString(extHeader["M2GNameLogic"]).Equals(""))
                {
                    char[] spearator = { ',' };
                    String[] strlist = Convert.ToString(extHeader["M2GNameLogic"]).Split(spearator);
                    string nameWithLogic = "";

                    foreach (String fieldName in strlist)
                    {
                        if (fieldName.Equals("-"))
                        {
                            nameWithLogic = nameWithLogic + "-";
                        }
                        else if (fieldName.Equals("/"))
                        {
                            nameWithLogic = nameWithLogic + "/";
                        }
                        else if (fieldName.Equals(" "))
                        {
                            nameWithLogic = nameWithLogic + " ";
                        }
                        else
                        {
                            nameWithLogic = nameWithLogic + Convert.ToString(extHeader[fieldName]);
                        }
                    }

                    headerCBROD.Remove("sName");
                    headerCBROD.Add("sName", nameWithLogic);
                }

                /* Add Name by Name Logic In Header */
                if (!Convert.ToString(extHeader["M2GCodeLogic"]).Equals(""))
                {
                    char[] spearator = { ',' };
                    String[] strlist = Convert.ToString(extHeader["M2GCodeLogic"]).Split(spearator);
                    string codeWithLogic = "";

                    foreach (String fieldName in strlist)
                    {
                        if (fieldName.Equals("-"))
                        {
                            codeWithLogic = codeWithLogic + "-";
                        }
                        else if (fieldName.Equals("/"))
                        {
                            codeWithLogic = codeWithLogic + "/";
                        }
                        else if (fieldName.Equals(" "))
                        {
                            codeWithLogic = codeWithLogic + " ";
                        }
                        else
                        {
                            codeWithLogic = codeWithLogic + Convert.ToString(extHeader[fieldName]);
                        }
                    }

                    headerCBROD.Remove("sCode");
                    headerCBROD.Add("sCode", codeWithLogic);
                }





                // Calling API for Header Data Parent Code Start
                if (makeParent)
                {
                    HashData objHashRequest = new HashData();

                    List<Hashtable> lstHash = new List<Hashtable>();
                    lstHash.Add(headerCBROD);
                    objHashRequest.data = lstHash;
                    string sContentCustVendCr = JsonConvert.SerializeObject(objHashRequest);
                    clsGeneric.writeLog("Upload URL :- " + "http://localhost/Focus8API/Masters/" + masterName);
                    clsGeneric.writeLog("Content to upload " + masterName + " Master :- " + sContentCustVendCr);
                    using (var clt = new WebClient())
                    {
                        clt.Headers.Add("fSessionId", SessionId);
                        clt.Headers.Add("Content-Type", "application/json");
                        var strResponse = clt.UploadString("http://localhost/Focus8API/Masters/" + masterName, sContentCustVendCr);
                        clsGeneric.writeLog("Response " + masterName + " Master :- " + strResponse);
                        if (strResponse != null)
                        {
                            var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                            if (DataResponse.result == -1)
                            {
                                //UpdateStatus(vtype, docNo, 0, CompanyId);

                                if (DataResponse.message.Equals(" , Name Is Unique "))
                                {
                                    //UpdateByQuery(headerCBROD["sName"].ToString(), headerCBROD["sCode"].ToString(), masterName, CompanyId);
                                    StartUpdate(vtype, docNo, CompanyId, SessionId);
                                }




                                return Json(new { status = false, data = new { message = DataResponse.message } });
                            }
                            else
                            {
                                var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(DataResponse.data[0]["MasterId"]));
                                parentId = iMasterId.ToString();
                                //UpdateStatus(vtype, docNo, 1, Convert.ToInt32(iMasterId), sCode, CompanyId);
                                //UpdateStatus(vtype, docNo, 1, CompanyId);
                                //return Json(new { status = true, data = new { message = "Posting Successful" } });
                            }
                        }
                    }
                }

                // Calling API for Header Data Parent Code End
                //}

                string dynamicMasterBodyQuery = "SELECT h.*, b.*, bextra.*, ch.*, " + parentId + "[MainParent] FROM dbo.tCore_Data_0 AS hbl "
                + "INNER JOIN tCore_Data" + vtype + "_0 AS  b ON hbl.iBodyId = b.iBodyId "
                + "INNER JOIN tCore_HeaderData" + vtype + "_0 AS h ON hbl.iHeaderId=h.iHeaderId "
                + "INNER JOIN tCore_Header_0 AS ch ON hbl.iHeaderId=ch.iHeaderId "
                + "INNER JOIN tCore_Indta_0 AS bextra ON hbl.iBodyId = bextra.iBodyId "
                + "INNER JOIN tCore_Data_Tags_0 as emast ON hbl.iBodyId = emast.iBodyId "
                + "WHERE (ch.sVoucherNo = '" + docNo + "') AND (ch.iVoucherType = " + vtype + ") ";


                dynamicMasterDS = focus_db.GetData(dynamicMasterBodyQuery, CompanyId, ref strErrorMessage);

                if (dynamicMasterDS != null)
                {
                    for (int iDynamicMaster = 0; iDynamicMaster < dynamicMasterDS.Tables[0].Rows.Count; iDynamicMaster++)
                    {
                        headerCBROD = new Hashtable();
                        string iBodyId = Convert.ToString(dynamicMasterDS.Tables[0].Rows[iDynamicMaster]["iBodyId"]);
                        string dynamicMasterBodyMapQuery = "SELECT * FROM ICS_MasterMap WHERE type=1 AND abbr='" + masterName + "'";
                        DataSet dynamicMasterBodyMapDS = focus_db.GetData(dynamicMasterBodyMapQuery, CompanyId, ref strErrorMessage);
                        if (dynamicMasterBodyMapDS != null)
                        {
                            for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterBodyMapDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                            {
                                string _sourceInDynamicMap = Convert.ToString(dynamicMasterBodyMapDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                                string _destinyInDynamicMap = Convert.ToString(dynamicMasterBodyMapDS.Tables[0].Rows[iDynamicMasterMap]["destination"]);

                                if (_destinyInDynamicMap.Equals(""))
                                {
                                    continue;
                                }

                                //string valInSourceDynamicMaster = Convert.ToString(dynamicMasterDS.Tables[0].Rows[iDynamicMaster][_sourceInDynamicMap]);
                                string valInSourceDynamicMaster = Convert.ToString(extBody[iDynamicMaster][_sourceInDynamicMap]);


                                //if(!parentId.Equals(""))
                                //{
                                // if parent not blank then make name and code according to parent
                                if (_sourceInDynamicMap.Equals("M2CName"))
                                {
                                    if (valInSourceDynamicMaster.Equals(""))
                                    {
                                        valInSourceDynamicMaster = docNo + "-" + (iDynamicMaster + 1).ToString();
                                    }
                                    //valInSourceDynamicMaster = m2GName + "-" + (iDynamicMaster + 1).ToString();
                                }
                                if (_sourceInDynamicMap.Equals("M2CCode"))
                                {
                                    if (valInSourceDynamicMaster.Equals(""))
                                    {
                                        valInSourceDynamicMaster = docNo + "-" + (iDynamicMaster + 1).ToString();
                                    }
                                    //valInSourceDynamicMaster = m2GCode + "-" + (iDynamicMaster + 1).ToString();
                                }
                                //}
                                headerCBROD.Add(_destinyInDynamicMap, valInSourceDynamicMaster);
                            }

                            // For Creating By Group
                            if (makeParent)
                            {
                                headerCBROD.Remove("ParentId");
                                headerCBROD.Add("ParentId", parentId);
                            }

                            /* Add Name by Name Logic In Body */
                            if (!Convert.ToString(extBody[iDynamicMaster]["M2CNameLogic"]).Equals(""))
                            {
                                char[] spearator = { ',' };
                                String[] strlist = Convert.ToString(extBody[iDynamicMaster]["M2CNameLogic"]).Split(spearator);
                                string nameWithLogic = "";

                                foreach (String fieldName in strlist)
                                {
                                    if (fieldName.Equals("-"))
                                    {
                                        nameWithLogic = nameWithLogic + "-";
                                    }
                                    else if (fieldName.Equals("/"))
                                    {
                                        nameWithLogic = nameWithLogic + "/";
                                    }
                                    else if (fieldName.Equals(" "))
                                    {
                                        nameWithLogic = nameWithLogic + " ";
                                    }
                                    else if (fieldName.Equals("#"))
                                    {
                                        nameWithLogic = nameWithLogic + (iDynamicMaster + 1).ToString();
                                    }
                                    else
                                    {
                                        nameWithLogic = nameWithLogic + Convert.ToString(extBody[iDynamicMaster][fieldName]);
                                    }
                                }

                                headerCBROD.Remove("sName");
                                headerCBROD.Add("sName", nameWithLogic);
                            }

                            /* Add Code by Code Logic In Body */
                            if (!Convert.ToString(extBody[iDynamicMaster]["M2CCodeLogic"]).Equals(""))
                            {
                                char[] spearator = { ',' };
                                String[] strlist = Convert.ToString(extBody[iDynamicMaster]["M2CCodeLogic"]).Split(spearator);
                                string codeWithLogic = "";

                                foreach (String fieldName in strlist)
                                {
                                    if (fieldName.Equals("-"))
                                    {
                                        codeWithLogic = codeWithLogic + "-";
                                    }
                                    else if (fieldName.Equals("/"))
                                    {
                                        codeWithLogic = codeWithLogic + "/";
                                    }
                                    else if (fieldName.Equals(" "))
                                    {
                                        codeWithLogic = codeWithLogic + " ";
                                    }
                                    else if (fieldName.Equals("#"))
                                    {
                                        codeWithLogic = codeWithLogic + (iDynamicMaster + 1).ToString();
                                    }
                                    else
                                    {
                                        codeWithLogic = codeWithLogic + Convert.ToString(extBody[iDynamicMaster][fieldName]);
                                    }
                                }

                                headerCBROD.Remove("sCode");
                                headerCBROD.Add("sCode", codeWithLogic);
                            }




                            /* string postStatusQuery = $@"SELECT CHD.PostingStatus FROM  dbo.tCore_HeaderData" + vtype + "_0 AS CHD INNER JOIN " +
                                              "dbo.tCore_Header_0 AS CH ON CHD.iHeaderId = CH.iHeaderId WHERE (CH.iVoucherType =" + vtype + ") AND (CH.sVoucherNo = N'" + docNo + "')";
                             vStatus = Convert.ToInt32(clsGeneric.ShowRecord(CompanyId, postStatusQuery));

                             if (vStatus == 0)
                             {*/
                            HashData objHashRequest = new HashData();

                            List<Hashtable> lstHash = new List<Hashtable>();
                            lstHash.Add(headerCBROD);
                            objHashRequest.data = lstHash;
                            string sContentCustVendCr = JsonConvert.SerializeObject(objHashRequest);
                            clsGeneric.writeLog("Upload URL :- " + "http://localhost/Focus8API/Masters/" + masterName);
                            clsGeneric.writeLog("Content to upload " + masterName + " Master :- " + sContentCustVendCr);
                            using (var clt = new WebClient())
                            {
                                clt.Headers.Add("fSessionId", SessionId);
                                clt.Headers.Add("Content-Type", "application/json");
                                var strResponse = clt.UploadString("http://localhost/Focus8API/Masters/" + masterName, sContentCustVendCr);
                                clsGeneric.writeLog("Response " + masterName + " Master :- " + strResponse);
                                if (strResponse != null)
                                {
                                    var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                                    if (DataResponse.result == -1)
                                    {
                                        //UpdateStatus(vtype, docNo, 0, CompanyId);

                                        return Json(new { status = false, data = new { message = DataResponse.message } });
                                    }
                                    else
                                    {
                                        var iMasterId = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(DataResponse.data[0]["MasterId"]));


                                        string dynamicMasterBodyImasterMapQuery = "SELECT * FROM ICS_MasterMap WHERE type=2 AND abbr='" + masterName + "'";
                                        string tagToUpdate = "";
                                        DataSet dynamicMasterBodyImasterMapDS = focus_db.GetData(dynamicMasterBodyImasterMapQuery, CompanyId, ref strErrorMessage);
                                        if (dynamicMasterBodyImasterMapDS != null)
                                        {
                                            for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterBodyImasterMapDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                                            {
                                                tagToUpdate = Convert.ToString(dynamicMasterBodyImasterMapDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                                            }
                                        }


                                        // Update Master in tool in dynamic master document In tCore_Data_Tags_0 Table
                                        if (!tagToUpdate.Equals(""))
                                        {
                                            string updateQuery = "Update tCore_Data_Tags_0 SET " + tagToUpdate + "=" + iMasterId + " WHERE iBodyId=" + iBodyId;
                                            ret = focus_db.GetExecute(updateQuery, CompanyId, ref strErrorMessage);
                                            if (ret > 0)
                                            {
                                                clsGeneric.writeLog("Query Execeuted:" + updateQuery);
                                                clsGeneric.ReSaveVoucher(SessionId, vtype, docNo);
                                            }
                                        }
                                        else
                                        {
                                            clsGeneric.writeLog(docNo + "tagToUpdate is Blank Find It By Hint Query:" + "SELECT * FROM tCore_Data_Tags_0  WHERE iBodyId=" + iBodyId);
                                        }



                                        //UpdateStatus(vtype, docNo, 1, Convert.ToInt32(iMasterId), sCode, CompanyId);
                                        //UpdateStatus(vtype, docNo, 1, CompanyId);
                                        //return Json(new { status = true, data = new { message = "Posting Successful" } });
                                    }
                                }
                            }
                            //}
                            //End Calling  API
                        }
                    }
                }
                return Json(new { status = true, data = new { message = "Posting Successful" } });


            }
            catch (Exception ex)
            {
                /*System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                for (int iError = 0; iError < trace.FrameCount; iError++)
                {
                    clsGeneric.writeLog("Line: " + trace.GetFrame(iError).GetFileLineNumber());
                }*/
                clsGeneric.writeLog("Exception Occure:" + ex.Message);

                return Json(new { status = true, data = new { message = "Exception Occure Please Check Log File..." } });
            }

        }


        static int StartUpdate(int vtype, string docNo, int CompanyId, string SessionId)
        {
            var extHeader = new Hashtable();
            var extBody = new List<System.Collections.Hashtable>();
            var extFooter = new List<System.Collections.Hashtable>();

            using (var clt = new WebClient())
            {
                clt.Headers.Add("fSessionId", SessionId);
                clt.Headers.Add("Content-Type", "application/json");
                clsGeneric.writeLog("Download CENT URL: " + "http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                var responseCENT = clt.DownloadString("http://localhost/Focus8API/Screen/Transactions/" + vtype + "/" + docNo);
                clsGeneric.writeLog("response CENT: " + responseCENT);

                if (responseCENT != null)
                {
                    var DataResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCENT);
                    if (DataResponse.result == -1)
                    {
                        //return Json(new { status = false, data = new { message = DataResponse.message } });
                    }
                    else
                    {
                        extHeader = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(DataResponse.data[0]["Header"]));
                        extBody = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(DataResponse.data[0]["Body"]));

                        for (int iBody = 0; iBody < extBody.Count; iBody++)
                        {
                            foreach (DictionaryEntry entry in extHeader)
                            {
                                extBody[iBody][entry.Key] = entry.Value;
                            }
                        }

                        if (DataResponse.data[0]["Footer"].ToString() != "[]")
                        {
                            extFooter = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(DataResponse.data[0]["Footer"]));
                        }
                    }
                }
            }


            string masterName = Convert.ToString(extHeader["MasterName"]);
            string iMasterIdKey = "";
            //string updatedBodyTagName = "";


            BL_DB focus_db = new BL_DB();
            string dynamicMasterBodyImasterMapQuery = "SELECT * FROM ICS_MasterMap WHERE type=2 AND abbr='" + masterName + "'";
            
            string strErrorMessage = "";
            DataSet dynamicMasterBodyImasterMapDS = focus_db.GetData(dynamicMasterBodyImasterMapQuery, CompanyId, ref strErrorMessage);
            if (dynamicMasterBodyImasterMapDS != null)
            {
                for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterBodyImasterMapDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                {
                    //updatedBodyTagName = Convert.ToString(dynamicMasterBodyImasterMapDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                    iMasterIdKey = Convert.ToString(dynamicMasterBodyImasterMapDS.Tables[0].Rows[iDynamicMasterMap]["destination"]);
                }
            }

            // Convert To Table Name
            string tableName = "muCore_" + (masterName.Split(new string[] { "__" }, StringSplitOptions.None)[1]);

            int hasImasterData = UpdateByQuery(extHeader, extBody, tableName, iMasterIdKey, CompanyId);
            return hasImasterData;
        }

        static int UpdateByQuery(Hashtable HeaderData, List<System.Collections.Hashtable> BodyData, string TableName, string iMasterIdAPIKey, int CompanyId)
        {
            BL_DB focus_db = new BL_DB();
            string dynamicMasterMapIsParentQuery = "SELECT * FROM ICS_MasterMap WHERE type=3 AND abbr='" + TableName + "'";
            string errorContainer = "";
            DataSet dynamicMasterMapIsParentDS = focus_db.GetData(dynamicMasterMapIsParentQuery, CompanyId, ref errorContainer);
            int retValue = 0;
            for (int iBody = 0; iBody < BodyData.Count; iBody++)
            {
                if (dynamicMasterMapIsParentDS != null)
                {
                    for (int iDynamicMasterMap = 0; iDynamicMasterMap < dynamicMasterMapIsParentDS.Tables[0].Rows.Count; iDynamicMasterMap++)
                    {
                        string _sourceInMap = Convert.ToString(dynamicMasterMapIsParentDS.Tables[0].Rows[iDynamicMasterMap]["source"]);
                        string _destinyInMap = Convert.ToString(dynamicMasterMapIsParentDS.Tables[0].Rows[iDynamicMasterMap]["destination"]);

                        string updateTableName = Convert.ToString(dynamicMasterMapIsParentDS.Tables[0].Rows[iDynamicMasterMap]["abbr"]);

                        string valInSourceDynamicMaster = Convert.ToString(BodyData[iBody][_sourceInMap]);
                        int iMasterIdToUpdate = Convert.ToInt32(BodyData[iBody][iMasterIdAPIKey]);

                        if(iMasterIdToUpdate > 0)
                        {
                            UpdateFiledByQuery(_destinyInMap, valInSourceDynamicMaster, updateTableName, iMasterIdToUpdate, CompanyId);
                            retValue = 1;
                        }
                    }
                }
            }
            return retValue;
        }

        static void UpdateFiledByQuery(string UpdateFiledName, string UpdateFiledValue, string TableName, int iMasterId, int CompanyId)
        {
            BL_DB DataAcesslayer = new BL_DB();
            string strErrorMessage = string.Empty;
            string strValue = "";

            int updateFiledValueINT;
            double updateFiledValueDOUBLE;

            if (Int32.TryParse(UpdateFiledValue, out updateFiledValueINT))
            {
                strValue = $@"UPDATE " + TableName + " SET " + UpdateFiledName + " = " + UpdateFiledValue + " FROM " + TableName + " WHERE iMasterId=" + iMasterId;
            }
            else if (Double.TryParse(UpdateFiledValue, out updateFiledValueDOUBLE))
            {
                strValue = $@"UPDATE " + TableName + " SET " + UpdateFiledName + " = " + UpdateFiledValue + " FROM " + TableName + " WHERE iMasterId=" + iMasterId;
            }
            else
            {
                strValue = $@"UPDATE " + TableName + " SET " + UpdateFiledName + " = '" + UpdateFiledValue + "' FROM " + TableName + " WHERE iMasterId=" + iMasterId;
            }

            //strValue = $@"UPDATE " + TableName + " SET " + UpdateFiledName + " = " + UpdateFiledValue + " FROM " + TableName + " WHERE iMasterId=" + iMasterId;


            clsGeneric.writeLog("Query :" + strValue);
            DataAcesslayer.GetExecute(strValue, CompanyId, ref strErrorMessage);
            if (strErrorMessage != "")
            {
                clsGeneric.writeLog("strErrorMessage :" + strErrorMessage);
            }
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