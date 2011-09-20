using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.NamingAppliers
{
	public class ComponentPropertyColumnNameApplier : ComponentMemberDeepPathPattern, IPatternApplier<PropertyPath, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<PropertyPath,IPropertyMapper>

		public void Apply(PropertyPath subject, IPropertyMapper applyTo)
		{
			var pathToMap = DepureFirstLevelIfCollection(subject);
			applyTo.Column(pathToMap.ToColumnName());
		}

		#endregion

		protected virtual PropertyPath DepureFirstLevelIfCollection(PropertyPath source)
		{
			// when the component is used as elements of a collection, the name of the property representing
			// the collection itself may be ignored since each collection will have its own table.
			// The method is virtual because in some cases may be a problem.
			const int penultimateOffset = 2;
			if(!source.GetRootMember().GetPropertyOrFieldType().IsGenericCollection())
			{
				return source;
			}
			var paths = source.InverseProgressivePath().ToArray();
			return paths[paths.Length - penultimateOffset];
		}
	}
}