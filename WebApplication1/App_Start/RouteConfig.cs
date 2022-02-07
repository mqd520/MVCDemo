using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
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

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller1}/{action}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller1}/{action}",
            //    defaults: new { controller = "Home", action = "Index", controller1 = "hhhh" }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{action}/{controller}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{first}/{second}",
            //    defaults: new { id = UrlParameter.Optional, second = "Index", first = "Home" }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{first}/{second}",
            //    defaults: new { first = "Home", second = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Default2",
            //    url: "{action}/{controller}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller1}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
