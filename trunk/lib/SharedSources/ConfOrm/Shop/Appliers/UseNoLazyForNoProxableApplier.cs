using System;
using ConfOrm.Mappers;
using NHibernate.Proxy;

namespace ConfOrm.Shop.Appliers
{
	public class UseNoLazyForNoProxableApplier<TApplyTo> : IPatternApplier<Type, TApplyTo> where TApplyTo : IEntityAttributesMapper
	{
		private readonly DynProxyTypeValidator proxyValidator = new DynProxyTypeValidator();
 
		public bool Match(Type subject)
		{
			var errors = proxyValidator.ValidateType(subject);
			return errors != null && errors.Count > 0;
		}

		public void Apply(Type subject, TApplyTo applyTo)
		{
			applyTo.Lazy(false);
		}
	}
}