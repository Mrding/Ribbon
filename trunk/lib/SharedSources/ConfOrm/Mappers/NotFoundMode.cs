using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public abstract class NotFoundMode
	{
		public static NotFoundMode Ignore = new IgnoreNotFoundMode();
		public static NotFoundMode Exception = new ExceptionNotFoundMode();

		public abstract HbmNotFoundMode ToHbm();

		private class IgnoreNotFoundMode : NotFoundMode
		{
			#region Overrides of NotFoundMode

			public override HbmNotFoundMode ToHbm()
			{
				return HbmNotFoundMode.Ignore;
			}

			#endregion
		}
		private class ExceptionNotFoundMode : NotFoundMode
		{
			#region Overrides of NotFoundMode

			public override HbmNotFoundMode ToHbm()
			{
				return HbmNotFoundMode.Exception;
			}

			#endregion
		}
	}
}