using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.NamingAppliers
{
	public abstract class AbstractManyToManyInCollectionTableApplier : ManyToManyPattern,
	                                                  IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		protected AbstractManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			bool isManyToMany = Match(subject.LocalMember);
			if (isManyToMany)
			{
				Type propertyType = subject.LocalMember.GetPropertyOrFieldType();
				Type cadidateToMany = propertyType.DetermineCollectionElementType();
				if (cadidateToMany.IsGenericType && typeof (KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
				{
					// does not match for on Dictionary
					// A Dictionary may have a relation with 3 entities, how manage the table-name will be matter of another applier
					return false;
				}
			}
			return isManyToMany;
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetManyToManyTableName(subject));
		}

		#endregion

		protected virtual string GetManyToManyTableName(PropertyPath subject)
		{
			var manyToManyRelation = GetRelation(subject.LocalMember);
			var fromMany = manyToManyRelation.From;
			var toMany = manyToManyRelation.To;
			bool fromIsMaster = DomainInspector.IsMasterManyToMany(fromMany, toMany);
			bool toIsMaster = DomainInspector.IsMasterManyToMany(toMany, fromMany);
			fromMany = subject.GetContainerEntity(DomainInspector);
			var explicitBidirectionalMember = DomainInspector.GetBidirectionalMember(fromMany, subject.LocalMember, toMany);
			if (explicitBidirectionalMember == null)
			{
				Relation[] twoRelations = GetTwoRelation(fromMany, fromIsMaster, toMany, toIsMaster);
				return GetTableNameForRelation(twoRelations[0], twoRelations[1]);
			}
			else
			{
				RelationOn[] twoRelationOn = GetTwoRelationOn(fromMany, fromIsMaster, subject, toMany, toIsMaster, explicitBidirectionalMember);
				return GetTableNameForRelationOnProperty(twoRelationOn[0], twoRelationOn[1]);
			}
		}

		protected virtual RelationOn[] GetTwoRelationOn(Type fromMany, bool fromIsMaster, PropertyPath fromMember, Type toMany, bool toIsMaster, MemberInfo toMember)
		{
			var twoRelationOn = new RelationOn[2];
			if (fromIsMaster == toIsMaster)
			{
				var relationsOn = new[] {new RelationOn(fromMany, fromMember.LocalMember, toMany), new RelationOn(toMany, toMember, fromMany)};
				twoRelationOn = (from relationOn in relationsOn orderby relationOn.From.Name select relationOn).ToArray();
			}
			else
			{
				if (fromIsMaster)
				{
					twoRelationOn[0] = new RelationOn(fromMany, fromMember.LocalMember, toMany, Declared.Explicit);
					twoRelationOn[1] = new RelationOn(toMany, toMember, fromMany, Declared.Implicit);
				}
				else
				{
					twoRelationOn[0] = new RelationOn(toMany, toMember, fromMany, Declared.Explicit);
					twoRelationOn[1] = new RelationOn(fromMany, fromMember.LocalMember, toMany, Declared.Implicit);
				}
			}
			return twoRelationOn;
		}

		protected virtual Relation[] GetTwoRelation(Type fromMany, bool fromIsMaster, Type toMany, bool toIsMaster)
		{
			var twoRelations = new Relation[2];
			if (fromIsMaster == toIsMaster)
			{
				var relations = new[] { new Relation(fromMany, toMany), new Relation(toMany, fromMany) };
				twoRelations = (from relation in relations orderby relation.From.Name select relation).ToArray();
			}
			else
			{
				if (fromIsMaster)
				{
					twoRelations[0] = new Relation(fromMany, toMany, Declared.Explicit);
					twoRelations[1] = new Relation(toMany, fromMany, Declared.Implicit);
				}
				else
				{
					twoRelations[0] = new Relation(toMany, fromMany, Declared.Explicit);
					twoRelations[1] = new Relation(fromMany, toMany, Declared.Implicit);
				}
			}
			return twoRelations;
		}

		public abstract string GetTableNameForRelation(Relation fromRelation, Relation toRelation);
		public abstract string GetTableNameForRelationOnProperty(RelationOn fromRelation, RelationOn toRelation);
	}
}