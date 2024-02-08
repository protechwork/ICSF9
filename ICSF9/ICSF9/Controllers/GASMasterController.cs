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
    public class GASMasterController : Controller
    {
        #region Variable Definition

        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        #endregion

        public ActionResult UpdateMaster(int CompanyId, string SessionId, string User, int LoginId, int vtype, string docNo, List<PEBody> BodyData)
        {

            BL_DB DataAcesslayer = new BL_DB();
            return Json(new { status = true, data = new { succes = msg.ToString() } });
        }
       

    }
}