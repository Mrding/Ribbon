using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class LazyRelation
	{
		public static LazyRelation Proxy = new LazyProxy();
		public static LazyRelation NoProxy = new LazyNoProxy();
		public static LazyRelation NoLazy = new NoLazyRelation();

		public abstract HbmLaziness ToHbm();
		private class LazyProxy : LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.Proxy;
			}
		}

		private class LazyNoProxy : LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.NoProxy;
			}
		}

		private class NoLazyRelation:LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.False;
			}
		}
	}
}