using System;
using System.Linq;
using System.Reflection;
using ConfOrm.NH;
namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match when a component is used more than one time in the same class.
	/// </summary>
	public class ComponentMultiUsagePattern: IPattern<PropertyPath>
	{
		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null || subject.PreviousPath == null || subject.LocalMember == null)
			{
				return false;
			}
			Type componentType = subject.LocalMember.DeclaringType;
			Type componentContainerType = subject.PreviousPath.LocalMember.DeclaringType; // TODO: should be recursive

			return CountPropertyOf(componentContainerType, componentType) > 1;
		}

		#endregion

		protected int CountPropertyOf(Type componentContainer, Type component)
		{
			return componentContainer.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.PropertyType).Count(t => t == component);
		}
	}
}