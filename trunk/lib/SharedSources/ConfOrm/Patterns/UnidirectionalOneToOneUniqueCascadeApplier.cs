using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class UnidirectionalOneToOneUniqueCascadeApplier : UnidirectionalUnaryAssociationPattern, IPatternApplier<MemberInfo, IManyToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public UnidirectionalOneToOneUniqueCascadeApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public override bool Match(MemberInfo subject)
		{
			var isUnidirectionalUnaryAssociation = base.Match(subject);
			if (isUnidirectionalUnaryAssociation)
			{
				var from = subject.DeclaringType;
				var to = subject.GetPropertyOrFieldType();
				return domainInspector.IsManyToOne(from, to) && domainInspector.IsMasterOneToOne(from, to);
			}
			return false;
		}

		#region Implementation of IPatternApplier<MemberInfo,IManyToOneMapper>

		public void Apply(MemberInfo subject, IManyToOneMapper applyTo)
		{
			applyTo.Unique(true);
			var from = subject.DeclaringType;
			var to = subject.GetPropertyOrFieldType();
			Cascade? applyCascade = domainInspector.ApplyCascade(from, subject, to);
			applyTo.Cascade(applyCascade.HasValue ? applyCascade.Value : Cascade.All);
		}

		#endregion
	}
}