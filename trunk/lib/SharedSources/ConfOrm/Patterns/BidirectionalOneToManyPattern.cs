using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyPattern : IPattern<Relation>
	{
		private readonly IDomainInspector domainInspector;

		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		public BidirectionalOneToManyPattern(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		protected IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		#region Implementation of IPattern<Relation>

		public virtual bool Match(Relation subject)
		{
			// the 'from' represent the one-side, the 'to' represent the many-side
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			Type one = subject.From;
			Type many = subject.To;
			if (many.IsGenericType && typeof(KeyValuePair<,>) == many.GetGenericTypeDefinition())
			{
				var dictionaryGenericArguments = many.GetGenericArguments();
				many = dictionaryGenericArguments[1];
			}
			if (domainInspector.IsManyToMany(one, many))
			{
				return false;
			}

			return HasCollectionOf(one, many) && many.HasPublicPropertyOf(one, p=> DomainInspector.IsPersistentProperty(p));
		}

		#endregion

		protected bool HasCollectionOf(Type one, Type many)
		{
			foreach (Type propertyType in
				one.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType))
			{
				if (propertyType.IsGenericCollection())
				{
					List<Type> interfaces = propertyType.GetInterfaces().Where(t => t.IsGenericType).ToList();
					if (propertyType.IsInterface)
					{
						interfaces.Add(propertyType);
					}
					Type genericEnumerable = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
					if (genericEnumerable != null)
					{
						Type genericArgument = genericEnumerable.GetGenericArguments()[0];
						if (genericArgument == many)
						{
							return true;
						}
						if (genericArgument.IsGenericType && typeof(KeyValuePair<,>) == genericArgument.GetGenericTypeDefinition())
						{
							var dictionaryGenericArguments = genericArgument.GetGenericArguments();
							var valueType = dictionaryGenericArguments[1];
							if (valueType == many)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}