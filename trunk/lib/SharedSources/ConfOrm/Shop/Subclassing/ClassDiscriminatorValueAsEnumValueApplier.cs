using System;
using System.Linq;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class ClassDiscriminatorValueAsEnumValueApplier<TRootEntity, TEnum> : IPatternApplier<Type, IClassAttributesMapper>
		where TRootEntity: class
		where TEnum: struct
	{
		private readonly IDomainInspector domainInspector;
		private readonly string[] enumsNames;
		private readonly int indexOfUnknow;

		public ClassDiscriminatorValueAsEnumValueApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if(!typeof(TEnum).IsEnum)
			{
				throw new NotSupportedException("The TEnum type parameter should be an enum.");
			}
			this.domainInspector = domainInspector;
			enumsNames = Enum.GetNames(typeof(TEnum));
			indexOfUnknow = Array.IndexOf(enumsNames.Select(x => x.ToLowerInvariant()).ToArray(), "unknown");
		}

		public bool Match(Type subject)
		{
			return domainInspector.IsTablePerClassHierarchy(subject) && typeof(TRootEntity).IsAssignableFrom(subject);
		}

		public void Apply(Type subject, IClassAttributesMapper applyTo)
		{
			var className = subject.Name;
			if (Array.IndexOf(enumsNames, className) >= 0)
			{
				applyTo.DiscriminatorValue(EnumUtil.ParseGettingUnderlyingValue(typeof(TEnum), className));
			}
			else if (indexOfUnknow >= 0)
			{
				applyTo.DiscriminatorValue(EnumUtil.ParseGettingUnderlyingValue(typeof(TEnum), enumsNames[indexOfUnknow]));
			}
			else
			{
				throw new ArgumentException("Canot find the discriminator value for the class " + subject.FullName + " using the enum " + typeof(TEnum).FullName);
			}
		}
	}
}