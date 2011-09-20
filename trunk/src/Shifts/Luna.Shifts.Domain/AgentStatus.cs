using System;
using System.Collections.Generic;
using System.Diagnostics;
using Luna.Common.Interfaces;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{

    public static class AgentStatusPropertyExtension<TKey>
    {
        private static IDictionary<TKey, IEnumerable<AgentStatus>> _list = new Dictionary<TKey, IEnumerable<AgentStatus>>();

        public static void SetValue(TKey entity, IEnumerable<AgentStatus> list)
        {
            _list[entity] = list;
        }

        public static IEnumerable<AgentStatus> GetValue(TKey entity)
        {
            if (entity == null || !_list.ContainsKey(entity))
                return new List<AgentStatus>();
            return _list[entity];
        }


        public static void Dispose()
        {
            _list.Clear();
        }
    }

    [DebuggerDisplay("{AgentAcdid}: {TimeStamp} {OnService()}")]
    public class AgentStatus : ExpandObject, IAgentStatus
    {
        private string _agentStatusTypeCode;

        public virtual IAgentStatus SetProperties(DateTime timeStamp, string extNo, string agentAcdid, object option)
        {
            TimeStamp = timeStamp;
            ExtNo = extNo;
            AgentAcdid = agentAcdid;
            _agentStatusTypeCode = Convert.ToString(option);
            return this;
        }

        protected virtual Int64 Id { get; set; }
        protected virtual int SplitId { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual string ExtNo { get; set; }

        public virtual string AgentAcdid { get; set; }

        public virtual AgentStatusType AgentStatusType { get; set; }

        public virtual bool OnService
        {
            get { return AgentStatusType.OnService; }
        }

        public virtual bool IsLogout()
        {
            return AgentStatusType.IsLogout;
        }

        public virtual AgentStatus SetAgentStatusType(Dictionary<string, AgentStatusType> types)
        {
            try
            {
                AgentStatusType = types[_agentStatusTypeCode];
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message + string.Format("\r\n The Key is {0}", _agentStatusTypeCode));
            }

            return this;
        }
    }

    public class LastStatusDto
    {
        public string AgentAcdid { get; set; }
        public DateTime TimeStamp { get; set; }
        public string StatusName { get; set; }
        public string AreaName { get; set; }
        public string ExtNo { get; set; }
        public string SeatNo { get; set; }
        public Employee Employee { get; set; }
    }
}