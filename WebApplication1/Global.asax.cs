using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Common;

using WebApplication1.App_Start;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.ILog logger = log4net.LogManager.GetLogger("myDebugAppender");
            CommonLogger.WriteLog(ELogCategory.Info, "WebApplication1 MvcApplication Startup ...");

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            MvcHandler.DisableMvcResponseHeader = false;
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.Headers.Remove("Server");
                app.Context.Response.Headers.Remove("X-AspNet-Version");
                app.Context.Response.Headers.Remove("X-AspNetMvc-Version");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpApplication http = sender as HttpApplication;
            Exception exception = Server.GetLastError();
            CommonLogger.WriteLog(
                ELogCategory.Fatal,
                string.Format("MvcApplication.Application_Error Exception: {0}", exception.Message),
                e: exception
            );

            Server.ClearError();
            http.Response.StatusCode = 500;
        }
    }
}
