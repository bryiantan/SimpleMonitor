using SimpleMonitor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SimpleMonitor.Web.Helper.Linq
{
    //code from http://vbcity.com/blogs/rock/archive/2012/11/23/leveraging-jqgrid-searches-and-dynamic-linq-on-asp-net-mvc-solutions.aspx

    public static class LinqDynamicConditionHelper  
    {  
        private static Dictionary<string, string> WhereOperation =   
                new Dictionary<string, string>  
                {  
                    { "eq", "{1} =  {2}({0})" },  
                    { "ne", "{1} != {2}({0})" },  
                    { "lt", "{1} <  {2}({0})" },  
                    { "le", "{1} <= {2}({0})" },  
                    { "gt", "{1} >  {2}({0})" },  
                    { "ge", "{1} >= {2}({0})" },  
                    { "bw", "{1}.StartsWith({2}({0}))" },  
                    { "bn", "!{1}.StartsWith({2}({0}))" },  
                    { "in", "" },  
                    { "ni", "" },  
                    { "ew", "{1}.EndsWith({2}({0}))" },  
                    { "en", "!{1}.EndsWith({2}({0}))" },  
                    { "cn", "{1}.Contains({2}({0}))" },  
                    { "nc", "!{1}.Contains({2}({0}))" },  
                    { "nu", "{1} == null" },  
                    { "nn", "{1} != null" }  
                };  
  
        private static Dictionary<string, string> ValidOperators =   
                new Dictionary<string, string>  
                {  
                    { "Object"   ,  "" },   
                    { "Boolean"  ,  "eq:ne:" },   
                    { "Char"     ,  "" },   
                    { "String"   ,  "eq:ne:lt:le:gt:ge:bw:bn:cn:nc:nu:nn:" },   //7/13/16 added nu/nn
                    { "SByte"    ,  "" },   
                    { "Byte"     ,  "eq:ne:lt:le:gt:ge:" },  
                    { "Int16"    ,  "eq:ne:lt:le:gt:ge:" },   
                    { "UInt16"   ,  "" },   
                    { "Int32"    ,  "eq:ne:lt:le:gt:ge:" },   
                    { "UInt32"   ,  "" },   
                    { "Int64"    ,  "eq:ne:lt:le:gt:ge:" },   
                    { "UInt64"   ,  "" },   
                    { "Decimal"  ,  "eq:ne:lt:le:gt:ge:" },   
                    { "Single"   ,  "eq:ne:lt:le:gt:ge:" },   
                    { "Double"   ,  "eq:ne:lt:le:gt:ge:" },   
                    { "DateTime" ,  "eq:ne:lt:le:gt:ge:" },   
                    { "TimeSpan" ,  "" },   
                    { "Guid"     ,  "" }  
                };

        public static string GetCondition<T>(RuleModel rule)
        {
            if (rule.data == "%")
            {
                // returns an empty string when the data is ‘%’  
                return "";
            }
            else
            {
                // initializing variables  
                Type myType = null;
                string myTypeName = string.Empty;
                string myTypeRawName = string.Empty;
                PropertyInfo myPropInfo = typeof(T).GetProperty(rule.field.Split('.')[0]);
                int index = 0;
                // navigating fields hierarchy  
                foreach (string wrkField in rule.field.Split('.'))
                {
                    if (index > 0)
                    {
                        myPropInfo = myPropInfo.PropertyType.GetProperty(wrkField);
                    }
                    index++;
                }
                // property type and its name  
                myType = myPropInfo.PropertyType;
                myTypeName = myPropInfo.PropertyType.Name;
                myTypeRawName = myTypeName;
                // handling ‘nullable’ fields  
                if (myTypeName.ToLower() == "nullable`1")
                {
                    myType = Nullable.GetUnderlyingType(myPropInfo.PropertyType);
                    myTypeName = myType.Name + "?";
                }
                // creating the condition expression  
                if (ValidOperators[myTypeRawName].Contains(rule.op + ":"))
                {
                    string expression = String.Format(WhereOperation[rule.op],
                                                      GetFormattedData(myType, rule.data),
                                                      rule.field,
                                                      myTypeName);
                    return expression;
                }
                else
                {
                    // un-supported operator  
                    return "";
                }
            }
        }
        private static string GetFormattedData(Type type, string value)  
        {  
            switch (type.Name.ToLower())  
            {  
                case "string":  
                    value = @"""" + value + @"""";  
                    break;  
                case "datetime":  
                    DateTime dt = DateTime.Parse(value);  
                    value = dt.Year.ToString() + "," +   
                            dt.Month.ToString() + "," +   
                            dt.Day.ToString();  
                    break;  
            }  
            return value;  
        }  
    }  
}