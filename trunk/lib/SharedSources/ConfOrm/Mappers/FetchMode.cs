using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class FetchMode
	{
		public static FetchMode Select = new SelectFetchMode();
		public static FetchMode Join = new JoinFetchMode();

		public abstract HbmFetchMode ToHbm();
		private class SelectFetchMode : FetchMode
		{
			public override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Select;
			}
		}

		private class JoinFetchMode : FetchMode
		{
			public override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Join;
			}
		}
	}
}