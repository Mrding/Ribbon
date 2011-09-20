using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    public interface IRealTimeAdherenceModel
    {
        //IList<IEnumerable> GetAdherenceBlocks(IList<TimeBox> agents, DateTime start, DateTime end, Action<Seat> resultLooping);

        IList<IEnumerable> GetAdherenceBlocks(IEnumerable agents, Func<object, TimeBox> cast, DateRange monitoringRange,  bool readFromCaches ,Action<object, Seat> resultLooping);

        IDictionary<Guid, IEnumerable> GetAgentAdherenceEvents(DateTime start, DateTime end);

        void AddAdherenceEvents(IEnumerable<AdherenceEvent> itmes);

        void RemoveAdherenceEvents(IEnumerable<AdherenceEvent> itmes);

        void AlterAdherenceEvents();
        
        IList<string> GetAbsenceTypes();
    }
}
