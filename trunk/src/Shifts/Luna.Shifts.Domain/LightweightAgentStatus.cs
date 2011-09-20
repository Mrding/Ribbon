using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public struct LightweightAgentStatus : IAgentStatus, IEquatable<LightweightAgentStatus>, IEqualityComparer<LightweightAgentStatus>
    {
        public IAgentStatus SetProperties(DateTime timeStamp, string extNo, string agentAcdid, object option)
        {
            TimeStamp = timeStamp;
            ExtNo = extNo;
            AgentAcdid = agentAcdid;
            OnService = Convert.ToBoolean(option);
            return this;
        }

        public DateTime TimeStamp { get; private set; }

        public string ExtNo { get; private set; }

        public string AgentAcdid { get; private set; }

        public bool OnService { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is LightweightAgentStatus)
            {
                var other = (LightweightAgentStatus)obj;
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return AgentAcdid.GetHashCode() ^ ExtNo.GetHashCode() ^ TimeStamp.GetHashCode();
        }

        public bool Equals(LightweightAgentStatus other)
        {
            return AgentAcdid == other.AgentAcdid && ExtNo == other.ExtNo && TimeStamp == other.TimeStamp &&
                   OnService == other.OnService;
        }

        public bool Equals(LightweightAgentStatus x, LightweightAgentStatus y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(LightweightAgentStatus obj)
        {
            return obj.GetHashCode();
        }
    }
}
