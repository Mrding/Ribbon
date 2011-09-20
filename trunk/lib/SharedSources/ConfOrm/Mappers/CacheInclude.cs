using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class CacheInclude
	{
		public static CacheInclude All = new AllCacheInclude();
		public static CacheInclude NonLazy = new NonLazyCacheInclude();

		public abstract HbmCacheInclude ToHbm();

		public class AllCacheInclude : CacheInclude
		{
			public override HbmCacheInclude ToHbm()
			{
				return HbmCacheInclude.All;
			}
		}

		public class NonLazyCacheInclude : CacheInclude
		{
			public override HbmCacheInclude ToHbm()
			{
				return HbmCacheInclude.NonLazy;
			}
		}
	}
}