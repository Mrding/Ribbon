namespace Luna.Common.Interfaces
{
    using System;

    public interface IIndexer
    {
        object GetItem(object index, Type indexType);
    }

    public interface IIntIndexer
    {
        object GetItem(int index);
    }

    public interface IDateIndexer
    {
        object GetItem(DateTime index);
    }

    public interface IIntIndexer<T> : IIntIndexer
    {
        T this[int index]
        {
            get;
        }
    }
    
    public interface IDateIndexer<T>
    {
        T this[DateTime index] { get; set; }
    }
}