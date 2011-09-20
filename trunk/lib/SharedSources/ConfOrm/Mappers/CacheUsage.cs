using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class CacheUsage
	{
		public static CacheUsage ReadOnly = new ReadOnlyUsage();
		public static CacheUsage ReadWrite = new ReadWriteUsage();
		public static CacheUsage NonstrictReadWrite = new NonstrictReadWriteUsage();
		public static CacheUsage Transactional = new TransactionalUsage();

		public abstract HbmCacheUsage ToHbm();

		private class NonstrictReadWriteUsage : CacheUsage
		{
			public override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.NonstrictReadWrite;
			}
		}

		private class ReadOnlyUsage : CacheUsage
		{
			public override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.ReadOnly;
			}
		}

		private class ReadWriteUsage : CacheUsage
		{
			public override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.ReadWrite;
			}
		}

		private class TransactionalUsage : CacheUsage
		{
			public override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.Transactional;
			}
		}
	}
}