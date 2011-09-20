using System;
using Luna.Common;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{
    public class CustomFilter : ICustomFilter
    {
        public CustomFilter(string resourceKey, IBatchAlterModel model, Func<Term, bool> whereClause)
        {
            ResourceKey = resourceKey;
            Model = model;
            WhereClause = whereClause;
        }

        public string ResourceKey { get; set; }
        public object Model { get; set; }
        public MulticastDelegate WhereClause { get; set; }

        public virtual void BeforeQuery() { }


        public void Dispose()
        {
            Model = null;
            WhereClause = null;
        }
    }
}
