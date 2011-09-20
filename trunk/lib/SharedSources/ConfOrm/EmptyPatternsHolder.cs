using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public class EmptyPatternsHolder : IPatternsHolder
	{
		private readonly List<IPattern<MemberInfo>> arrayPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> bagPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPatternValueGetter<Relation, Cascade?>> cascadePatterns =
			new List<IPatternValueGetter<Relation, Cascade?>>();

		private readonly List<IPattern<Type>> componentPatterns = new List<IPattern<Type>>();
		private readonly List<IPattern<MemberInfo>> dictionaryPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> listPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPattern<MemberInfo>> persistentPropertyExclusionPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> poidPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> poidStrategyPatterns =
			new List<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>>();

		private readonly List<IPattern<MemberInfo>> setPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<Relation>> manyToOneRelations = new List<IPattern<Relation>>();
		private readonly List<IPattern<MemberInfo>> versionPropertyPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> heterogeneousAssociations = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<Relation>> oneToManyRelations= new List<IPattern<Relation>>();

		#region Implementation of IPatternsHolder

		public ICollection<IPattern<MemberInfo>> Poids
		{
			get { return poidPatterns; }
		}

		public ICollection<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> PoidStrategies
		{
			get { return poidStrategyPatterns; }
		}

		public ICollection<IPattern<Type>> Components
		{
			get { return componentPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Sets
		{
			get { return setPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Bags
		{
			get { return bagPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Lists
		{
			get { return listPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Arrays
		{
			get { return arrayPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Dictionaries
		{
			get { return dictionaryPatterns; }
		}

		public ICollection<IPatternValueGetter<Relation, Cascade?>> Cascades
		{
			get { return cascadePatterns; }
		}

		public ICollection<IPattern<MemberInfo>> PersistentPropertiesExclusions
		{
			get { return persistentPropertyExclusionPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Versions
		{
			get { return versionPropertyPatterns; }
		}

		public ICollection<IPattern<Relation>> ManyToOneRelations
		{
			get { return manyToOneRelations;}
		}

		public ICollection<IPattern<Relation>> OneToManyRelations
		{
			get { return oneToManyRelations; }
		}

		public ICollection<IPattern<MemberInfo>> HeterogeneousAssociations
		{
			get { return heterogeneousAssociations; }
		}

		#endregion
	}
}