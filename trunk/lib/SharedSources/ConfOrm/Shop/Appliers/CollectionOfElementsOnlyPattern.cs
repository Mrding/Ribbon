using System;
using System.Reflection;

namespace ConfOrm.Shop.Appliers
{
	public class CollectionOfElementsOnlyPattern : CollectionOfElementsPattern
	{
		public CollectionOfElementsOnlyPattern(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		protected override bool IsDictionaryOfElements(MemberInfo subject, Type memberType)
		{
			return KeyIsElement(memberType, subject) && ValueIsElement(memberType, subject);
		}

		protected virtual bool KeyIsElement(Type memberType, MemberInfo subject)
		{
			var mapKey = memberType.DetermineDictionaryKeyType();
			return !DomainInspector.IsManyToMany(subject.ReflectedType, mapKey) && !DomainInspector.IsComponent(mapKey);
		}
	}
}