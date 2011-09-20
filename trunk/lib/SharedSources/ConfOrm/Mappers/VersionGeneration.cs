using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class VersionGeneration
	{
		public static VersionGeneration Never = new NeverGeneration();
		public static VersionGeneration Always = new AlwaysGeneration();

		public abstract HbmVersionGeneration ToHbm();
		private class NeverGeneration : VersionGeneration
		{
			public override HbmVersionGeneration ToHbm()
			{
				return HbmVersionGeneration.Never;
			}
		}

		private class AlwaysGeneration : VersionGeneration
		{
			public override HbmVersionGeneration ToHbm()
			{
				return HbmVersionGeneration.Always;
			}
		}
	}

}