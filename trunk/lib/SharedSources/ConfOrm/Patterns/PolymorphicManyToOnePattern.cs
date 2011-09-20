using System;
using System.Linq;

namespace ConfOrm.Patterns
{
	public class PolymorphicManyToOnePattern : IPattern<Relation>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphicManyToOnePattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public bool Match(Relation subject)
		{
			Type to = subject.To;
			return domainInspector.GetBaseImplementors(to).Where(t => t != to).IsSingle(implementor => domainInspector.IsManyToOne(subject.From, implementor));
		}
	}
}