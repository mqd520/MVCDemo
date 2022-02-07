using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Controllers.Tools
{
    public sealed class LiteralSubsegment : PathSubsegment
    {
        public LiteralSubsegment(string literal)
        {
            this.Literal = literal;
        }

        public string Literal { get; private set; }
    }
}