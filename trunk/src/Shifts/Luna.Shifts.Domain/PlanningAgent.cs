using System;
using System.Collections.Generic;
using System.ComponentModel;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using System.Linq;
using System.Collections;
using Luna.Common.Interfaces;

namespace Luna.Shifts.Domain
{
    public class PlanningAgent : IEquatable<PlanningAgent>, IEquatable<ISimpleEmployee>, IEqualityComparer<PlanningAgent>, IAgent, IIntIndexer<ITerm>, IDateIndexer<DateTerm>, IIndexable,
        IDateIndexer<ITerm>
    {
        private const int CellUnit = 5;
        private readonly Attendance _laborRule;
        private readonly TimeBox _timeBox;
        private Dictionary<int, HeaderContainer<DateTime, IIndependenceTerm, int>> _dailyContainer;

        private Dictionary<DateTime, DateTerm> _dailyRecords;

        private bool[] _onlines;
        private DateRange _enquiryRange;

        //为了 hibernate hql new 语法支持
        protected PlanningAgent(TimeBox timeBox)
        {
            _timeBox = timeBox;
        }

        //为了 hibernate hql new 语法支持, 重新读取 ReloadAgent用
        public PlanningAgent(TimeBox timeBox, Attendance attendance)
            : this(timeBox)
        {
            _laborRule = attendance;
        }

        /// <summary>
        /// 排班秘书使用
        /// </summary>
        /// <param name="timeBox"></param>
        /// <param name="attendance"></param>
        /// <param name="enquiryRange"></param>
        public PlanningAgent(TimeBox timeBox, Attendance attendance, DateRange enquiryRange)
            : this(timeBox, attendance)
        {
            _enquiryRange = enquiryRange;

            var enquiryDays = (int)_enquiryRange.Duration.TotalDays;
            _dailyContainer = new Dictionary<int, HeaderContainer<DateTime, IIndependenceTerm, int>>(enquiryDays); // 重要逻辑控制
            _dailyRecords = new Dictionary<DateTime, DateTerm>(enquiryDays);
            _timeBox.Boundary = new DateRange(_enquiryRange.Start.AddDays(-Global.HeadDayAmount), _enquiryRange.End); // 重要逻辑控制
            BuildOnlines();
        }

        /// <summary>
        /// 汇入班表,不考虑参与排班期使用
        /// </summary>
        /// <param name="timeBox"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public PlanningAgent(TimeBox timeBox, DateTime start, DateTime end)
            : this(timeBox)
        {
            _enquiryRange = new DateRange(start, end);

            var enquiryDays = (int)_enquiryRange.Duration.TotalDays;
            _dailyContainer = new Dictionary<int, HeaderContainer<DateTime, IIndependenceTerm, int>>(enquiryDays); // 重要逻辑控制
            _dailyRecords = new Dictionary<DateTime, DateTerm>(enquiryDays);

            timeBox.Boundary = new DateRange(_enquiryRange.Start.AddDays(-Global.HeadDayAmount), end); // 重要逻辑控制

            BuildOnlines();
        }

        public int Index { get; set; }

        public ISimpleEmployee Profile
        {
            get { return Schedule.Agent; }
        }

        public TimeBox Schedule
        {
            get { return _timeBox; }
        }

        public Attendance LaborRule
        {
            get { return _laborRule; }
        }

        public bool[] Onlines
        {
            get { return _onlines; }
        }

        public IAgent TransferPropertiesFrom(IAgent original)
        {
            original.SaftyInvoke<PlanningAgent>(o =>
                                                    {
                                                        _enquiryRange = o._enquiryRange;
                                                        var enquiryDays = (int)_enquiryRange.Duration.TotalDays;
                                                        _timeBox.Boundary = new DateRange(_enquiryRange.Start.AddDays(-Global.HeadDayAmount), _enquiryRange.End); // 重要逻辑控制
                                                        _dailyContainer = new Dictionary<int, HeaderContainer<DateTime, IIndependenceTerm, int>>(enquiryDays); // 重要逻辑控制
                                                        _dailyRecords = new Dictionary<DateTime, DateTerm>(enquiryDays);
                                                        BuildOnlines();
                                                    });
            return this;
        }

        public virtual bool? IsSelected
        {
            get
            {
                bool? isSelected = false;
                Profile.SaftyInvoke<ISelectable>(o => { isSelected = o.IsSelected; });

                return isSelected;
            }
            set { Profile.SaftyInvoke<ISelectable>(o => { o.IsSelected = value; }); }
        }

        private bool? _operationFail = false;
        public virtual bool? OperationFail
        {
            get { return _operationFail; }
            set { _operationFail = value; }
        }

        public object Tag { get; set; }

        public virtual int DayOffRemains { get; set; }

        public void BuildOnlines()
        {
            //start, end 为阔展查询范围

            _dailyContainer.Clear();

            var totalDays = Convert.ToInt32(_enquiryRange.Duration.TotalDays); //_dateStorage.Count;
            DayOffRemains = totalDays;

            _onlines = _timeBox.TermSet.ConvertToCell(_enquiryRange, CellUnit, t => t.OnService, item =>
                item.SaftyInvoke<IIndependenceTerm>(t =>
                                                  {

                                                      //HrDate使用
                                                      var date = t.SaftyGetHrDate();
                                                      var dateKey = date.IndexOf(_timeBox.Boundary.Start);

                                                      if (dateKey != -1)
                                                      {
                                                          if (!_dailyContainer.ContainsKey(dateKey))
                                                          {
                                                              _dailyContainer[dateKey] = new HeaderContainer<DateTime, IIndependenceTerm, int>(date, i => i).Initial(date, false);
                                                              if (t.IsNot<IOffWork>())
                                                                  DayOffRemains--;
                                                          }
                                                          _dailyContainer[dateKey].Items.Add(t);
                                                      }
                                                  })
            );


            var zeroBasedStartDate = _timeBox.Boundary.Start; //_enquiryRange.Start.Date.AddDays(-Global.HeadDayAmount); // range.start - 1day
            for (var i = 0; i < totalDays; i++)
            {
                if (_dailyContainer.ContainsKey(i)) continue;
                var dateKey = zeroBasedStartDate.AddDays(i);
                _dailyContainer[i] = new HeaderContainer<DateTime, IIndependenceTerm, int>(dateKey, @int => @int).Initial(dateKey, true);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _timeBox.TermSet.GetEnumerator();
            //return _dailyContainer.OrderBy(o => o.Key).Select(o =>
            //                                                      {
            //                                                          var firstItem = o.Value.Items[0];
            //                                                          return firstItem;
            //                                                      }).GetEnumerator();
        }

        //public IEnumerable Fetch(DateTime start, DateTime end)
        //{
        //    var result = new List<ITerm>();
        //    var temp = start;
        //    while (temp <= end)
        //    {
        //        var dateKey = temp.IndexOf(_dateStorage);
        //        result.Add(_dailyContainer[dateKey][])
        //    }
        //    return _dailyContainer.OrderBy(o => o.Key).Select(o => o.Value.Items.FirstOrDefault());
        //}

        public IEnumerable<Term> SelectTargetTerms(Func<Term, bool> predicate)
        {
            return Schedule.TermSet.Where(predicate);
        }

        /// <summary>
        /// Get Assignment
        /// </summary>
        /// <param name="dayOfIndex">hrDate</param>
        /// <returns>Assignment</returns>
        public ITerm this[int dayOfIndex]
        {
            get
            {
                if (!_dailyContainer.ContainsKey(dayOfIndex))
                    return default(ITerm);
                return _dailyContainer[dayOfIndex].Items[0];
            }
        }

        public DateTerm this[DateTime date]
        {
            get
            {
                if (!_dailyRecords.ContainsKey(date))
                    return null;
                return _dailyRecords[date];

            }
            set { _dailyRecords[date] = value; }
        }

        public object GetItem(int index)
        {
            return this[index];
        }

        private int _hashCode;

        public override int GetHashCode()
        {
            if (Schedule != null)
                _hashCode = Schedule.GetHashCode();

            return _hashCode;
        }

        public virtual int GetHashCode(PlanningAgent obj)
        {
            return obj.GetHashCode();
        }

        public virtual bool Equals(PlanningAgent other)
        {
            if (ReferenceEquals(this, other)) return true;
            return this.Schedule.Equals(other.Schedule);
        }

        public virtual bool Equals(ISimpleEmployee other)
        {
            return Profile.Equals(other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PlanningAgent;
            if (other == null)
            {
                var otherEmployee = obj as ISimpleEmployee;
                if (otherEmployee == null) return false;

                return this.Equals(otherEmployee);
            }
            return this.Equals(other); //other.LaborRule.Equals(LaborRule) &&
        }

        public bool Equals(PlanningAgent x, PlanningAgent y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        ITerm IDateIndexer<ITerm>.this[DateTime date]
        {
            get
            {
                var dateKey = date.IndexOf(_timeBox.Boundary.Start);

                if (!_dailyContainer.ContainsKey(dateKey))
                    return default(ITerm);
                return _dailyContainer[dateKey].Items[0];
            }
            set { }
        }
    }
}