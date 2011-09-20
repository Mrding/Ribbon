using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Globalization;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class BoxPairSwapModel : AbstractBoxPairSwap,IBoxPairSwapModel
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public BoxPairSwapModel(ITimeBoxRepository timeBoxRepository, IAttendanceRepository attendanceRepository,
            ILaborHoursCountingModel laborHoursCountingModel, IEmployeeRepository employeeRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;

            Applier = new BoxSwapModel(timeBoxRepository, attendanceRepository, laborHoursCountingModel);
            Replier = new BoxSwapModel(timeBoxRepository, attendanceRepository, laborHoursCountingModel);
        }

        private void GetSwapingDate(Pair<long?> termPair)
        {
            var term = new Pair<Term>();
            if (termPair.Applier.HasValue)
                term.Applier = _attendanceRepository.Get<Term>(termPair.Applier);
            if (termPair.Replier.HasValue)
                term.Replier = _attendanceRepository.Get<Term>(termPair.Replier);
            Applier.SwapingDate=Replier.SwapingDate = term.Applier.Get(term.Replier);
            _attendanceRepository.Clear();
        }
        private void ReSet()
        {
            Applier.ReSet();
            Replier.ReSet();
        }

        private void Initialize(Pair<Guid> agentPair, bool isLaborRule)
        {
            if (isLaborRule)
            {
                Applier.InitializeAttendance(agentPair.Applier);
                Replier.InitializeAttendance(agentPair.Replier);
            }
            else
            {
                Applier.ScheduleDate = Applier.SwapingDate;
                Replier.ScheduleDate = Replier.SwapingDate;
            }
            Applier.InitializeTimeBox(agentPair.Applier);
            Replier.InitializeTimeBox(agentPair.Replier);
            if (isLaborRule)
            {
                Applier.GetQLaborRule();
                Replier.GetQLaborRule();
            }
        }

        /// <summary>
        /// 基本代换班
        /// </summary>
        public void SwapingForTerm(Pair<Guid> agentPair, Pair<long?> assignmentPair, bool isLaborRule)
        {
            ReSet();
            _attendanceRepository.Clear();
            GetSwapingDate(assignmentPair);
            Initialize(agentPair,isLaborRule);

            Applier.InitializeSwapingForTerm(assignmentPair.Applier);
            Replier.InitializeSwapingForTerm(assignmentPair.Replier);
            if (Applier.Term != null)
            {
                Replier.TimeOff = Replier.TimeBox.SpecificTerm<TimeOff>().CollideTerms(Applier.Term).FirstOrDefault();
            }
            if (Replier.Term != null)
            {
                Applier.TimeOff = Applier.TimeBox.SpecificTerm<TimeOff>().CollideTerms(Replier.Term).FirstOrDefault();
            }
            //删除班
            DeleteTerm();
            //设置TimeOff
            SetTimeOff();
            //交换班表
            SwapSpecificTerm();
            SwapSpecificSubEvents();
            //验证交换信息
            VaildateSwapMessage();
        }

        /// <summary>
        /// 基本代换班验证
        /// </summary>
        public Dictionary<Guid, Dictionary<string, bool>> SwapingForTermValidate(Pair<Guid> agentPair, Pair<long?> assignmentPair, Pair<DateRange> dateRange)
        {
            _attendanceRepository.Clear();
            //初始化并做单一验证
            Applier.InitializeTimeBox(agentPair.Applier, dateRange.Applier);
            Replier.InitializeTimeBox(agentPair.Replier, dateRange.Replier);

            Applier.InitializeSwapingForTerm(assignmentPair.Applier);
            Replier.InitializeSwapingForTerm(assignmentPair.Replier);
            //验证是否重叠
            if (assignmentPair.Applier.HasValue)
                Applier.HasExchanged = Replier.TimeBox.CollideTerms(dateRange.Applier).Where(o => o.Level == 0).Where(o=> assignmentPair.Replier.HasValue ? o.Id != assignmentPair.Replier:true).Any();
            if (assignmentPair.Replier.HasValue)
                Replier.HasExchanged = Applier.TimeBox.CollideTerms(dateRange.Replier).Where(o => o.Level == 0).Where(o => assignmentPair.Applier.HasValue ? o.Id != assignmentPair.Applier : true).Any();
            //验证换完之后TimeOff移至后一天，验证后一天是否重复
            return SetValidates();
        }

        /// <summary>
        /// 离线代换班
        /// </summary>
        public void SwapingForSubEvent(Pair<Guid> agentPair, Pair<long> assignmentPair)
        {
            _attendanceRepository.Clear();
            Applier.Term = _attendanceRepository.Get<Term>(assignmentPair.Applier);
            Replier.Term = _attendanceRepository.Get<Term>(assignmentPair.Replier);
            var dateA = new DateRange(Applier.Term.Start, Applier.Term.End);
            var dateB = new DateRange(Replier.Term.Start, Replier.Term.End);
            _attendanceRepository.Clear();
            //初始化班表信息
            Applier.InitializeTimeBox(agentPair.Applier,dateA );
            Replier.InitializeTimeBox(agentPair.Replier,dateB );
            Applier.InitializeSwapingForSubEvent(new Pair<DateRange>(dateA, dateB), assignmentPair.Applier);
            Replier.InitializeSwapingForSubEvent(new Pair<DateRange>(dateB, dateA), assignmentPair.Replier);
            //验证
            ValidateTermGapHasMatched();
            //保存日志
            SaveLog();
            //删除班
            DeleteTerm();
            //交换班表
            SwapSpecificTermIsNeedSeat();
            //验证交换信息
            VaildateSwapMessage();
        }

        /// <summary>
        /// 离线代换班验证
        /// </summary>
        public Dictionary<Guid, Dictionary<string, bool>> SwapingForSubEventValidate(Pair<Guid> agentPair, Pair<long> subEventPair)
        {
            _attendanceRepository.Clear();
            Applier.Term = _attendanceRepository.Get<Term>(subEventPair.Applier);
            Replier.Term = _attendanceRepository.Get<Term>(subEventPair.Replier);
            var dateA = new DateRange(Applier.Term.Start, Applier.Term.End);
            var dateB = new DateRange(Replier.Term.Start, Replier.Term.End);
            _attendanceRepository.Clear();
            //初始化班表信息
            Applier.InitializeTimeBox(agentPair.Applier, dateA);
            Replier.InitializeTimeBox(agentPair.Replier, dateB);
            Applier.InitializeSwapingForSubEvent(new Pair<DateRange>(dateA, dateB), subEventPair.Applier);
            Replier.InitializeSwapingForSubEvent(new Pair<DateRange>(dateB, dateA), subEventPair.Replier);
            Applier.IsNeedSeat = Replier.IsNeedSeat = Applier.Term.IsNeedSeat != Replier.Term.IsNeedSeat;
            return SetValidates();
        }

        /// <summary>
        /// 多日代换班
        /// </summary>
        public void SwapingForMultiDay(Pair<Guid> agentPair, DateRange dateRange, bool isLaborRule)
        {
            ReSet();
            _attendanceRepository.Clear();
            Applier.SwapingDate = Replier.SwapingDate = dateRange;
            Initialize(agentPair,isLaborRule);

            Applier.InitializeSwapingForMultiDay();
            Replier.InitializeSwapingForMultiDay();
            //是否存在TimeOff
            ValidateHasTimeOff();
            //是否存在AbsentEvent
            ValidateHasAbsentEvent();
            //是否存在锁
            ValidateHasLocked();
            //删除班表
            DeleteTerms(new Pair<IList<Term>>(Applier.CurrentTerms, Replier.CurrentTerms));
            //删除DayOff
            DeleteTerms(new Pair<IList<Term>>(Applier.DayOffs, Replier.DayOffs));
            //交换班表
            SwapTerms(new Pair<IList<Term>>(Applier.CurrentTerms, Replier.CurrentTerms));
            SwapTerms(new Pair<IList<Term>>(Applier.LevelOnes, Replier.LevelOnes));
            SwapTerms(new Pair<IList<Term>>(Applier.LevelTwos, Replier.LevelTwos));
            SwapTerms(new Pair<IList<Term>>(Applier.LevelThrees, Replier.LevelThrees));
            //验证交换信息
            VaildateSwapMessage();
        }

        /// <summary>
        /// 多日代换班验证
        /// </summary>
        public Dictionary<Guid, Dictionary<string, bool>> SwapingForMultiDayValidate(Pair<Guid> agentPair, DateRange dateRange)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 时段代换班
        /// </summary>
        public void SwapingForDateRange(Pair<Guid> agentPair, Pair<DateRange> dateRange, bool isLaborRule)
        {
            ReSet();
            _attendanceRepository.Clear();
            Applier.SwapingDate = Replier.SwapingDate = dateRange.Applier.GetSwapingDate(dateRange.Applier, dateRange.Replier);
            Initialize(agentPair, isLaborRule);
            Applier.InitializeSwapingForDateRange(Applier.SwapingDate);
            Replier.InitializeSwapingForDateRange(Replier.SwapingDate);
            //删除请假
            DeleteTerms(new Pair<IList<Term>>(Applier.AbsentEvents, Replier.AbsentEvents));
            //删除班表
            DeleteTerms(new Pair<IList<Term>>(Applier.TempTerms, Replier.TempTerms));
            if (dateRange.Applier==dateRange.Replier)
            {
                Applier.DealWithDateRange(Replier, dateRange.Applier);
                Replier.DealWithDateRange(Applier, dateRange.Replier);
            }
            else
            {
                Applier.DealWithDateRange(Replier, dateRange);
                Replier.DealWithDateRange(Applier, new Pair<DateRange>(dateRange.Replier,dateRange.Applier));
            }
            //验证交换信息
            VaildateSwapMessage();

        }

        /// <summary>
        /// 时段代换班验证
        /// </summary>
        public Dictionary<Guid, Dictionary<string, bool>> SwapingForDateRangeValidate(Pair<Guid> agentPair,  Pair<DateRange> dateRange)
        {
            ReSet();
            _attendanceRepository.Clear();
            Applier.SwapingDate = Replier.SwapingDate = dateRange.Applier.GetSwapingDate(dateRange.Applier, dateRange.Replier);
            Initialize(agentPair,false);
            Applier.InitializeSwapingForDateRange(Applier.SwapingDate);
            Replier.InitializeSwapingForDateRange(Replier.SwapingDate);

            if (dateRange.Applier == dateRange.Replier)
            {
                if (Replier.NotSubEventAndGap(dateRange.Applier))
                {
                    Applier.HasExchanged = true;
                }
                if (Applier.NotSubEventAndGap(dateRange.Replier))
                {
                    Replier.HasExchanged = true;
                }
            }
            else
            {
                //获取切割时间
                var cutDateRange = DateRange.Cut(dateRange.Applier, dateRange.Replier);
                var applierRegion = cutDateRange.Where(t => t.Item2 == RegionType.Applier).Select(o => o.Item1);
                var replierRegion = cutDateRange.Where(t => t.Item2 == RegionType.Replier).Select(o => o.Item1);
                foreach (var range in applierRegion)
                {
                    if (Replier.NotSubEventAndGap(range))
                    {
                        Applier.HasExchanged = true;
                        break;
                    }
                }
                foreach (var range in replierRegion)
                {
                    if (Applier.NotSubEventAndGap(range))
                    {
                        Replier.HasExchanged = true;
                        break;
                    }
                }
            }
            return SetValidates();
        }

        public Dictionary<string, bool> PostForNewRangeValidate(Guid agent, DateRange dateRange)
        {
            _attendanceRepository.Clear();
            return Applier.NewRangeValidate(agent, dateRange);
        }

        /// <summary>
        /// 获取劳动政策
        /// </summary>
        public Pair<Attendance> GetLaborRule()
        {
            Applier.GetLaborRule();
            Replier.GetLaborRule();
            var results = new Pair<Attendance>
                              {
                                  Applier = Applier.Agent.LaborRule,
                                  Replier = Replier.Agent.LaborRule
                              };
            return results;
        }

        public string LaborRuleInfo
        {
            get { return string.Format("申请方({0}):\n{1}\n回复方({2}):\n{3}", Applier.TimeBox.Agent.Name, Applier, Replier.TimeBox.Agent.Name, Replier); }
        }

        public IList<Employee> GetAllEmployee()
        {
            return _employeeRepository.GetAll();
        }

        public void SaveLog()
        {
            Applier.SaveLog(Applier.Term);
            Replier.SaveLog(Replier.Term);
        }

        public void SaveLogs()
        {
            foreach (var assignment in Applier.CurrentTerms)
            {
                Applier.SaveLog(assignment);
            }
            foreach (var assignment in Replier.CurrentTerms)
            {
                Replier.SaveLog(assignment);
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public string Submit()
        {
            var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession();
            if (Applier.Message.Length == 0 && Replier.Message.Length == 0)
            {
                Applier.Submit();
                Replier.Submit();
                return LanguageReader.GetValue("Shifts_BoxPairSwap_Success");
            }
            session.Transaction.Rollback();
            return WithMessage();
        }
    }
}


