using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class OneToOneUnidirectionalToManyToOnePattern : IPattern<Relation>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		private readonly IDomainInspector domainInspector;
		private readonly IExplicitDeclarationsHolder declarationsHolder;

		public OneToOneUnidirectionalToManyToOnePattern(IDomainInspector domainInspector, IExplicitDeclarationsHolder declarationsHolder)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if (declarationsHolder == null)
			{
				throw new ArgumentNullException("declarationsHolder");
			}
			this.domainInspector = domainInspector;
			this.declarationsHolder = declarationsHolder;
		}

		#region Implementation of IPattern<Relation>

		public bool Match(Relation subject)
		{
			bool isRelationBetweenEntities = domainInspector.IsEntity(subject.From) && domainInspector.IsEntity(subject.To);
			return isRelationBetweenEntities && declarationsHolder.OneToOneRelations.Contains(subject) && IsUnidirectional(subject);
		}

		#endregion

		private bool IsUnidirectional(Relation relation)
		{
			return !HasPropertyOf(relation.To, relation.From);
		}

		protected bool HasPropertyOf(Type container, Type contained)
		{
			return container.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType).Any(t => t == contained);
		}
	}
}