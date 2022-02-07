using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace WebApplication1.Controllers.Tools
{
    public sealed class ParsedRoute
    {
        public ParsedRoute(IList<PathSegment> pathSegments)
        {
            this.PathSegments = pathSegments;
        }

        public BoundUrl Bind(RouteValueDictionary currentValues, RouteValueDictionary values, RouteValueDictionary defaultValues, RouteValueDictionary constraints)
        {
            StringBuilder builder;
            StringBuilder builder2;
            bool flag2;
            bool flag3;
            BoundUrl url;
            int num;
            if (currentValues == null)
            {
                currentValues = new RouteValueDictionary();
            }
            if (values == null)
            {
                values = new RouteValueDictionary();
            }
            if (defaultValues == null)
            {
                defaultValues = new RouteValueDictionary();
            }
            RouteValueDictionary acceptedValues = new RouteValueDictionary();
            HashSet<string> unusedNewValues = new HashSet<string>(values.Keys, StringComparer.OrdinalIgnoreCase);
            ForEachParameter(this.PathSegments, delegate (ParameterSubsegment parameterSubsegment) {
                object obj2;
                object obj3;
                string parameterName = parameterSubsegment.ParameterName;
                bool flag = values.TryGetValue(parameterName, out obj2);
                if (flag)
                {
                    unusedNewValues.Remove(parameterName);
                }
                bool flag22 = currentValues.TryGetValue(parameterName, out obj3);
                if ((flag & flag22) && !RoutePartsEqual(obj3, obj2))
                {
                    return false;
                }
                if (flag)
                {
                    if (IsRoutePartNonEmpty(obj2))
                    {
                        acceptedValues.Add(parameterName, obj2);
                    }
                }
                else if (flag22)
                {
                    acceptedValues.Add(parameterName, obj3);
                }
                return true;
            });
            foreach (KeyValuePair<string, object> pair in values)
            {
                if (IsRoutePartNonEmpty(pair.Value) && !acceptedValues.ContainsKey(pair.Key))
                {
                    acceptedValues.Add(pair.Key, pair.Value);
                }
            }
            foreach (KeyValuePair<string, object> pair2 in currentValues)
            {
                string key = pair2.Key;
                if (!acceptedValues.ContainsKey(key) && (GetParameterSubsegment(this.PathSegments, key) == null))
                {
                    acceptedValues.Add(key, pair2.Value);
                }
            }
            ForEachParameter(this.PathSegments, delegate (ParameterSubsegment parameterSubsegment) {
                object obj2;
                if (!acceptedValues.ContainsKey(parameterSubsegment.ParameterName) && !IsParameterRequired(parameterSubsegment, defaultValues, out obj2))
                {
                    acceptedValues.Add(parameterSubsegment.ParameterName, obj2);
                }
                return true;
            });
            if (!ForEachParameter(this.PathSegments, delegate (ParameterSubsegment parameterSubsegment) {
                object obj2;
                return !IsParameterRequired(parameterSubsegment, defaultValues, out obj2) || acceptedValues.ContainsKey(parameterSubsegment.ParameterName);
            }))
            {
                return null;
            }
            RouteValueDictionary otherDefaultValues = new RouteValueDictionary(defaultValues);
            ForEachParameter(this.PathSegments, delegate (ParameterSubsegment parameterSubsegment) {
                otherDefaultValues.Remove(parameterSubsegment.ParameterName);
                return true;
            });
            using (Dictionary<string, object>.Enumerator enumerator3 = otherDefaultValues.GetEnumerator())
            {
                while (true)
                {
                    if (enumerator3.MoveNext())
                    {
                        object obj2;
                        KeyValuePair<string, object> current = enumerator3.Current;
                        if (!values.TryGetValue(current.Key, out obj2))
                        {
                            continue;
                        }
                        unusedNewValues.Remove(current.Key);
                        if (RoutePartsEqual(obj2, current.Value))
                        {
                            continue;
                        }
                        url = null;
                    }
                    else
                    {
                        builder = new StringBuilder();
                        builder2 = new StringBuilder();
                        flag2 = false;
                        flag3 = false;
                        num = 0;
                        goto TR_0049;
                    }
                    break;
                }
            }
            return url;
        TR_001B:
            num++;
        TR_0049:
            while (true)
            {
                if (num >= this.PathSegments.Count)
                {
                    if (flag2 && (builder2.Length > 0))
                    {
                        if (flag3)
                        {
                            return null;
                        }
                        builder.Append(builder2.ToString());
                    }
                    if (constraints != null)
                    {
                        foreach (KeyValuePair<string, object> pair4 in constraints)
                        {
                            unusedNewValues.Remove(pair4.Key);
                        }
                    }
                    if (unusedNewValues.Count > 0)
                    {
                        bool flag6 = true;
                        foreach (string str2 in unusedNewValues)
                        {
                            object obj5;
                            if (acceptedValues.TryGetValue(str2, out obj5))
                            {
                                builder.Append(flag6 ? '?' : '&');
                                flag6 = false;
                                builder.Append(Uri.EscapeDataString(str2));
                                builder.Append('=');
                                builder.Append(Uri.EscapeDataString(Convert.ToString(obj5, CultureInfo.InvariantCulture)));
                            }
                        }
                    }
                    BoundUrl url1 = new BoundUrl();
                    url1.Url = builder.ToString();
                    url1.Values = acceptedValues;
                    return url1;
                }
                PathSegment segment = this.PathSegments[num];
                if (segment is SeparatorPathSegment)
                {
                    if (flag2 && (builder2.Length > 0))
                    {
                        if (flag3)
                        {
                            return null;
                        }
                        builder.Append(builder2.ToString());
                        builder2.Length = 0;
                    }
                    flag2 = false;
                    if ((builder2.Length <= 0) || (builder2[builder2.Length - 1] != '/'))
                    {
                        builder2.Append("/");
                    }
                    else
                    {
                        if (flag3)
                        {
                            return null;
                        }
                        builder.Append(builder2.ToString(0, builder2.Length - 1));
                        builder2.Length = 0;
                        flag3 = true;
                    }
                    goto TR_001B;
                }
                else
                {
                    ContentPathSegment segment2 = segment as ContentPathSegment;
                    if (segment2 != null)
                    {
                        bool flag4 = false;
                        using (IEnumerator<PathSubsegment> enumerator4 = segment2.Subsegments.GetEnumerator())
                        {
                            while (true)
                            {
                                if (enumerator4.MoveNext())
                                {
                                    object obj3;
                                    object obj4;
                                    PathSubsegment current = enumerator4.Current;
                                    LiteralSubsegment subsegment3 = current as LiteralSubsegment;
                                    if (subsegment3 != null)
                                    {
                                        flag2 = true;
                                        builder2.Append(UrlEncode(subsegment3.Literal));
                                        continue;
                                    }
                                    ParameterSubsegment subsegment4 = current as ParameterSubsegment;
                                    if (subsegment4 == null)
                                    {
                                        continue;
                                    }
                                    if (flag2 && (builder2.Length > 0))
                                    {
                                        if (flag3)
                                        {
                                            url = null;
                                            break;
                                        }
                                        builder.Append(builder2.ToString());
                                        builder2.Length = 0;
                                        flag4 = true;
                                    }
                                    flag2 = false;
                                    if (acceptedValues.TryGetValue(subsegment4.ParameterName, out obj3))
                                    {
                                        unusedNewValues.Remove(subsegment4.ParameterName);
                                    }
                                    defaultValues.TryGetValue(subsegment4.ParameterName, out obj4);
                                    if (RoutePartsEqual(obj3, obj4))
                                    {
                                        builder2.Append(UrlEncode(Convert.ToString(obj3, CultureInfo.InvariantCulture)));
                                    }
                                    else
                                    {
                                        if (flag3)
                                        {
                                            url = null;
                                            break;
                                        }
                                        if (builder2.Length > 0)
                                        {
                                            builder.Append(builder2.ToString());
                                            builder2.Length = 0;
                                        }
                                        builder.Append(UrlEncode(Convert.ToString(obj3, CultureInfo.InvariantCulture)));
                                        flag4 = true;
                                    }
                                    continue;
                                }
                                if (flag4 && (builder2.Length > 0))
                                {
                                    if (flag3)
                                    {
                                        return null;
                                    }
                                    builder.Append(builder2.ToString());
                                    builder2.Length = 0;
                                }
                                goto TR_001B;
                            }
                        }
                        break;
                    }
                    goto TR_001B;
                }
                break;
            }
            return url;
        }

        private static string EscapeReservedCharacters(System.Text.RegularExpressions.Match m)
        {
            return ("%" + Convert.ToUInt16(m.Value[0]).ToString("x2", CultureInfo.InvariantCulture));
        }

        private static bool ForEachParameter(IList<PathSegment> pathSegments, Func<ParameterSubsegment, bool> action)
        {
            for (int i = 0; i < pathSegments.Count; i++)
            {
                PathSegment segment = pathSegments[i];
                if (!(segment is SeparatorPathSegment))
                {
                    ContentPathSegment segment2 = segment as ContentPathSegment;
                    if (segment2 != null)
                    {
                        using (IEnumerator<PathSubsegment> enumerator = segment2.Subsegments.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator.MoveNext())
                                {
                                    break;
                                }
                                PathSubsegment current = enumerator.Current;
                                LiteralSubsegment subsegment2 = current as LiteralSubsegment;
                                if (subsegment2 == null)
                                {
                                    ParameterSubsegment arg = current as ParameterSubsegment;
                                    if ((arg != null) && !action(arg))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static ParameterSubsegment GetParameterSubsegment(IList<PathSegment> pathSegments, string parameterName)
        {
            ParameterSubsegment foundParameterSubsegment = null;
            bool flag = ForEachParameter(pathSegments, delegate (ParameterSubsegment parameterSubsegment) {
                if (!string.Equals(parameterName, parameterSubsegment.ParameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                foundParameterSubsegment = parameterSubsegment;
                return false;
            });
            return foundParameterSubsegment;
        }

        private static bool IsParameterRequired(ParameterSubsegment parameterSubsegment, RouteValueDictionary defaultValues, out object defaultValue)
        {
            if (!parameterSubsegment.IsCatchAll)
            {
                return !defaultValues.TryGetValue(parameterSubsegment.ParameterName, out defaultValue);
            }
            defaultValue = null;
            return false;
        }

        private static bool IsRoutePartNonEmpty(object routePart)
        {
            string str = routePart as string;
            return ((str == null) ? (routePart != null) : (str.Length > 0));
        }

        public RouteValueDictionary Match(string virtualPath, RouteValueDictionary defaultValues)
        {
            IList<string> source = RouteParser.SplitUrlToPathSegmentStrings(virtualPath);
            if (defaultValues == null)
            {
                defaultValues = new RouteValueDictionary();
            }
            RouteValueDictionary matchedValues = new RouteValueDictionary();
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < this.PathSegments.Count; i++)
            {
                PathSegment segment = this.PathSegments[i];
                if (source.Count <= i)
                {
                    flag = true;
                }
                string a = flag ? null : source[i];
                if (segment is SeparatorPathSegment)
                {
                    if (!flag && !string.Equals(a, "/", StringComparison.Ordinal))
                    {
                        return null;
                    }
                }
                else
                {
                    ContentPathSegment contentPathSegment = segment as ContentPathSegment;
                    if (contentPathSegment != null)
                    {
                        if (contentPathSegment.IsCatchAll)
                        {
                            this.MatchCatchAll(contentPathSegment, source.Skip<string>(i), defaultValues, matchedValues);
                            flag2 = true;
                        }
                        else if (!this.MatchContentPathSegment(contentPathSegment, a, defaultValues, matchedValues))
                        {
                            return null;
                        }
                    }
                }
            }
            if (!flag2 && (this.PathSegments.Count < source.Count))
            {
                for (int j = this.PathSegments.Count; j < source.Count; j++)
                {
                    if (!RouteParser.IsSeparator(source[j]))
                    {
                        return null;
                    }
                }
            }
            if (defaultValues != null)
            {
                foreach (KeyValuePair<string, object> pair in defaultValues)
                {
                    if (!matchedValues.ContainsKey(pair.Key))
                    {
                        matchedValues.Add(pair.Key, pair.Value);
                    }
                }
            }
            return matchedValues;
        }

        private void MatchCatchAll(ContentPathSegment contentPathSegment, IEnumerable<string> remainingRequestSegments, RouteValueDictionary defaultValues, RouteValueDictionary matchedValues)
        {
            object obj2;
            string str = string.Join(string.Empty, remainingRequestSegments.ToArray<string>());
            ParameterSubsegment subsegment = contentPathSegment.Subsegments[0] as ParameterSubsegment;
            if (str.Length > 0)
            {
                obj2 = str;
            }
            else
            {
                defaultValues.TryGetValue(subsegment.ParameterName, out obj2);
            }
            matchedValues.Add(subsegment.ParameterName, obj2);
        }

        private bool MatchContentPathSegment(ContentPathSegment routeSegment, string requestPathSegment, RouteValueDictionary defaultValues, RouteValueDictionary matchedValues)
        {
            if (string.IsNullOrEmpty(requestPathSegment))
            {
                object obj2;
                if (routeSegment.Subsegments.Count > 1)
                {
                    return false;
                }
                ParameterSubsegment subsegment3 = routeSegment.Subsegments[0] as ParameterSubsegment;
                if (subsegment3 == null)
                {
                    return false;
                }
                if (!defaultValues.TryGetValue(subsegment3.ParameterName, out obj2))
                {
                    return false;
                }
                matchedValues.Add(subsegment3.ParameterName, obj2);
                return true;
            }
            int length = requestPathSegment.Length;
            int num2 = routeSegment.Subsegments.Count - 1;
            ParameterSubsegment subsegment = null;
            LiteralSubsegment subsegment2 = null;
            while (num2 >= 0)
            {
                int num3 = length;
                ParameterSubsegment subsegment4 = routeSegment.Subsegments[num2] as ParameterSubsegment;
                if (subsegment4 != null)
                {
                    subsegment = subsegment4;
                }
                else
                {
                    LiteralSubsegment subsegment5 = routeSegment.Subsegments[num2] as LiteralSubsegment;
                    if (subsegment5 != null)
                    {
                        subsegment2 = subsegment5;
                        int startIndex = length - 1;
                        if (subsegment != null)
                        {
                            startIndex--;
                        }
                        if (startIndex < 0)
                        {
                            return false;
                        }
                        int num5 = requestPathSegment.LastIndexOf(subsegment5.Literal, startIndex, StringComparison.OrdinalIgnoreCase);
                        if (num5 == -1)
                        {
                            return false;
                        }
                        if ((num2 == (routeSegment.Subsegments.Count - 1)) && ((num5 + subsegment5.Literal.Length) != requestPathSegment.Length))
                        {
                            return false;
                        }
                        num3 = num5;
                    }
                }
                if ((subsegment != null) && (((subsegment2 != null) && (subsegment4 == null)) || (num2 == 0)))
                {
                    int num6;
                    int num7;
                    if (subsegment2 == null)
                    {
                        num6 = (num2 != 0) ? num3 : 0;
                        num7 = length;
                    }
                    else if ((num2 == 0) && (subsegment4 != null))
                    {
                        num6 = 0;
                        num7 = length;
                    }
                    else
                    {
                        num6 = num3 + subsegment2.Literal.Length;
                        num7 = length - num6;
                    }
                    string str = requestPathSegment.Substring(num6, num7);
                    if (string.IsNullOrEmpty(str))
                    {
                        return false;
                    }
                    matchedValues.Add(subsegment.ParameterName, str);
                    subsegment = null;
                    subsegment2 = null;
                }
                length = num3;
                num2--;
            }
            return ((length == 0) || (routeSegment.Subsegments[0] is ParameterSubsegment));
        }

        private static bool RoutePartsEqual(object a, object b)
        {
            string str = a as string;
            string str2 = b as string;
            return (((str == null) || (str2 == null)) ? (((a == null) || (b == null)) ? (a == b) : a.Equals(b)) : string.Equals(str, str2, StringComparison.OrdinalIgnoreCase));
        }

        private static string UrlEncode(string str)
        {
            return Regex.Replace(Uri.EscapeUriString(str), "([#;?:@&=+$,])", new MatchEvaluator(ParsedRoute.EscapeReservedCharacters));
        }

        private IList<PathSegment> PathSegments { get; set; }
    }

}