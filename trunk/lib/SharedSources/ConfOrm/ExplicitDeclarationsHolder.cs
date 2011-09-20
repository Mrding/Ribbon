using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public class ExplicitDeclarationsHolder: IExplicitDeclarationsHolder
	{
		private readonly HashSet<Type> domainClasses = new HashSet<Type>();
		private readonly HashSet<Type> rootEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassHierarchyEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerConcreteClassEntities = new HashSet<Type>();
		private readonly HashSet<Type> classExclusions = new HashSet<Type>();
		private readonly HashSet<Relation> manyToOneRelations = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToManyRelations = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToOneRelations = new HashSet<Relation>();
		private readonly HashSet<Relation> manyToManyRelations = new HashSet<Relation>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
		private readonly HashSet<Type> components = new HashSet<Type>();
		private readonly Dictionary<Relation, Cascade> cascades = new Dictionary<Relation, Cascade>();
		private readonly HashSet<MemberInfo> persistentProperties = new HashSet<MemberInfo>();
		private readonly HashSet<Type> complexTypes = new HashSet<Type>();
		private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> complexTypeMembers = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> versions = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> heterogeneousAssociations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> naturalIds = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> exclusionProperties = new HashSet<MemberInfo>();
		private readonly IDictionary<RelationOn, MemberInfo> bidirectionalMembers = new Dictionary<RelationOn, MemberInfo>();

		public HashSet<Type> DomainClasses
		{
			get { return domainClasses; }
		}

		public ICollection<Type> RootEntities
		{
			get { return rootEntities; }
		}

		public ICollection<Type> TablePerClassEntities
		{
			get { return tablePerClassEntities; }
		}

		public ICollection<Type> TablePerClassHierarchyEntities
		{
			get { return tablePerClassHierarchyEntities; }
		}

		public ICollection<Type> TablePerConcreteClassEntities
		{
			get { return tablePerConcreteClassEntities; }
		}

		public HashSet<Type> ClassExclusions
		{
			get { return classExclusions; }
		}

		public ICollection<Relation> ManyToOneRelations
		{
			get { return manyToOneRelations; }
		}

		public ICollection<Relation> OneToManyRelations
		{
			get { return oneToManyRelations; }
		}

		public ICollection<Relation> OneToOneRelations
		{
			get { return oneToOneRelations; }
		}

		public ICollection<Relation> ManyToManyRelations
		{
			get { return manyToManyRelations; }
		}

		public ICollection<MemberInfo> Sets
		{
			get { return sets; }
		}

		public ICollection<MemberInfo> Bags
		{
			get { return bags; }
		}

		public ICollection<MemberInfo> Lists
		{
			get { return lists; }
		}

		public ICollection<MemberInfo> Arrays
		{
			get { return arrays; }
		}

		public ICollection<MemberInfo> Dictionaries
		{
			get { return dictionaries; }
		}

		public ICollection<Type> Components
		{
			get { return components; }
		}

		public IDictionary<Relation, Cascade> Cascades
		{
			get { return cascades; }
		}

		public ICollection<MemberInfo> PersistentProperties
		{
			get { return persistentProperties; }
		}

		public ICollection<Type> ComplexTypes
		{
			get { return complexTypes; }
		}

		public ICollection<MemberInfo> ComplexTypeMembers
		{
			get { return complexTypeMembers; }
		}

		public ICollection<MemberInfo> Poids
		{
			get { return poids; }
		}

		public ICollection<MemberInfo> VersionProperties
		{
			get { return versions; }
		}

		public ICollection<MemberInfo> HeterogeneousAssociations
		{
			get { return heterogeneousAssociations; }
		}

		public ICollection<MemberInfo> NaturalIds
		{
			get { return naturalIds; }
		}

		public ICollection<MemberInfo> ExclusionProperties
		{
			get { return exclusionProperties; }
		}

		public IDictionary<RelationOn, MemberInfo> BidirectionalMembers
		{
			get { return bidirectionalMembers; }
		}
	}
}