using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyColumnApplier : ManyToManyPattern, IPatternApplier<PropertyPath, IManyToManyMapper>
	{
		public ManyToManyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			return Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,IManyToManyMapper>

		public void Apply(PropertyPath subject, IManyToManyMapper applyTo)
		{
			applyTo.Column(GetColumnNameForRelation(subject));
		}

		#endregion

		protected virtual string GetColumnNameForRelation(PropertyPath subject)
		{
			var relation = GetRelation(subject.LocalMember);
			var fromMany = relation.From;
			if (!DomainInspector.IsEntity(fromMany))
			{
				fromMany = subject.GetContainerEntity(DomainInspector);
			}
			var toMany = relation.To;
			var baseColumnName = toMany.Name + "Id";
			if (fromMany != toMany)
			{
				return baseColumnName;
			}
			return subject.ToColumnName() + baseColumnName;
		}
	}
}