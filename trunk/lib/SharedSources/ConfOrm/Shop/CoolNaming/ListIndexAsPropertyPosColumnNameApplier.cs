using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class ListIndexAsPropertyPosColumnNameApplier: IPatternApplier<PropertyPath, IListPropertiesMapper>
	{
		public bool Match(PropertyPath subject)
		{
			return subject != null;
		}

		public void Apply(PropertyPath subject, IListPropertiesMapper applyTo)
		{
			applyTo.Index(lim=> lim.Column(GetIndexColumnName(subject)));
		}

		protected virtual string GetIndexColumnName(PropertyPath subject)
		{
			return subject.ToColumnName() + "Pos";
		}
	}
}