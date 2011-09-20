using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.Shop.Appliers
{
	public class ManyToManyPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public ManyToManyPattern(IDomainInspector domainInspector)
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
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			Type propertyType = subject.GetPropertyOrFieldType();
			if (!propertyType.IsGenericCollection())
			{
				// can't determine relation for a no generic collection
				return false;
			}

			Type fromMany = subject.DeclaringType;
			Type cadidateToMany = propertyType.DetermineCollectionElementType();
			if (cadidateToMany.IsGenericType && typeof (KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
			{
				// many-to-many on map
				Type[] dictionaryGenericArguments = cadidateToMany.GetGenericArguments();
				return domainInspector.IsManyToMany(fromMany, dictionaryGenericArguments[1])
				       || domainInspector.IsManyToMany(fromMany, dictionaryGenericArguments[0]);
			}
			else
			{
				// many-to-many on plain collection
				Type toMany = cadidateToMany;
				return domainInspector.IsManyToMany(fromMany, toMany);
			}
		}

		#endregion

		protected Relation GetRelation(MemberInfo subject)
		{
			Type propertyType = subject.GetPropertyOrFieldType();

			Type fromMany = subject.DeclaringType;
			Type cadidateToMany = propertyType.DetermineCollectionElementType();
			if (cadidateToMany.IsGenericType && typeof (KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
			{
				// many-to-many on map
				Type[] dictionaryGenericArguments = cadidateToMany.GetGenericArguments();
				return domainInspector.IsManyToMany(fromMany, dictionaryGenericArguments[1])
				       	? new Relation(fromMany, dictionaryGenericArguments[1])
				       	: new Relation(fromMany, dictionaryGenericArguments[0]);
			}
			else
			{
				// many-to-many on plain collection
				Type toMany = cadidateToMany;
				return new Relation(fromMany, toMany);
			}
		}
	}
}