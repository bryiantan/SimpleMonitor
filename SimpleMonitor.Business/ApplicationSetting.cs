using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business
{
    public static class ApplicationSetting
    {
        public static string JqGridDefaultPageSize
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["JqGridDefaultPageSize"];
            }
        }
    }
}
