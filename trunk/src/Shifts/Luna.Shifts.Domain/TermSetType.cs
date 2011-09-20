using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.UserTypes;
using Luna.Common;
namespace Luna.Shifts.Domain
{
    public class TermSetType : CollectionType, IUserCollectionType
    {
        public TermSetType(string role, string foreignKeyPropertyName, bool isEmbeddedInXML)
            : base(role, foreignKeyPropertyName, isEmbeddedInXML)
        {
        }

        public TermSetType()
            : base(string.Empty, string.Empty, false)
        {
        }

        public override Type ReturnedClass
        {
            get { return typeof(PersistentTermSet<Term>); }
        }

        #region IUserCollectionType Members

        public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            return new PersistentTermSet<Term>(session);
        }

        public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            return new PersistentTermSet<Term>(session, (ISet<Term>)collection);
        }

        public IEnumerable GetElements(object collection)
        {
            return ((IEnumerable)collection);
        }

        public bool Contains(object collection, object entity)
        {
            return ((ICollection<Term>)collection).Contains((Term)entity);
        }


        public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
        {
            return base.ReplaceElements(original, target, owner, copyCache, session);
        }

        public override object Instantiate(int anticipatedSize)
        {
            return new TermSet<Term>();
        }

        #endregion

        public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister,object key)
        {
            return new PersistentTermSet<Term>(session);
        }
    }
}
