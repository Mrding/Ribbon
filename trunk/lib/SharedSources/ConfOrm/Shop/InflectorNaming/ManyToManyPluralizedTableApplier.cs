using System;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.InflectorNaming
{
	public class ManyToManyPluralizedTableApplier : AbstractManyToManyInCollectionTableApplier
	{
		private readonly IInflector inflector;

		public ManyToManyPluralizedTableApplier(IDomainInspector domainInspector, IInflector inflector)
			: base(domainInspector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		public IInflector Inflector
		{
			get { return inflector; }
		}

		public override string GetTableNameForRelation(Relation fromRelation, Relation toRelation)
		{
			return string.Format("{0}To{1}", inflector.Pluralize(fromRelation.From.Name), inflector.Pluralize(fromRelation.To.Name));
		}

		public override string GetTableNameForRelationOnProperty(RelationOn fromRelation, RelationOn toRelation)
		{
			var propertyOfRelarion = fromRelation.On.Name;
			var pluralizedTo = inflector.Pluralize(fromRelation.To.Name);
			if (propertyOfRelarion.Contains(pluralizedTo))
			{
				return inflector.Pluralize(fromRelation.From.Name) + propertyOfRelarion;
			}
			return inflector.Pluralize(fromRelation.From.Name) + propertyOfRelarion + pluralizedTo;
		}
	}
}