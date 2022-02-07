using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Controllers.Tools
{
    public sealed class ParameterSubsegment : PathSubsegment
    {
        public ParameterSubsegment(string parameterName)
        {
            if (!parameterName.StartsWith("*", StringComparison.Ordinal))
            {
                this.ParameterName = parameterName;
            }
            else
            {
                this.ParameterName = parameterName.Substring(1);
                this.IsCatchAll = true;
            }
        }

        public bool IsCatchAll { get; private set; }

        public string ParameterName { get; private set; }
    }
}