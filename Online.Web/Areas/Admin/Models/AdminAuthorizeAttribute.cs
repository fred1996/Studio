using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.BLL;

namespace Online.Web.Areas.Admin.Models
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
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
                var token =Convert.ToInt64(HttpContext.Current.Session["CrmUserId"].ToString());
                if (token==0) return false;
                var user = DataSource.Userses.FirstOrDefault(t => t.UserID == token);
                if (user?.UserRoleses == null || !user.UserRoleses.Any(t=>t.Roles.PowerId>= 1000))
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
            filterContext.Result = new RedirectResult("/Admin/Home/Login?returnUrl=/Admin/" + returnUrl);
        }

        private UserContextBll _dataSource;

        public UserContextBll DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new UserContextBll();
                }
                return _dataSource;
            }
        }
    }
}