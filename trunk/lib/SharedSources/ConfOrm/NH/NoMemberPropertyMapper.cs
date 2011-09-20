using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class NoMemberPropertyMapper : IAccessorPropertyMapper
	{
		public void Access(Accessor accessor) { }

		public void Access(Type accessorType) { }
	}
}