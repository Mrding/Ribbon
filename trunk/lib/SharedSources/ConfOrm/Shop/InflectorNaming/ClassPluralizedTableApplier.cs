using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class ClassPluralizedTableApplier : IPatternApplier<Type, IClassAttributesMapper>
	{
		private readonly IInflector inflector;

		public ClassPluralizedTableApplier(IInflector inflector)
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

		#region IPatternApplier<Type,IClassAttributesMapper> Members

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IClassAttributesMapper applyTo)
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