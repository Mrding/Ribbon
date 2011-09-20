using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain.Model;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using uNhAddIns.Adapters;

namespace Luna.Infrastructure.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class LoginModel : ILoginModel
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public LoginModel(ILoginRepository loginRepository, IAuditLogRepository auditLogRepository)
        {
            _loginRepository = loginRepository;
            _auditLogRepository = auditLogRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public Employee GetMachedEmployee(string agentId)
        {
            return agentId.IsNotNullOrEmpty() ? _loginRepository.GetMachedEmployee(agentId) : null;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Login(Employee employee, string role)
        {
            ApplicationCache.Set(Global.LoginEmployee, employee);
            ApplicationCache.Set(Global.LoggerId, employee.AgentId);

            if (role.IsNotNullOrEmpty())
            {
                //Set Rules
                SetFunctionKeys(role);
            }
            //Log
            AuditLoginLog(new AuditLog { Action = LanguageReader.GetValue("Logging_Admin_Login"), CurrentUser = employee.AgentId });
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Login(string agentId, Guid roleId)
        {
            var employee = _loginRepository.FirstOrDefault(o => o.AgentId == agentId);
            ApplicationCache.Set(Global.LoginEmployee, employee);
            ApplicationCache.Set(Global.LoggerId, employee.AgentId);

            if (roleId != Guid.Empty)
            {
                //Set Rules
                SetFunctionKeys(roleId);
            }
            //Log
            AuditLoginLog(new AuditLog { Action = LanguageReader.GetValue("Logging_Admin_Login"), CurrentUser = employee.AgentId });
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void AuditLoginLog(AuditLog log)
        {
            _auditLogRepository.MakePersistent(log);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void ChangePassword(Employee employee, bool needLog)
        {
            employee.Password = Cryptographer.CreateHash("SHA512Managed", employee.Password);
            _loginRepository.MakePersistent(employee);
            if (needLog)
                AuditLoginLog(new AuditLog { Action = LanguageReader.GetValue("Administration_Login_ChangePassword"), CurrentUser = employee.AgentId });
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void ChangePassword(string agentId, string newPassword)
        {
            var employee = _loginRepository.FirstOrDefault(o => o.AgentId == agentId);
            employee.Password = newPassword;
            _loginRepository.MakePersistent(employee);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public IList<string> GetRoles(Employee employee)
        {
            return _loginRepository.GetRoles(employee.Id);
        }

        private void SetFunctionKeys(Guid roleId)
        {
            var functionKeys = _loginRepository.GetFunctionKeys(roleId);
            ApplicationCache.Set(Global.LoginUserFunctionKeys, functionKeys);
        }

        private void SetFunctionKeys(string role)
        {
            var functionKeys = _loginRepository.GetFunctionKeys(role);
            ApplicationCache.Set(Global.LoginUserFunctionKeys, functionKeys);
        }
    }
}
