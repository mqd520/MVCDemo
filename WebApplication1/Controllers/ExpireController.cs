using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1.Controllers
{
    public class ExpireController : Controller
    {
        // GET: Expire
        public ActionResult Index()
        {
            var dict = new RouteValueDictionary();
            dict.Add("controller", "Home");
            dict.Add("action", "Index");

            var route = new WebApplication1.Controllers.Tools.Route("{controller}/{action}/{id}", null)
            {
                Constraints = new RouteValueDictionary(),
                DataTokens = new RouteValueDictionary(),
                Defaults = dict
            };
            var routeData = route.GetRouteData(Request.RequestContext.HttpContext);


            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.AddHeader("progma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";

            ViewBag.Title = DateTime.Now.ToString("mm:ss");

            return View();
        }
    }
}