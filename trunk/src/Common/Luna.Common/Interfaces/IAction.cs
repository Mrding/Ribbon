namespace Luna.Common
{
    public interface IAction
    {
        void Action();
    }

    public interface IAction<T>
    {
        void Action(T param);
    }

    public interface IAction<T1, T2>
    {
        void Action(T1 p1, T2 p2);
    }

    public interface IPredicate<T>
    {
        bool Predicate(T param);
    }

    public interface IPredicate<T1, T2>
    {
        bool Predicate(T1 p1, T2 p2);
    }

    public interface IFunc<TResult>
    {
        TResult Func();
    }

    public interface IFunc<T, TResult>
    {
        TResult Func(T param);
    }

    public interface IFunc<T1, T2, TResult>
    {
        TResult Func(T1 p1, T2 p2);
    }
}
