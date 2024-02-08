using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICSF9.Models
{


    public class Pur_Ord
    {
        public string vno { get; set; }
        public string product { get; set; }
        public float quantity { get; set; }
        public float rate { get; set; }
        public int idate { get; set; }
        public string act { get; set; }

    }
    public class PEBody
    {
        public string FieldText { get; set; }
        public string FieldValue { get; set; }
        public long iFieldId { get; set; }
        public string sFieldName { get; set; }
        public int iPosition { get; set; }
        public int iDataTypeId { get; set; }

    }
    public class BodyData
    {
        public Int32 RowNo { get; set; }
        public string Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Gross { get; set; }

    }

    public class SysStkGenBody
    {
        public int taxid { get; set; }
        public int prdid { get; set; }
        public int unitid { get; set; }
        public decimal prdQty { get; set; }
        public decimal prdRate { get; set; }
        public string Batch { get; set; }
        public int bDate { get; set; }

        public int fgCode { get; set; }

    }

    public class FGLinRejBody
    {
        
        public int prdid { get; set; }
        public int unitid { get; set; }
        public decimal prdQty { get; set; }
        public decimal prdRate { get; set; }
        public string Batch { get; set; }
        public string bDate { get; set; }
        public decimal fgRejQty { get; set; }
        public int fgCode { get; set; }

    }

    public class MRPRMBodyData
    {
        public int branchid { get; set; }
        public int workcentreid { get; set; }
        public int prdid { get; set; }
        public int unitid { get; set; }
        public decimal planQty { get; set; }
        public decimal sfgreqQty { get; set; }
        public decimal bomQty { get; set; }

        public decimal rmreqQty { get; set; }
        public decimal qunatity { get; set; }

        public int fgCode { get; set; }
        public int sfgid { get; set; }
    }

}
