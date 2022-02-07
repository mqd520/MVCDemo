using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;

namespace WebApplication1.Controllers.Tools
{
    public static class RouteParser
    {
        private static string GetLiteral(string segmentLiteral)
        {
            string str = segmentLiteral.Replace("{{", "").Replace("}}", "");
            return ((str.Contains("{") || str.Contains("}")) ? null : segmentLiteral.Replace("{{", "{").Replace("}}", "}"));
        }

        private static int IndexOfFirstOpenParameter(string segment, int startIndex)
        {
            while (true)
            {
                startIndex = segment.IndexOf('{', startIndex);
                if (startIndex == -1)
                {
                    return -1;
                }
                if (((startIndex + 1) == segment.Length) || (((startIndex + 1) < segment.Length) && (segment[startIndex + 1] != '{')))
                {
                    return startIndex;
                }
                startIndex += 2;
            }
        }

        public static bool IsInvalidRouteUrl(string routeUrl)
        {
            return (routeUrl.StartsWith("~", StringComparison.Ordinal) || (routeUrl.StartsWith("/", StringComparison.Ordinal) || (routeUrl.IndexOf('?') != -1)));
        }

        public static bool IsSeparator(string s)
        {
            return string.Equals(s, "/", StringComparison.Ordinal);
        }

        private static bool IsValidParameterName(string parameterName)
        {
            if (parameterName.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < parameterName.Length; i++)
            {
                char ch = parameterName[i];
                if ((ch == '/') || ((ch == '{') || (ch == '}')))
                {
                    return false;
                }
            }
            return true;
        }

        public static ParsedRoute Parse(string routeUrl)
        {
            if (routeUrl == null)
            {
                routeUrl = string.Empty;
            }
            if (IsInvalidRouteUrl(routeUrl))
            {
                //throw new ArgumentException(System.Web.SR.GetString("Route_InvalidRouteUrl"), "routeUrl");
            }
            IList<string> pathSegments = SplitUrlToPathSegmentStrings(routeUrl);
            Exception exception = ValidateUrlParts(pathSegments);
            if (exception != null)
            {
                throw exception;
            }
            return new ParsedRoute(SplitUrlToPathSegments(pathSegments));
        }

        private static IList<PathSubsegment> ParseUrlSegment(string segment, out Exception exception)
        {
            int startIndex = 0;
            List<PathSubsegment> list = new List<PathSubsegment>();
            while (true)
            {
                if (startIndex < segment.Length)
                {
                    int num2 = IndexOfFirstOpenParameter(segment, startIndex);
                    if (num2 != -1)
                    {
                        int index = segment.IndexOf('}', num2 + 1);
                        if (index == -1)
                        {
                            object[] args = new object[] { segment };
                            exception = new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Route_MismatchedParameter", args), "routeUrl");
                            return null;
                        }
                        string str = GetLiteral(segment.Substring(startIndex, num2 - startIndex));
                        if (str == null)
                        {
                            object[] args = new object[] { segment };
                            exception = new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Route_MismatchedParameter", args), "routeUrl");
                            return null;
                        }
                        if (str.Length > 0)
                        {
                            list.Add(new LiteralSubsegment(str));
                        }
                        list.Add(new ParameterSubsegment(segment.Substring(num2 + 1, (index - num2) - 1)));
                        startIndex = index + 1;
                        continue;
                    }
                    string literal = GetLiteral(segment.Substring(startIndex));
                    if (literal == null)
                    {
                        object[] args = new object[] { segment };
                        exception = new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Route_MismatchedParameter", args), "routeUrl");
                        return null;
                    }
                    if (literal.Length > 0)
                    {
                        list.Add(new LiteralSubsegment(literal));
                    }
                }
                exception = null;
                return list;
            }
        }

        private static IList<PathSegment> SplitUrlToPathSegments(IList<string> urlParts)
        {
            List<PathSegment> list = new List<PathSegment>();
            foreach (string str in urlParts)
            {
                Exception exception;
                if (IsSeparator(str))
                {
                    list.Add(new SeparatorPathSegment());
                    continue;
                }
                list.Add(new ContentPathSegment(ParseUrlSegment(str, out exception)));
            }
            return list;
        }

