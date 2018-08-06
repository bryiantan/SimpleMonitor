using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SimpleMonitor.Web.Helper.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class AttributeHelpers : AuthorizeAttribute
    {
        #region Ajax AsynchronousAntiForgeryTokenAttribute
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ValidateAjaxAntiForgeryToken : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                try
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest()) // if it is ajax request.
                    {
                        this.ValidateRequestHeader(filterContext.HttpContext.Request); // run the validation.
                    }
                    else
                    {
                        AntiForgery.Validate();
                    }
                }
                catch (HttpAntiForgeryException e)
                {
                    throw new HttpAntiForgeryException("Antiforgery token not found");
                }
            }

            private void ValidateRequestHeader(HttpRequestBase request)
            {
                string cookieToken = string.Empty;
                string formToken = string.Empty;
                string tokenValue = request.Headers["RequestVerificationToken"]; 
                if (!string.IsNullOrEmpty(tokenValue))
                {
                    string[] tokens = tokenValue.Split(',');
                    if (tokens.Length == 2)
                    {
                        cookieToken = tokens[0].Trim();
                        formToken = tokens[1].Trim();
                    }
                }

                AntiForgery.Validate(cookieToken, formToken); 
            }
        }
        #endregion
    }
}