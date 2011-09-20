using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyTableApplier : BidirectionalManyToManyPattern,
	                                                   IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		private readonly IDomainInspector domainInspector;

		public BidirectionalManyToManyTableApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<MemberInfo,ICollectionPropertiesMapper> Members

		public override bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			return !IsCircularManyToMany(subject) && base.Match(subject);
		}

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			Relation relation = GetRelation(subject);
			string tableName = GetTableNameForRelation(relation.From, relation.To);
			applyTo.Table(tableName);
		}

		#endregion

		protected virtual string GetTableNameForRelation(Type fromMany, Type toMany)
		{
			bool fromIsMaster = domainInspector.IsMasterManyToMany(fromMany, toMany);
			bool toIsMaster = domainInspector.IsMasterManyToMany(toMany, fromMany);
			string[] names;
			if (fromIsMaster == toIsMaster)
			{
				names = (from t in (new[] {fromMany, toMany}) orderby t.Name select t.Name).ToArray();
			}
			else
			{
				names = fromIsMaster ? new[] {fromMany.Name, toMany.Name} : new[] {toMany.Name, fromMany.Name};
			}
			return GetTableNameForRelation(names);
		}

		protected virtual string GetTableNameForRelation(string[] names)
		{
			return names[0] + names[1];
		}
	}
}