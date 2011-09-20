using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismManyToOneClassApplier : IPatternApplier<MemberInfo, IManyToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismManyToOneClassApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		public bool Match(MemberInfo subject)
		{
			// apply only when there is just one solution
			Type propertyOrFieldType = subject.GetPropertyOrFieldType();
			var baseImplementors = domainInspector.GetBaseImplementors(propertyOrFieldType).ToArray();
			return baseImplementors.Length == 1 && !propertyOrFieldType.Equals(baseImplementors[0]);
		}

		public void Apply(MemberInfo subject, IManyToOneMapper applyTo)
		{
			applyTo.Class(domainInspector.GetBaseImplementors(subject.GetPropertyOrFieldType()).Single());
		}
	}
}