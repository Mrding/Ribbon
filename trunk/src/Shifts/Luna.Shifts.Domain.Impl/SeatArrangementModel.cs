using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class SeatArrangementModel : ISeatArrangementModel
    {
        private readonly ISeatRepository _seatRepository;
        private readonly ISeatBoxRepository _seatBoxRepository;
        private readonly ITimeBoxRepository _timeBoxRepository;

        public SeatArrangementModel(ISeatRepository seatRepository ,ISeatBoxRepository seatBoxRepository,
            ITimeBoxRepository timeBoxRepository)
        {
            _seatRepository = seatRepository;
            _seatBoxRepository = seatBoxRepository;
            _timeBoxRepository = timeBoxRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IList<SeatBox> GetSeatBoxesWithSeatArrangment(Guid[] seatIds, DateTime start, DateTime end)
        {
            var seatBoxList = _seatBoxRepository.GetByRagne(seatIds, start, end);

            var seatIdsParams = new string[seatBoxList.Count];
            var counter = 0;
            var seatBoxSet = seatBoxList.ToDictionary(o =>
            {
                o.Initial();
                var key = o.Seat.Id.ToString();
                seatIdsParams[counter] = key;
                counter++;
                return key;
            });

            Func<string, Seat> getSeat = seat => seatBoxSet.ContainsKey(seat) ? seatBoxSet[seat].Seat : default(Seat);
            Func<string, SeatArrangement, bool> addSeatArrangement =
                (seat, seatArrangement) =>
                    {
                        //if(seatArrangement.IsCoverd(start, end))
                            return seatBoxSet[seat].AddOccupation(seatArrangement);
                        //return false;
                    };

            var extendedRange = new {Start = start.AddDays(-2), End = end.AddDays(2)};

            var agentsOnSeat = _timeBoxRepository.GetTimeBoxes(seatIdsParams, new Guid[0], extendedRange.Start, extendedRange.End);

            foreach (var timeBox in agentsOnSeat)
                timeBox.TermSet.GenSeatArrangements(timeBox.Agent, getSeat, addSeatArrangement);
            return seatBoxList;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IDictionary<string, SeatBox> GetAllSeat()
        {
            
            return _seatBoxRepository.GetAll().ToDictionary(o=> o.Id.ToString(), o=> o as SeatBox);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Dictionary<ISimpleEmployee, ICollection<SeatArrangement>> GetAgentsWithSeatArrangement(Guid[] employeeIds, DateTime start, DateTime end, IDictionary<string, SeatBox> seatBoxes)
        {
            var timeBoxes = _timeBoxRepository.GetTimeBoxesByRange(employeeIds, start, end);
            var results = new Dictionary<ISimpleEmployee, ICollection<SeatArrangement>>(timeBoxes.Count);

            foreach (var timeBox in timeBoxes)
            {
                results[timeBox.Agent] = timeBox.TermSet.GenSeatArrangements(timeBox.Agent, seat => seatBoxes.ContainsKey(seat) ? seatBoxes[seat].SaftyGetProperty<Seat, SeatBox>(o => o.Seat) : default(Seat),
                 (seat, seatArrangement) => true);
            }

            return results;
        }
    }
}
