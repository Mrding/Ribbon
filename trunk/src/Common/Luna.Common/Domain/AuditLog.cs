using System;

namespace Luna.Common.Domain
{
    public class AuditLog
    {
        public string CurrentUser { get; set; }
        public string Action { get; set; }
        protected DateTime LogTime { get; set; }
    }
}
