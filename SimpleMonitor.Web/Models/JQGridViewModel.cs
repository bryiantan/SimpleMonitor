using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleMonitor.Web.Models
{
    public class JQGridViewModel
    {
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public bool _search { get; set; }
        public string filters { get; set; }

    }

    public class RuleModel
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }

    public class JQGridFilterModel
    {
        public string groupOp { get; set; }
        public List<RuleModel> rules { get; set; }
    }

}