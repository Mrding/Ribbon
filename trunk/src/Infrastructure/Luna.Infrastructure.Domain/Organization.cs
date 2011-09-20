using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Interfaces;

namespace Luna.Infrastructure.Domain
{
    public class Organization : Entity, IHierarchical
    {
        //public Organization()
        //{
        //    //_children = new ObservableSet<IHierarchical>();
        //}

        public virtual IHierarchical Parent { get; set; }
      
        public virtual ICollection<IHierarchical> Children { get; set; }

        public virtual LaborRule LaborRule { get; set; }

        public virtual Entity ServiceSite { get; set; }

        public virtual IList<Employee> Employees { get; set; }
       
        public override string GetUniqueKey()
        {
            return Name;
        }
    }
}
