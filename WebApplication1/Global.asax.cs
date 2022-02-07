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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //HttpApplication app = sender as HttpApplication;
            //HttpContextWrapper context = new HttpContextWrapper(app.Context);

            //var dict = new RouteValueDictionary();
            //dict.Add("controller", "Home");
            //dict.Add("action", "Index");
            //var p = WebApplication1.Controllers.Tools.RouteParser.Parse("{controller}/{action}");
            //var values = p.Match("Home1/Index1/Id1", dict);
            //var n = 0;
            //n++;

            //RouteData routeData = RouteTable.Routes.GetRouteData(context);
            //if (routeData != null)
            //{
            //    IRouteHandler routeHandler = routeData.RouteHandler;
            //    if (routeHandler == null)
            //    {
            //        //throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, System.Web.SR.GetString("UrlRoutingModule_NoRouteHandler"), new object[0]));
            //    }
            //    if (!(routeHandler is StopRoutingHandler))
            //    {
            //        RequestContext requestContext = new RequestContext(context, routeData);
            //        context.Request.RequestContext = requestContext;
            //        IHttpHandler httpHandler = routeHandler.GetHttpHandler(requestContext);
            //        if (httpHandler == null)
            //        {
            //            object[] args = new object[] { routeHandler.GetType() };
            //            //throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, System.Web.SR.GetString("UrlRoutingModule_NoHttpHandler"), args));
            //        }
            //    }
            //}
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpApplication http = sender as HttpApplication;
            Exception exception = Server.GetLastError();
            Exception[] exceptions = http.Context.AllErrors;
            CommonLogger.WriteLog(
                ELogCategory.Fatal,
                string.Format("MvcApplication.Application_Error Exception: {0}", exception.Message),
                e: exception
            );

            //Server.ClearError();
            //http.Response.StatusCode = 500;
        }
    }
}
