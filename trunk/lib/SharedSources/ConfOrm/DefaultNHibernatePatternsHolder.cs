using ConfOrm.Patterns;

namespace ConfOrm
{
	public class DefaultNHibernatePatternsHolder : EmptyPatternsHolder
	{
		public DefaultNHibernatePatternsHolder(IDomainInspector domainInspector, IExplicitDeclarationsHolder explicitDeclarations)
		{
			Poids.Add(new PoIdPattern());
			Sets.Add(new SetCollectionPattern());
			Bags.Add(new BagCollectionPattern());
			Lists.Add(new ListCollectionPattern(domainInspector));
			Arrays.Add(new ArrayCollectionPattern());
			Components.Add(new ComponentPattern(domainInspector));
			Dictionaries.Add(new DictionaryCollectionPattern());

			PoidStrategies.Add(new HighLowPoidPattern());
			PoidStrategies.Add(new GuidOptimizedPoidPattern());

			PersistentPropertiesExclusions.Add(new ReadOnlyPropertyPattern());
			ManyToOneRelations.Add(new OneToOneUnidirectionalToManyToOnePattern(domainInspector, explicitDeclarations));
			ManyToOneRelations.Add(new PolymorphicManyToOnePattern(domainInspector));
			OneToManyRelations.Add(new PolymorphicOneToManyPattern(domainInspector));
			HeterogeneousAssociations.Add(new HeterogeneousAssociationOnPolymorphicPattern(domainInspector));
		}
	}
}