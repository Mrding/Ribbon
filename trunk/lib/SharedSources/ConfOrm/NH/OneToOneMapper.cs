using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class OneToOneMapper: IOneToOneMapper
	{
		private readonly MemberInfo member;
		private readonly HbmOneToOne oneToOne;
		private readonly IAccessorPropertyMapper entityPropertyMapper;

		public OneToOneMapper(MemberInfo member, HbmOneToOne oneToOne)
		{
			this.member = member;
			this.oneToOne = oneToOne;
			if (member == null)
			{
				this.oneToOne.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => oneToOne.access = x);
			}
		}

		#region Implementation of IOneToOneMapper

		public void Cascade(Cascade cascadeStyle)
		{
			oneToOne.cascade = (cascadeStyle.Exclude(ConfOrm.Cascade.DeleteOrphans)).ToCascadeString();
		}

		#endregion

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			// not supported by HbmOneToOne
		}

		#endregion

		public void Lazy(LazyRelation lazyRelation)
		{
			oneToOne.lazy = lazyRelation.ToHbm();
			oneToOne.lazySpecified = oneToOne.lazy != HbmLaziness.Proxy;
		}

		public void Constrained(bool value)
		{
			oneToOne.constrained = value;
		}

		public void PropertyReference(MemberInfo propertyInTheOtherSide)
		{
			if(propertyInTheOtherSide == null)
			{
				oneToOne.propertyref = null;
				return;
			}

			if (member != null && propertyInTheOtherSide.DeclaringType != member.GetPropertyOrFieldType())
			{
				throw new ArgumentOutOfRangeException("propertyInTheOtherSide", string.Format("Expected a member of {0} found the member {1} of {2}", member.GetPropertyOrFieldType(), propertyInTheOtherSide, propertyInTheOtherSide.DeclaringType));
			}

			oneToOne.propertyref = propertyInTheOtherSide.Name;
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				oneToOne.formula = null;
				oneToOne.formula1 = null;
				return;
			}

			var formulaLines = formula.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				oneToOne.formula = new[] { new HbmFormula { Text = formulaLines } };
				oneToOne.formula1 = null;
			}
			else
			{
				oneToOne.formula1 = formula;
				oneToOne.formula = null;
			}
		}
	}
}