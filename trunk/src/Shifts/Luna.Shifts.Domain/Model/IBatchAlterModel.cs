using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    [IgnoreRegister]
    public interface IBatchAlterModel
    {
        string Category { get; }

        string Title { get; }

        DateTime SearchDate { get; set; }

        void Initial();


        IEnumerable<ICustomAction> OptionalActions { get; }

        void OnDispatching();


        void TryDispatch(IAgent agent, Func<Term, bool> filter, Action<Term, TimeBox> action, Action<ITerm, string, bool> callback);

        void Comment(string content);

        void TearDown();

    }
}