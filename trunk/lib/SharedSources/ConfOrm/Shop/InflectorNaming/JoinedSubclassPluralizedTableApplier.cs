using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class JoinedSubclassPluralizedTableApplier : IPatternApplier<Type, IJoinedSubclassAttributesMapper>
	{
		private readonly IInflector inflector;

		public JoinedSubclassPluralizedTableApplier(IInflector inflector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		public IInflector Inflector
		{
			get { return inflector; }
		}

		#region IPatternApplier<Type,IJoinedSubclassAttributesMapper> Members

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IJoinedSubclassAttributesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		public virtual string GetTableName(Type subject)
		{
			return inflector.Pluralize(subject.Name);
		}

		#endregion
	}
}