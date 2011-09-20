using System;
using System.Collections;

namespace Luna.Common
{
    public interface IEntityFactory
    {
        T Create<T>();

        T Create<T>(IDictionary args);

        T Create<T>(object source);

        T Create<T>(Type type);
    }
}