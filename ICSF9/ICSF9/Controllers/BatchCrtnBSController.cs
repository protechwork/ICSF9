using ICSF9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ICSF9.Controllers
{
    public class BatchCrtnBSController : Controller
    {
        #region Variable Definition
        StringBuilder msg = new StringBuilder();
        string strErrorMessage = string.Empty;
        string strQry = string.Empty;
        #endregion

        public ActionResult UpdateMaster(int CompanyId, string SessionId, string User, int LoginId, string docNo,int irow)
        {
            
            return Json(new { status = true, data = new { succes =  docNo.ToString() + "/" +  irow.ToString() } });
        }
    }
}