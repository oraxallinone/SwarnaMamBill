using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONSTRUCTION.Models
{
    public class tblBudget_MV
    {
        public int id { get; set; }
        public string details { get; set; }
        public Nullable<decimal> price { get; set; }
        public string createdDate { get; set; }
    }


    public class tblBudget_all_MV
    {
        public int id { get; set; }
        public string group1 { get; set; }
        public string group2 { get; set; }
        public string group3 { get; set; }
        public string group4 { get; set; }
        public string details { get; set; }
        public Nullable<decimal> price { get; set; }
        public string createdDate { get; set; }
    }
}