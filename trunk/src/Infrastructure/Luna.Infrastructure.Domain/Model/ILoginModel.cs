using System;
using System.Collections.Generic;

namespace Luna.Infrastructure.Domain.Model
{
    public interface ILoginModel
    {
        Employee GetMachedEmployee(string agentId);

        void Login(Employee employee, string role);

        void Login(string agentId, Guid roleId);

        void ChangePassword(Employee employee, bool needLog);

        void ChangePassword(string agentId, string newPassword);

        IList<string> GetRoles(Employee employee);
    }
}
