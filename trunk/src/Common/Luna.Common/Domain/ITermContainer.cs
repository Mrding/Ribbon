using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Luna.Common.Domain
{
    public interface ITermContainer : IEnumerable
    {
        //IEnumerable GetAllTerms(); 

        IEnumerable Fetch(DateTime start, DateTime end);

        //IList Result { get; }

        //void SetTime(ITerm term, DateTime start, DateTime end, Action<ITerm, bool> callback);
    }
}
