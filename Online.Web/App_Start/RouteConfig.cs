using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Online.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            "tyle",
            "tyle/{*input}",
           new { controller = "Home", action = "Index" },
            new string[] { "Online.Web.Controllers" }
          );

            routes.MapRoute(
                   "Default",
                   "{controller}/{action}/{id}",
                   new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                     new string[] { "Online.Web.Controllers" }
            );
        }
    }
}
