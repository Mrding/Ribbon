using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.Shop.Appliers
{
	/// <summary>
	/// Match a generic collection when the relation between the container and the collection element is one-to-many.
	/// </summary>
	public class OneToManyPattern: IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public OneToManyPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		protected IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			Relation oneToMany = GetOneToMany(subject);
			return oneToMany != null && DomainInspector.IsOneToMany(oneToMany.From, oneToMany.To);
		}

		#endregion

		protected Relation GetOneToMany(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return null;
			}
			var manyType = memberType.DetermineCollectionElementType();
			if (manyType.IsGenericType && typeof(KeyValuePair<,>) == manyType.GetGenericTypeDefinition())
			{
				var dictionaryGenericArguments = manyType.GetGenericArguments();
				manyType = dictionaryGenericArguments[1];
			}
			var oneType = subject.ReflectedType;
			return new Relation(oneType, manyType);
		}
	}
}