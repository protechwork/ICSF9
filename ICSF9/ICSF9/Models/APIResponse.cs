using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICSF9.Models
{
    public class APIResponse
    {
        public class PostResponse
        {
            public string url { get; set; }
            public List<Hashtable> data { get; set; }
            public int result { get; set; }
            public string message { get; set; }
        }
    }
}