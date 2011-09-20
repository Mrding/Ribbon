using System;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class SubclassDiscriminatorValueAsEnumValueApplier<TRootEntity, TEnum> : IPatternApplier<Type, ISubclassAttributesMapper>
		where TRootEntity : class
		where TEnum : struct
	{
		private readonly string[] enumsNames;

		public SubclassDiscriminatorValueAsEnumValueApplier()
		{
			if (!typeof (TEnum).IsEnum)
			{
				throw new NotSupportedException("The TEnum type parameter should be an enum.");
			}
			enumsNames = Enum.GetNames(typeof (TEnum));
		}

		#region IPatternApplier<Type,ISubclassAttributesMapper> Members

		public bool Match(Type subject)
		{
			return typeof(TRootEntity).IsAssignableFrom(subject);
		}

		public void Apply(Type subject, ISubclassAttributesMapper applyTo)
		{
			string className = subject.Name;
			if (Array.IndexOf(enumsNames, className) >= 0)
			{
				applyTo.DiscriminatorValue(EnumUtil.ParseGettingUnderlyingValue(typeof (TEnum), className));
			}
			else
			{
				throw new ArgumentException("Canot find the discriminator value for the class " + subject.FullName +
				                            " using the enum " + typeof (TEnum).FullName);
			}
		}

		#endregion
	}
}