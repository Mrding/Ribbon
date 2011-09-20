using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match with multiple collection of the same type and uses the property name to disambiguates the collection key-column.
	/// </summary>
	/// <remarks>
	/// This applier is reflection-expensive. Perhaps would be better the usage of the property-path always.
	/// Because reflection-expensive, and because you can use a specific where clause to disambiguates collection elements, 
	/// this applier is not part of <see cref="DefaultPatternsAppliersHolder"/> (you have to use it explicitly).
	/// </remarks>
	public class UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier : IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		private readonly HashSet<Relation> relationsWithMultipleCollections = new HashSet<Relation>();

		public UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			DomainInspector = domainInspector;
		}

		protected IDomainInspector DomainInspector { get; set; }

		protected HashSet<Relation> RelationsWithMultipleCollections
		{
			get { return relationsWithMultipleCollections; }
		}

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				return false;
			}
			var elementType = subject.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
			if(elementType == null)
			{
				return false;
			}
			var collectionOwner = subject.GetContainerEntity(DomainInspector);
			if (relationsWithMultipleCollections.Contains(new Relation(collectionOwner, elementType)))
			{
				// early match minimizing reflection; we already know the relation
				return true;
			}
			if (!IsUnidirectionalOneToMany(subject.LocalMember.DeclaringType, elementType))
			{
				return false;
			}
			var hasMultipleCollection = HasMultipleCollectionOf(collectionOwner, elementType);
			if (hasMultipleCollection)
			{
				relationsWithMultipleCollections.Add(new Relation(collectionOwner, elementType));
			}
			return hasMultipleCollection;
		}

		private bool IsUnidirectionalOneToMany(Type from, Type to)
		{
			return DomainInspector.IsOneToMany(from, to) && to.GetFirstPropertyOfType(from, p=> DomainInspector.IsPersistentProperty(p)) == null;
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km=> km.Column(GetColumnName(subject)));
		}

		/// <summary>
		/// Check if the <paramref name="collectionOwner"/> has more than one collection of the same entity-type.
		/// </summary>
		/// <param name="collectionOwner">The class containing the collection.</param>
		/// <param name="elementType">The type of the element of the generic collection (is an entity for sure).</param>
		/// <returns>True when the <paramref name="collectionOwner"/> contains more than one collection of the same <paramref name="elementType"/>.</returns>
		/// <remarks>
		/// Override this method if you want speed-up the pattern avoiding the usage of reflection in certain cases where you know you have a double usage.
		/// A good way is inheriting this applier and adding your well-known relations to <see cref="RelationsWithMultipleCollections"/> directly in the constructor.
		/// </remarks>
		protected virtual bool HasMultipleCollectionOf(Type collectionOwner, Type elementType)
		{
			int collectionCount = 0;
			return HasMultipleCollectionOf(collectionOwner, elementType, ref collectionCount);
		}

		protected bool HasMultipleCollectionOf(Type collectionOwner, Type elementType, ref int collectionCount)
		{
			foreach (var propertyType in collectionOwner.GetProperties(PublicPropertiesOfClassHierarchy).Where(p=> DomainInspector.IsPersistentProperty(p)).Select(p => p.PropertyType))
			{
				if (!propertyType.Equals(elementType) && DomainInspector.IsComponent(propertyType))
				{
					if (HasMultipleCollectionOf(propertyType, elementType, ref collectionCount))
					{
						return true;
					}
				}
				else
				{
					var propertyElementType = propertyType.DetermineCollectionElementOrDictionaryValueType();
					if (elementType.Equals(propertyElementType))
					{
						collectionCount++;
					}
				}
				if(collectionCount > 1)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual string GetColumnName(PropertyPath subject)
		{
			return GetBaseColumnName(subject) + "_key";
		}

		protected virtual string GetBaseColumnName(PropertyPath subject)
		{
			return subject.GetContainerEntity(DomainInspector).Name + subject.ToColumnName();
		}
	}
}