using System;
using System.Collections.Generic;

namespace ConfOrm
{
	public static class EnumUtil
	{
		private static readonly Dictionary<Type, Func<object, object>> Converters;
		static EnumUtil()
		{
			//The approved types for an enum are byte, sbyte, short, ushort, int, uint, long, or ulong.
			Converters = new Dictionary<Type, Func<object, object>>
			             	{
			             		{typeof (int), x => Convert.ToInt32(x)},
			             		{typeof (short), x => Convert.ToInt16(x)},
			             		{typeof (long), x => Convert.ToInt64(x)},
			             		{typeof (byte), x => Convert.ToByte(x)},
			             		{typeof (sbyte), x => Convert.ToSByte(x)},
			             		{typeof (uint), x => Convert.ToUInt32(x)},
			             		{typeof (ushort), x => Convert.ToUInt16(x)},
			             		{typeof (ulong), x => Convert.ToUInt64(x)}
			             	};
		}

		public static object ParseGettingUnderlyingValue(Type enumType, string enumValueName)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (enumValueName == null)
			{
				throw new ArgumentNullException("enumValueName");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum.");
			}
			Func<object, object> converter = Converters[Enum.GetUnderlyingType(enumType)];
			return converter(Enum.Parse(enumType, enumValueName));
		}
	}
}