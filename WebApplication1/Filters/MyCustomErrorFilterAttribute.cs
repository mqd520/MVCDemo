﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Common;

namespace WebApplication1.Filters
{
    public class MyCustomErrorFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            CommonLogger.WriteLog(
                ELogCategory.Fatal,
                string.Format("MyCustomErrorFilterAttribute.OnException, Msg: {0}", filterContext.Exception.Message),
                filterContext.Exception
            );

            if (!filterContext.ExceptionHandled)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Server.ClearError();
                filterContext.HttpContext.Response.StatusCode = 500;
            }
        }
    }
}