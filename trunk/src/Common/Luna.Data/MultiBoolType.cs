using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Luna.Data
{
    public class MultiBoolType : ICompositeUserType
    {
        public System.Type ReturnedClass
        {
            get { return typeof(bool[]); }
        }

        bool ICompositeUserType.Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            bool[] lhs = (bool[])x;
            bool[] rhs = (bool[])y;

            return lhs[0].Equals(rhs[0]) && lhs[1].Equals(rhs[1]) && lhs[2].Equals(rhs[2]) && lhs[3].Equals(rhs[3]) &&
                   lhs[4].Equals(rhs[4]) && lhs[5].Equals(rhs[5]) && lhs[6].Equals(rhs[6]);
        }

        public int GetHashCode(object x)
        {
            unchecked
            {
                bool[] a = (bool[])x;
                var result = a[0].GetHashCode();
                result = (result * 397) ^ a[1].GetHashCode(); result = (result * 397) ^ a[2].GetHashCode();
                result = (result * 397) ^ a[3].GetHashCode(); result = (result * 397) ^ a[4].GetHashCode();
                result = (result * 397) ^ a[5].GetHashCode(); result = (result * 397) ^ a[6].GetHashCode();
                return result;
            }
        }

        public Object DeepCopy(Object x)
        {
            if (x == null) return null;
            bool[] result = new bool[7];
            bool[] input = (bool[])x;
            result[0] = input[0];
            result[1] = input[1];
            result[2] = input[2];
            result[3] = input[3];
            result[4] = input[4];
            result[5] = input[5];
            result[6] = input[6];
            return result;
        }

        public bool IsMutable
        {
            get { return true; }
        }

        public Object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, Object owner)
        {
            bool a1 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[0], session, owner);
            bool a2 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[1], session, owner);
            bool a3 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[2], session, owner);
            bool a4 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[3], session, owner);
            bool a5 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[4], session, owner);
            bool a6 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[5], session, owner);
            bool a7 = (bool)NHibernateUtil.Boolean.NullSafeGet(rs, names[6], session, owner);

            return new bool[] { a1, a2, a3, a4, a5, a6, a7 };
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            bool[] bools = (value == null) ? new bool[7] : (bool[])value;

            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[0], index, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[1], index + 1, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[2], index + 2, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[3], index + 3, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[4], index + 4, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[5], index + 5, session);
            NHibernateUtil.Boolean.NullSafeSet(cmd, bools[6], index + 6, session);
        }

        //public void NullSafeSet(IDbCommand st, Object value, int index, ISessionImplementor session)
        //{
            
        //}

        public string[] PropertyNames
        {
            get { return new string[] { "s1", "s2", "s3", "s4", "s5", "s6", "s7" }; }
        }

        public IType[] PropertyTypes
        {
            get { return new IType[] { NHibernateUtil.Boolean, NHibernateUtil.Boolean, NHibernateUtil.Boolean, NHibernateUtil.Boolean, NHibernateUtil.Boolean, NHibernateUtil.Boolean, NHibernateUtil.Boolean }; }
        }

        public Object GetPropertyValue(Object component, int property)
        {
            return ((bool[])component)[property];
        }

        public void SetPropertyValue(
            Object component,
            int property,
            Object value)
        {
            ((bool[])component)[property] = (bool)value;
        }

        public object Assemble(
            object cached,
            ISessionImplementor session,
            object owner)
        {
            return DeepCopy(cached);
        }

        public object Disassemble(Object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            return DeepCopy(original);
        }
    }
}