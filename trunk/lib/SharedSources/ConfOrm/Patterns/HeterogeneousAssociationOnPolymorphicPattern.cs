using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class HeterogeneousAssociationOnPolymorphicPattern: IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public HeterogeneousAssociationOnPolymorphicPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public bool Match(MemberInfo subject)
		{
			// try to find the relation through PolymorphismResolver
			var memberType = subject.GetPropertyOrFieldType();
			if(typeof(IEnumerable).IsAssignableFrom(memberType) || memberType.Name.StartsWith("System"))
			{
				// short-cut
				return false;
			}
			var baseImplementors = domainInspector.GetBaseImplementors(memberType).ToArray();
			return baseImplementors.Length > 1 && baseImplementors.All(domainInspector.IsEntity);
		}
	}
}