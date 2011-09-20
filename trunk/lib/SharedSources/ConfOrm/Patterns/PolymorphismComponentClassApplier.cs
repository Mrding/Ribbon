using System;
using System.Linq;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismComponentClassApplier : IPatternApplier<Type, IComponentAttributesMapper>
	{
				private readonly IDomainInspector domainInspector;

		public PolymorphismComponentClassApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		/// <summary>
		/// An interface is implemented only by a component and thus by its own hierarchy.
		/// </summary>
		/// <param name="subject">The type of the property inside the entity.</param>
		/// <returns>
		/// true when it is a component (already checked by DomainInspector), the type is an interface, there is just one implementor the implementor is a component;
		/// false otherwise.
		/// </returns>
		public bool Match(Type subject)
		{
			if(subject == null || !subject.IsInterface)
			{
				return false;
			}
		
			return domainInspector.GetBaseImplementors(subject).IsSingle(t=> domainInspector.IsComponent(t));
		}

		public void Apply(Type subject, IComponentAttributesMapper applyTo)
		{
			var implementor = domainInspector.GetBaseImplementors(subject).Single();
			applyTo.Class(implementor);
		}
	}
}