using System;
using System.Reflection;

namespace ConfOrm.Shop.Appliers
{
	public class CollectionOfComponentsPattern: IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public CollectionOfComponentsPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		public bool Match(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return false;
			}
			var collectionElementType = memberType.DetermineCollectionElementType();

			return domainInspector.IsComponent(collectionElementType);
		}
	}
}