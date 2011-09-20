using System;
using ConfOrm.NH;

namespace ConfOrm.Shop.Appliers
{
	public class ComponentMemberDeepPathPattern: IPattern<PropertyPath>
	{
		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null || subject.PreviousPath == null || subject.LocalMember == null)
			{
				return false;
			}
			if (subject.PreviousPath.LocalMember.GetPropertyOrFieldType().IsGenericCollection())
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}