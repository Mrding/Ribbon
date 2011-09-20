using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismOneToManyClassApplier : IPatternApplier<MemberInfo, IOneToManyMapper>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismOneToManyClassApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		#region IPatternApplier<MemberInfo,IOneToManyMapper> Members

		public bool Match(MemberInfo subject)
		{
			// apply only when there is just one solution
			Type elementType = subject.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
			var baseImplementors = domainInspector.GetBaseImplementors(elementType).ToArray();
			return baseImplementors.Length == 1 && !elementType.Equals(baseImplementors[0]);
		}

		public void Apply(MemberInfo subject, IOneToManyMapper applyTo)
		{
			Type elementType = subject.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
			applyTo.Class(domainInspector.GetBaseImplementors(elementType).Single());
		}

		#endregion
	}
}