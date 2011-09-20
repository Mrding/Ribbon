using System;
using System.Collections.Generic;

namespace Luna.Infrastructure.Domain
{
    public class AuthRole
    {
        public virtual Guid Id { get; set; }
        public virtual string RoleName { get; set; }
        public virtual IList<Employee> Employees { get; set; }
    }
}
