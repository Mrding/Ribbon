using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ComponentPattern : IPattern<Type>
	{
		private readonly IDomainInspector domainInspector;

		private const BindingFlags FlattenHierarchyMembers =
			BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public ComponentPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region Implementation of IPattern<Type>

		public bool Match(Type subject)
		{
			return !subject.IsEnum && !subject.Namespace.StartsWith("System") /* hack */ && !domainInspector.IsEntity(subject)
			       &&
			       !subject.GetProperties(FlattenHierarchyMembers).Cast<MemberInfo>().Concat(
			       	subject.GetFields(FlattenHierarchyMembers)).Any(m => domainInspector.IsPersistentId(m));
		}

		#endregion
	}
}