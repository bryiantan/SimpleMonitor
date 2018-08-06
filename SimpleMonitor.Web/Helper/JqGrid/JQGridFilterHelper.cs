using SimpleMonitor.Web.Helper.Linq;
using SimpleMonitor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace SimpleMonitor.Web.Helper.JqGrid
{
    public static class JQGridFilterHelper
    {
        public static string FilterQuery<T>(JQGridViewModel jqGridViewModel)
        {
            StringBuilder sb = new StringBuilder();
            int counter = 0;
            string rtRule = string.Empty;
            string rtWhere = string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            JQGridFilterModel filters = serializer.Deserialize<JQGridFilterModel>(
                                                                  jqGridViewModel.filters
                                                                                 );
            foreach (RuleModel rule in filters.rules)
            {
                if (rule.data == "ALL")
                {
                    rtRule = "1=1";
                }
                else
                {
                    rtRule = LinqDynamicConditionHelper.GetCondition<T>(rule);
                }
                
                if (rtRule.Length > 0)
                {
                    counter++;
                    sb.Append(rtRule);

                    //check if needed operator at the end 
                    if (filters.rules.Count() > counter)
                    {
                        sb.Append(filters.groupOp.ToLower() == "and" ? " && " : " || ");
                    }
                }
            }

            return sb.ToString();
        }
    }
}