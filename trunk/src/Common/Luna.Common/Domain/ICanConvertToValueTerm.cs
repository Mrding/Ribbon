using System;

namespace Luna.Common
{
    public interface ICanConvertToValueTerm : IHierarchicalTerm, IStyledTerm
    {
        TimeValueRange TimeRange { get; set; }

        bool SetNewTime(DateTime start, DateTime end);

    }
}