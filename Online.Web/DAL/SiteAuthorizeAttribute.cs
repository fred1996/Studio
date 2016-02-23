using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.Web.Help;

namespace Online.Web.DAL
{
    public class SiteAuthorizeAttribute: AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            var attr = filterContext.ActionDescriptor.GetCustomAttributes(false)
                                    .OfType<AllowAnonymousAttribute>()
                                    .FirstOrDefault();
            var allowAnonymous = attr != null;
            if (allowAnonymous)
                return;
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var token = Convert.ToInt64(HttpContext.Current.Session["UserId"]);
                if (token <= 0)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            var returnUrl = HttpUtility.UrlDecode("/" + controllerName + "/" + actionName);
            filterContext.Result = new RedirectResult("/Account/Login?returnUrl="+ returnUrl);
        }
    }
}