        public static IList<string> SplitUrlToPathSegmentStrings(string url)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(url))
            {
                return list;
            }
            int startIndex = 0;
            while (true)
            {
                if (startIndex < url.Length)
                {
                    int index = url.IndexOf('/', startIndex);
                    if (index != -1)
                    {
                        string str = url.Substring(startIndex, index - startIndex);
                        if (str.Length > 0)
                        {
                            list.Add(str);
                        }
                        list.Add("/");
                        startIndex = index + 1;
                        continue;
                    }
                    string item = url.Substring(startIndex);
                    if (item.Length > 0)
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
        }

        private static Exception ValidateUrlParts(IList<string> pathSegments)
        {
            Exception exception;
            HashSet<string> usedParameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool? nullable = null;
            bool flag = false;
            using (IEnumerator<string> enumerator = pathSegments.GetEnumerator())
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        if (flag)
                        {
                            exception = new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Route_CatchAllMustBeLast", new object[0]), "routeUrl");
                        }
                        else
                        {
                            bool flag2;
                            Exception exception2;
                            if (nullable == null)
                            {
                                flag2 = new bool?(IsSeparator(current)).Value;
                            }
                            else
                            {
                                flag2 = IsSeparator(current);
                                if (flag2 && nullable.Value)
                                {
                                    exception = new ArgumentException("Route_CannotHaveConsecutiveSeparators", "routeUrl");
                                    break;
                                }
                                nullable = new bool?(flag2);
                            }
                            if (flag2)
                            {
                                continue;
                            }
                            IList<PathSubsegment> pathSubsegments = ParseUrlSegment(current, out exception2);
                            if (exception2 != null)
                            {
                                exception = exception2;
                            }
                            else
                            {
                                exception2 = ValidateUrlSegment(pathSubsegments, usedParameterNames, current);
                                if (exception2 == null)
                                {
                                    //Func<PathSubsegment, bool> predicate = <> c.<> 9__9_0;
                                    //if (<> c.<> 9__9_0 == null)
                                    //{
                                    //    Func<PathSubsegment, bool> local1 = <> c.<> 9__9_0;
                                    //    predicate = <> c.<> 9__9_0 = seg => (seg is ParameterSubsegment) && ((ParameterSubsegment)seg).IsCatchAll;
                                    //}
                                    //flag = pathSubsegments.Any<PathSubsegment>(predicate);
                                    continue;
                                }
                                exception = exception2;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
                }
            }
            return exception;
        }

        private static Exception ValidateUrlSegment(IList<PathSubsegment> pathSubsegments, HashSet<string> usedParameterNames, string pathSegment)
        {
            Exception exception;
            bool flag = false;
            Type type = null;
            using (IEnumerator<PathSubsegment> enumerator = pathSubsegments.GetEnumerator())
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        PathSubsegment current = enumerator.Current;
                        if ((type != null) && (type == current.GetType()))
                        {
                            exception = new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Route_CannotHaveConsecutiveParameters", new object[0]), "routeUrl");
                        }
                        else
                        {
                            type = current.GetType();
                            if (current is LiteralSubsegment)
                            {
                                continue;
                            }
                            ParameterSubsegment subsegment3 = current as ParameterSubsegment;
                            if (subsegment3 == null)
                            {
                                continue;
                            }
                            string parameterName = subsegment3.ParameterName;
                            if (subsegment3.IsCatchAll)
                            {
                                flag = true;
                            }
                            if (!IsValidParameterName(parameterName))
                            {
                                object[] args = new object[] { parameterName };
                                exception = new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Route_InvalidParameterName", args), "routeUrl");
                            }
                            else
                            {
                                if (!usedParameterNames.Contains(parameterName))
                                {
                                    usedParameterNames.Add(parameterName);
                                    continue;
                                }
                                object[] args = new object[] { parameterName };
                                exception = new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Route_RepeatedParameter", args), "routeUrl");
                            }
                        }
                    }
                    else
                    {
                        return ((!flag || (pathSubsegments.Count == 1)) ? null : new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Route_CannotHaveCatchAllInMultiSegment", new object[0]), "routeUrl"));
                    }
                    break;
                }
            }
            return exception;
        }
    }
}