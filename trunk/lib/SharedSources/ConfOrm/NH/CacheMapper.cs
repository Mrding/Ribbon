using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class CacheMapper: ICacheMapper
	{
		private readonly HbmCache cacheMapping;

		public CacheMapper(HbmCache cacheMapping)
		{
			if (cacheMapping == null)
			{
				throw new ArgumentNullException("cacheMapping");
			}
			this.cacheMapping = cacheMapping;
			Usage(CacheUsage.Transactional);
		}

		#region Implementation of ICacheMapper

		public void Usage(CacheUsage cacheUsage)
		{
			cacheMapping.usage = cacheUsage.ToHbm();
		}

		public void Region(string regionName)
		{
			cacheMapping.region = regionName;
		}

		public void Include(CacheInclude cacheInclude)
		{
			cacheMapping.include = cacheInclude.ToHbm();
		}

		#endregion
	}
}