using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyColumnApplier: BidirectionalManyToManyPattern, IPatternApplier<MemberInfo, IManyToManyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IManyToManyMapper>

		public void Apply(MemberInfo subject, IManyToManyMapper applyTo)
		{
			var relation = GetRelation(subject);
			applyTo.Column(GetColumnName(subject, relation));
		}

		#endregion

		protected virtual string GetColumnName(MemberInfo subject, Relation relation)
		{
			if (relation.From == relation.To)
			{
				// circular many-to-many
				return subject.Name;
			}
			else
			{
				return KeyMapper.DefaultColumnName(relation.To);
			}
		}
	}
}