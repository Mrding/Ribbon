using System;

namespace Luna.Common
{
    public interface ITerm
    {
        DateTime Start { get; }

        DateTime End { get; }

        //bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd);

    }

    public interface IDateRange
    {
        DateTime Start { get; }

        DateTime End { get; }
    }

    public interface IWritableTerm : ITerm
    {
        new DateTime Start { get; set; }

        new DateTime End { get; set; }
    }
}