using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Infrastructure.Domain;
using uNhAddIns.Adapters;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Data.Repositories;
using Luna.Core.Extensions;
using Luna.Common;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class MaintenanceScheduleModel : IMaintenanceScheduleModel
    {
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly IBackupTermRepository _backupTermRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public MaintenanceScheduleModel(ITimeBoxRepository timeBoxRepository, IBackupTermRepository backupTermRepository, 
            IAttendanceRepository attendanceRepository)
        {
            _timeBoxRepository = timeBoxRepository;
            _backupTermRepository = backupTermRepository;
            _attendanceRepository = attendanceRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Announce(Schedule schedule)
        {
            var timeboxes = _timeBoxRepository.GetTimeBoxesFrom(schedule.Campaign, schedule.Start, schedule.End);

            foreach (var timeBox in timeboxes)
            {
                foreach (var term in timeBox.TermSet)
                {
                    var hrDate = term.GetLowestTerm().SaftyGetProperty<DateTime, IAssignment>(o => o.SaftyGetHrDate());

                    if (!hrDate.IsInTheRange(schedule))
                        continue;

                    //backupTerm
                    var backup = new BackupTerm(term.Id, timeBox.Agent.Id, term.Start, term.End, term.Text, term.Background, term.Level)
                                     {
                                         HrDate = hrDate
                                     };
                    if (term.ParentTerm != null)
                        backup.ParentTermId = term.ParentTerm.Id;
                    term.SaftyInvoke<AssignmentBase>(o => backup.WorkingTotals = o.WorkingTotals);

                    _timeBoxRepository.SaveOrUpdate(backup);
                }
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void CloseAnnouncement(Schedule schedule)
        {
            var attIds = _attendanceRepository.GetAttendanceFrom(schedule).Select(o => o.Agent.Id).ToArray();
            _backupTermRepository.Delete(attIds, schedule);
        }

        public bool EmptyAgentTerms(IAgent agent, DateTime[] days, bool lockedTermIncluded)
        {
            var groupByLevelTerms = new Dictionary<int, IList<Term>>();
            var termCapacity = agent.Schedule.TermSet.Count();
            var timeBox = agent.Schedule;

            var iNext = 0;
            for (int i = 0; i < days.Length; i = iNext)
            {
                var start = days[i];
                DateTime end;
                double dayDiff;
                do
                {
                    end = (iNext == i ? start : days[iNext]).AddDays(1);
                    iNext++;
                    if (iNext >= days.Length)
                        break;
                    dayDiff = days[iNext].Subtract(start).TotalDays;
                } while (dayDiff == 1.0);

                //attention: suvent event might not in the range so then the end time need add one day
                agent.Schedule.TermSet.Retrieve<Term>(start, end.AddDays(1), o => o.IsNot<IImmutableTerm>()).Where(o => o.GetLowestTerm().StartIsCoverd(start, end)).ForEach(
                    o => TermWhere(o, timeBox, groupByLevelTerms, termCapacity, lockedTermIncluded));
            }

            //var @operator = timeBox.Operator;
            foreach (var o in groupByLevelTerms.OrderByDescending(o => o.Key))
            {
                if (o.Value.Count > 0)
                {
                    //@operator = 
                    _timeBoxRepository.BatchDeleteTerm(agent.Schedule, string.Empty, o.Value.Select(v => v.Id).ToArray());
                    //if (string.IsNullOrEmpty(@operator))
                    //    return false;
                }
            }

            return true;
        }


        public bool EmptyAgentTerms(IAgent agent, bool lockedTermIncluded)
        {
            var groupByLevelTerms = new Dictionary<int, IList<Term>>();

            var termCapacity = agent.Schedule.TermSet.Count();

            var timeBox = agent.Schedule;
            agent.Schedule.TermSet.ForEach(
                o => TermWhere(o, timeBox, groupByLevelTerms, termCapacity, lockedTermIncluded));

            //var @operator = timeBox.Operator;

            foreach (var o in groupByLevelTerms.OrderByDescending(o => o.Key))
            {
                if (o.Value.Count > 0)
                {
                    //@operator = 
                    _timeBoxRepository.BatchDeleteTerm(agent.Schedule, string.Empty, o.Value.Select(v => v.Id).ToArray());
                    //if (string.IsNullOrEmpty(@operator))
                    //    return false;
                }
            }
            return true;
        }

        private static bool TermWhere(Term o, TimeBox timeBox, Dictionary<int, IList<Term>> groupByLevelTerms, int termCapacity, bool lockedTermIncluded)
        {
            if (o.Is<IImmutableTerm>()) return false;

            bool? locked = null;
            var inside = !o.IsOutOfBoundary(t =>
                                            {
                                                if (lockedTermIncluded) return;
                                                locked = t.SaftyGetProperty<bool, Term>(x => x.Locked);

                                            }, timeBox);
            var result = inside && (locked == null || locked.Value == false);
            if (result)
            {
                if (!groupByLevelTerms.ContainsKey(o.Level))
                    groupByLevelTerms[o.Level] = new List<Term>(termCapacity);

                groupByLevelTerms[o.Level].Add(o);
            }
            return result;
        }
    }
}
