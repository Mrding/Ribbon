using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToOneAssociationPoidApplier : IPatternApplier<MemberInfo, IIdMapper>
	{
		protected const BindingFlags PublicPropertiesOfClass = BindingFlags.Public | BindingFlags.Instance;
		private readonly IDomainInspector domainInspector;

		public BidirectionalOneToOneAssociationPoidApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			Type container = subject.ReflectedType;
			MemberInfo bidirectionalOneToOneOrNull = BidirectionalOneToOneOrNull(container);
			if (bidirectionalOneToOneOrNull != null)
			{
				var otherSidePropertyType = bidirectionalOneToOneOrNull.GetPropertyOrFieldType();
				return domainInspector.IsOneToOne(container, otherSidePropertyType) && !domainInspector.IsManyToOne(container, otherSidePropertyType);
			}
			return false;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IIdMapper>

		public void Apply(MemberInfo subject, IIdMapper applyTo)
		{
			applyTo.Generator(Generators.Foreign(BidirectionalOneToOneOrNull(subject.ReflectedType)));
		}

		#endregion

		protected MemberInfo BidirectionalOneToOneOrNull(Type container)
		{
			return
				CandidatesBidirectionalOneToOne(container).FirstOrDefault(
					foreingProp => domainInspector.IsMasterOneToOne(foreingProp.GetPropertyOrFieldType(), container));
		}

		protected IEnumerable<MemberInfo> CandidatesBidirectionalOneToOne(Type container)
		{
			return from property in container.GetProperties(PublicPropertiesOfClass)
			       where domainInspector.IsEntity(property.PropertyType)
			       select (MemberInfo) property;
		}
	}
}