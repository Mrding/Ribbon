using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class SeatDispatcherModel : ISeatDispatcherModel
    {
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly ISeatBoxRepository _seatBoxRepository;
        
        private readonly IAreaRepository _areaRepository;

        public SeatDispatcherModel(ITimeBoxRepository timeBoxRepository, 
            ISeatBoxRepository seatBoxRepository, IAreaRepository areaRepository)
        {
            _timeBoxRepository = timeBoxRepository;
            _seatBoxRepository = seatBoxRepository;
            _areaRepository = areaRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<SeatBox> GetSeats(Schedule schedule, Guid[] excludedEmployeeIds, out IList<TimeBox> timeBoxes ,out IList<Area> areas)
        {
            areas = _areaRepository.GetAreaByCampaign(schedule.Campaign);
            var results = _seatBoxRepository.Search(areas.ToArray(),schedule.Start, schedule.End, true);

            var seatIds = new string[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                var item =  results[i];
                seatIds[i] = item.Id.ToString();
                item.Initial();
            }

            timeBoxes = _timeBoxRepository.GetTimeBoxes(seatIds, excludedEmployeeIds, schedule.Start, schedule.End);
            return results;
        }


        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void SubmitChanges() { }

        [PersistenceConversation(ConversationEndMode = EndMode.Abort)]
        public void Abort() { }


    }
}
