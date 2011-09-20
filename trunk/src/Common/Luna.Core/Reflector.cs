using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Luna.Core
{
    public delegate void Proc();
    public delegate void Proc<T1>(T1 arg1);
    public delegate void Proc<T1, T2>(T1 arg1, T2 args);
    public delegate void Proc<T1, T2, T3>(T1 arg1, T2 args, T3 arg3);
    public delegate void Proc<T1, T2, T3, T4>(T1 arg1, T2 args, T3 arg3, T4 arg4);
    public delegate void Proc<T1, T2, T3, T4, T5>(T1 arg1, T2 args, T3 arg3, T4 arg4, T5 arg5);
    public delegate R Func<R>();
    public delegate R Func<T1, R>(T1 arg1);
    public delegate R Func<T1, T2, R>(T1 arg1, T2 args);
    public delegate R Func<T1, T2, T3, R>(T1 arg1, T2 args, T3 arg3);
    public delegate R Func<T1, T2, T3, T4, R>(T1 arg1, T2 args, T3 arg3, T4 arg4);
    public delegate R Func<T1, T2, T3, T4, T5, R>(T1 arg1, T2 args, T3 arg3, T4 arg4, T5 arg5);

    public class Reflector
    {
        private object target;

        public Reflector(object obj)
        {
            target = obj ;
        }

        public object Target
        {
            get { return target; }
        }

        public T As<T>()
        {
            return (T)target;
        }

        public T Property<T>(string name)
        {
            PropertyInfo pi = target.GetType().GetProperty(name);

            if (null != pi && pi.CanRead)
            {
                object value = pi.GetValue(target, null);

                if (null != value)
                {
                    return (T)value;
                }
            }

            return default(T);
        }

        public T SetProperty<T>(string name, T value)
        {
            PropertyInfo pi = target.GetType().GetProperty(name, typeof(T));

            if (null != pi && pi.CanWrite)
            {
                pi.SetValue(target, value, null);
            }

            return value;
        }

        public Proc Proc(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, Type.EmptyTypes);

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc), target, mi.Name, false) as Proc;
            }

            return null;
        }

        public Proc<T> Proc<T>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc<T>), target, mi.Name, false) as Proc<T>;
            }

            return null;
        }

        public Proc<T1, T2> Proc<T1, T2>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1), typeof(T2) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc<T1, T2>), target, mi.Name, false) as Proc<T1, T2>;
            }

            return null;
        }

        public Proc<T1, T2, T3> Proc<T1, T2, T3>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1), typeof(T2), typeof(T3) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc<T1, T2, T3>), target, mi.Name, false) as Proc<T1, T2, T3>;
            }

            return null;
        }

        public Proc<T1, T2, T3, T4> Proc<T1, T2, T3, T4>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc<T1, T2, T3, T4>), target, mi.Name, false) as Proc<T1, T2, T3, T4>;
            }

            return null;
        }

        public Proc<T1, T2, T3, T4, T5> Proc<T1, T2, T3, T4, T5>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Proc<T1, T2, T3, T4, T5>), target, mi.Name, false) as Proc<T1, T2, T3, T4, T5>;
            }

            return null;
        }

        public Func<R> Func<R>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, Type.EmptyTypes);

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Func<R>), target, mi.Name, false) as Func<R>;
            }

            return null;
        }

        public Func<T1, R> Func<T1, R>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Func<T1, R>), target, mi.Name, false) as Func<T1, R>;
            }

            return null;
        }

        public Func<T1, T2, R> Func<T1, T2, R>(string name)
        {
            MethodInfo mi = target.GetType().GetMethod(name, new Type[] { typeof(T1), typeof(T2) });

            if (null != mi)
            {
                return Delegate.CreateDelegate(typeof(Func<T1, T2, R>), target, mi.Name, false) as Func<T1, T2, R>;
            }

            return null;
        }
    }
}
