using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SessionId()
        {
            return View();
        }

        public ActionResult Error500()
        {
            int n = Convert.ToInt32("sdddd");

            return View();
        }

        public ActionResult Error404()
        {
            Response.StatusCode = 404;

            return View();
        }

        public ActionResult Error403()
        {
            Response.StatusCode = 403;

            return View();
        }

        public ActionResult Error401()
        {
            Response.StatusCode = 401;

            return View();
        }

        public ActionResult ModuleList()
        {
            HttpModuleCollection hmc = HttpContext.ApplicationInstance.Modules;

            return View(hmc);
        }
    }
}