using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    //public interface ISeatingQueryService
    //{
    //    ISeatingTerm[] GetTermByEmployee(IAgent targetEmployee, DateTime start, DateTime end);
    //    //IConsolidationRule[] GetConsolidationRuleBySite(ISeatingSite targetSite);
    //    //ISeatingOrderMethod GetOrderMethodByOrganizationAndArea(Guid targetOrganizationID, ISeatingArea tragetArea);
    //    ISeatingSeat[] GetSeatByArea(ISeatingArea targetArea);
    //}


    public interface ISeatingServiceModel
    {
        ISeatingSite<Entity>[] GetSites(Entity campaign);

        IList<SeatBox> Preparing(Entity campaign, Entity[] areas, DateTime start, DateTime end);

        event EventHandler<SeatingEngineStatus> EngineStatusChanged;

        IEnumerable<TimeBox> ArrangeSeat(Entity site, int bufferLength, bool isMatchingRank,
                                      IgnoreAgentPriority agentPriorityMethodology, DateTime start, DateTime end, SeatingEngineStatus engineStatus);

        void SubmitArrangement();

        void RegisterSynchronization(Action<bool> complete);
    }
}
