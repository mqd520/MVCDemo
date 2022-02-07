using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WebApplication1.Controllers.Tools
{
    public class Route : RouteBase
    {
        private const string HttpMethodParameterName = "httpMethod";
        private string _url;
        private ParsedRoute _parsedRoute;

        public Route(string url, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.Constraints = constraints;
            this.RouteHandler = routeHandler;
        }

        public Route(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.Constraints = constraints;
            this.DataTokens = dataTokens;
            this.RouteHandler = routeHandler;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            RouteValueDictionary values = this._parsedRoute.Match(virtualPath, this.Defaults);
            if (values == null)
            {
                return null;
            }
            RouteData data = new RouteData(this, this.RouteHandler);
            if (!this.ProcessConstraints(httpContext, values, RouteDirection.IncomingRequest))
            {
                return null;
            }
            foreach (KeyValuePair<string, object> pair in values)
            {
                data.Values.Add(pair.Key, pair.Value);
            }
            if (this.DataTokens != null)
            {
                foreach (KeyValuePair<string, object> pair2 in this.DataTokens)
                {
                    data.DataTokens[pair2.Key] = pair2.Value;
                }
            }
            return data;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            BoundUrl url = this._parsedRoute.Bind(requestContext.RouteData.Values, values, this.Defaults, this.Constraints);
            if (url == null)
            {
                return null;
            }
            if (!this.ProcessConstraints(requestContext.HttpContext, url.Values, RouteDirection.UrlGeneration))
            {
                return null;
            }
            VirtualPathData data = new VirtualPathData(this, url.Url);
            if (this.DataTokens != null)
            {
                foreach (KeyValuePair<string, object> pair in this.DataTokens)
                {
                    data.DataTokens[pair.Key] = pair.Value;
                }
            }
            return data;
        }

        protected virtual bool ProcessConstraint(HttpContextBase httpContext, object constraint, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            IRouteConstraint constraint2 = constraint as IRouteConstraint;
            if (constraint2 != null)
            {
                //return constraint2.Match(httpContext, this, parameterName, values, routeDirection);
                return true;
            }
            string str = constraint as string;
            if (str != null)
            {
                object obj2;
                values.TryGetValue(parameterName, out obj2);
                return Regex.IsMatch(Convert.ToString(obj2, CultureInfo.InvariantCulture), "^(" + str + ")$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }
            object[] args = new object[] { parameterName, this.Url };
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Route_ValidationMustBeStringOrCustomConstraint", args));
        }

        private bool ProcessConstraints(HttpContextBase httpContext, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (this.Constraints != null)
            {
                using (Dictionary<string, object>.Enumerator enumerator = this.Constraints.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        KeyValuePair<string, object> current = enumerator.Current;
                        if (!this.ProcessConstraint(httpContext, current.Value, current.Key, values, routeDirection))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public RouteValueDictionary Constraints { get; set; }

        public RouteValueDictionary DataTokens { get; set; }

        public RouteValueDictionary Defaults { get; set; }

        public IRouteHandler RouteHandler { get; set; }

        public string Url
        {
            get
            {
                string text1 = this._url;
                if (this._url == null)
                {
                    string local1 = this._url;
                    text1 = string.Empty;
                }
                return text1;
            }
            set
            {
                this._parsedRoute = RouteParser.Parse(value);
                this._url = value;
            }
        }
    }
}