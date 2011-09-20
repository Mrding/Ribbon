using System;

namespace Luna.Shifts.Domain
{
    public interface IAgentStatus
    {
        DateTime TimeStamp { get; }

        string ExtNo { get; }

        string AgentAcdid { get; }

        bool OnService { get; }

        IAgentStatus SetProperties(DateTime timeStamp, string extNo, string agentAcdid, object option);
    }
}
