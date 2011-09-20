using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Luna.Shifts.Domain
{
    public class OccupationSetType : CollectionType, IUserCollectionType
    {
        public OccupationSetType(string role, string foreignKeyPropertyName, bool isEmbeddedInXML)
            : base(role, foreignKeyPropertyName, isEmbeddedInXML)
        {
        }

        public OccupationSetType()
            : base(string.Empty, string.Empty, false)
        {
        }

        public override Type ReturnedClass
        {
            get { return typeof(PersistentTermSet<Occupation>); }
        }

        #region IUserCollectionType Members

        public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            return new PersistentTermSet<Occupation>(session);
        }

        public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            return new PersistentTermSet<Occupation>(session, (Iesi.Collections.Generic.ISet<Occupation>)collection);
        }

        public IEnumerable GetElements(object collection)
        {
            return ((IEnumerable)collection);
        }

        public bool Contains(object collection, object entity)
        {
            return ((ICollection<Occupation>)collection).Contains((Occupation)entity);
        }


        public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner,
                                      IDictionary copyCache, ISessionImplementor session)
        {
            return base.ReplaceElements(original, target, owner, copyCache, session);
        }

        public override object Instantiate(int anticipatedSize)
        {
            return new TermSet<Occupation>();
        }

        #endregion

        public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister,
                                                          object key)
        {
            return new PersistentTermSet<Occupation>(session);
        }
    }
}