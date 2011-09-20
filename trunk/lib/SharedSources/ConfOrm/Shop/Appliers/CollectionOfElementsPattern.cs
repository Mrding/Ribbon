using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.Shop.Appliers
{
	public class CollectionOfElementsPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public CollectionOfElementsPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		public virtual bool Match(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return false;
			}
			var manyType = memberType.DetermineCollectionElementType();
			if (manyType.IsGenericType && typeof(KeyValuePair<,>) == manyType.GetGenericTypeDefinition())
			{
				if (!IsDictionaryOfElements(subject, memberType)) return false;
			}

			return !domainInspector.IsManyToMany(subject.ReflectedType, manyType) && !domainInspector.IsOneToMany(subject.ReflectedType, manyType)
			       && !domainInspector.IsComponent(manyType);
		}

		protected virtual bool IsDictionaryOfElements(MemberInfo subject, Type memberType)
		{
			return ValueIsElement(memberType, subject);
		}

		protected virtual bool ValueIsElement(Type memberType, MemberInfo subject)
		{
			var mapValue = memberType.DetermineDictionaryValueType();
			return !domainInspector.IsManyToMany(subject.ReflectedType, mapValue) && !domainInspector.IsOneToMany(subject.ReflectedType, mapValue) && !domainInspector.IsComponent(mapValue);
		}
	}
}