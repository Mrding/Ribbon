using System;
using System.Diagnostics;

namespace Luna.Common
{
    [DebuggerDisplay("{Text} : {Start} ~ {End} {OnService}")]
    public class RtaaSlicedTerm : ITerm
    {


        public string Text { get; set; }

        public bool OnService { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int Level { get; set; }

        public bool Ignore { get; set; }

        
    }
}