using System;

namespace Luna.Common
{
    public interface ICustomFilter : IDisposable
    {
         string ResourceKey { get; set; }

        object Model { get; set; }

        MulticastDelegate WhereClause { get; set; }

        void BeforeQuery();
    }
}