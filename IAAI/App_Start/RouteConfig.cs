using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IAAI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // 似乎沒有用
            routes.MapRoute(
                name: "NewsDetails",
                url: "Home/News/Details/{id}",
                defaults: new { controller = "News", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "KnowledgeDetail",
                url: "Home/Knowledge/Details/{id}",
                defaults: new { controller = "Home", action = "KnowledgeDetail", id = UrlParameter.Optional }
            );
        }
    }
}