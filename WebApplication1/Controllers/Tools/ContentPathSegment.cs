using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Controllers.Tools
{
    public sealed class ContentPathSegment : PathSegment
    {
        public ContentPathSegment(IList<PathSubsegment> subsegments)
        {
            this.Subsegments = subsegments;
        }

        public bool IsCatchAll
        {
            get
            {
                //Func<PathSubsegment, bool> predicate = <> c.<> 9__2_0;
                //if (<> c.<> 9__2_0 == null)
                //{
                //    Func<PathSubsegment, bool> local1 = <> c.<> 9__2_0;
                //    predicate = <> c.<> 9__2_0 = seg => (seg is ParameterSubsegment) && ((ParameterSubsegment)seg).IsCatchAll;
                //}
                //return this.Subsegments.Any<PathSubsegment>(predicate);

                return false;
            }
        }

        public IList<PathSubsegment> Subsegments { get; private set; }
    }
}