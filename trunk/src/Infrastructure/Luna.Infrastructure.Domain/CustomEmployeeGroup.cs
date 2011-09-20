using System.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class CustomEmployeeGroup : AbstractEntity<int>
    {
        protected CustomEmployeeGroup()
        { }

        public CustomEmployeeGroup(string groupName, ICollection<ISimpleEmployee> employees)
        {
            GroupName = groupName;
            _employees = new Iesi.Collections.Generic.HashedSet<ISimpleEmployee>(employees);
        }

        public virtual Employee Owner { get; set; }
        public virtual string GroupName { get; set; }

        private Iesi.Collections.Generic.ISet<ISimpleEmployee> _employees = new Iesi.Collections.Generic.HashedSet<ISimpleEmployee>();

        public virtual Iesi.Collections.Generic.ISet<ISimpleEmployee> Employees
        {
            get { return _employees; }
            set { _employees = value; }
        }
    }
}
