using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class CollectionFetchMode
	{
		public static CollectionFetchMode Select = new SelectFetchMode();
		public static CollectionFetchMode Join = new JoinFetchMode();
		public static CollectionFetchMode Subselect = new SubselectFetchMode();

		public abstract HbmCollectionFetchMode ToHbm();
		private class SelectFetchMode : CollectionFetchMode
		{
			public override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Select;
			}
		}

		private class JoinFetchMode : CollectionFetchMode
		{
			public override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Join;
			}
		}

		private class SubselectFetchMode : CollectionFetchMode
		{
			public override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Subselect;
			}
		}
	}
}