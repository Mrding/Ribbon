using System;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyInCollectionTableApplier : AbstractManyToManyInCollectionTableApplier
	{
		public ManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public override string GetTableNameForRelation(Relation fromRelation, Relation toRelation)
		{
			return string.Format("{0}To{1}", fromRelation.From.Name, fromRelation.To.Name);
		}

		public override string GetTableNameForRelationOnProperty(RelationOn fromRelation, RelationOn toRelation)
		{
			if(fromRelation.DeclaredAs != toRelation.DeclaredAs)
			{
				return fromRelation.From.Name + fromRelation.On.Name;
			}
			else
			{
				return fromRelation.From.Name + fromRelation.On.Name + toRelation.From.Name + toRelation.On.Name;				
			}
		}
	}
}