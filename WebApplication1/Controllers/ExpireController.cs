using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class ExpireController : Controller
    {
        // GET: Expire
        public ActionResult Index()
        {
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