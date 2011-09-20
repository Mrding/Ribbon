using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public class BasicAssignmentType : TermStyle, IEnumerable<ICanConvertToValueTerm>, ITermContainer
    {
        private readonly IList<ICanConvertToValueTerm> _termSet;

        public BasicAssignmentType()
        {
            _timeRange = new TimeValueRange(480, 1020);
            _background = "BurlyWood";
            _onService = true; 
            _termSet = new List<ICanConvertToValueTerm> {this};
        }

        /// <summary>
        /// AddSubEventInsertRule with checking IsCoverd, IsInTheRange
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public virtual bool AddSubEventInsertRule(SubEventInsertRule rule)
        {
            //GetEnumerator();

            rule.SetNewTime(Start.AddMinutes(rule.TimeRange.StartValue), default(DateTime));

            if (!rule.IsInTheRange(Start, End))
                return false;

            if (_subEventInsertRules.Any(o => !ReferenceEquals(rule, o) && rule.IsCoverd(o)))
                return false;

            _subEventInsertRules.Add(rule);
            _termSet.Clear();
            _termSet.Add(this);
            SubEventInsertRules = _subEventInsertRules;
            return true;
        }

        public virtual bool RemoveSubEventInsertRule(SubEventInsertRule rule)
        {
            if (_subEventInsertRules.Remove(rule))
            {
                _termSet.Clear();
                _termSet.Add(this);
                SubEventInsertRules = _subEventInsertRules;
                return true;
            }
            return false;
        }

        public virtual IEnumerable<SubEventInsertRule> GetSubEventInsertRules()
        {
            return _subEventInsertRules.AsEnumerable();
        }

        public virtual bool IgnoreAdherence { get; set; }

        private bool _asAWork = true;
        public virtual bool AsAWork
        {
            get { return _asAWork; }
            set
            {
                _asAWork = value;
                if (_asAWork)
                    AsARest = false;
            }
        }

        /// <summary>
        /// 1440代表往前移动, 0代表当天.默认为0.
        /// </summary>
        public virtual int TodayIndicator { get; set; }

        public virtual bool GapGuaranteed { get; set; }

        public virtual string Purpose { get { return "adjustment"; } }

        private IList<SubEventInsertRule> _subEventInsertRules = new List<SubEventInsertRule>();
        public virtual IList<SubEventInsertRule> SubEventInsertRules
        {
            get { return _subEventInsertRules; }
            set { _subEventInsertRules = value; }
        }

        private void BuildTermSet()
        {
            if (_termSet.Count <= _subEventInsertRules.Count)
            {
                foreach (var item in _subEventInsertRules)
                {
                    item.SetNewTime(this.Start.AddMinutes(item.TimeRange.StartValue), default(DateTime));
                    _termSet.Add(item);
                    _termSet.Add(item.CreateVisibleSubEvent());
                }
            }
        }

        public virtual IEnumerator<ICanConvertToValueTerm> GetEnumerator()
        {
            BuildTermSet();
            return _termSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int Level
        {
            get { return 0; }
        }

        private DateTime _start;
        public override DateTime Start
        {
            get
            {
                if (_start == default(DateTime))
                    _start = DateTime.Parse("2000/1/1").Date.AddMinutes(TimeRange.StartValue);
                return _start;
            }
        }

        private DateTime _end;
        public override DateTime End
        {
            get
            {
                if (_end == default(DateTime))
                    _end = Start.AddMinutes(TimeRange.Length);
                return _end;
            }
        }

        public override bool SetNewTime(DateTime start, DateTime end)
        {
            if (end <= start) 
                return false;
            
            var newStartValue = TimeRange.StartValue + Convert.ToInt32(start.Subtract(Start).TotalMinutes);
            var newEndValue = TimeRange.EndValue + Convert.ToInt32(end.Subtract(End).TotalMinutes);

            var newTimeValueRange = new TimeValueRange(newStartValue, newEndValue);
            if (newTimeValueRange.Invalid)
                return false;

            var oldTimeRangeValue = _timeRange;

            _start = default(DateTime); // reset 会在 get Start 属性时发生重新赋值动作
            _end = default(DateTime); // reset 会在 get End 属性时发生重新赋值动作
            _termSet.Clear();
            _termSet.Add(this);
            
            TimeRange = new TimeValueRange(newStartValue, newEndValue);

            BuildTermSet(); // 重新填充 termSet

             if(_subEventInsertRules.Any(s => !s.IsInTheRange(start, end)))
             {
                 _start = default(DateTime);
                 _end = default(DateTime);
                 _termSet.Clear();
                 _termSet.Add(this);
                 TimeRange = oldTimeRangeValue;
                 BuildTermSet();
                 return false;
             }
            return true;
        }

        //private string _entityPrint;
        public virtual string GetEntityPrint()
        {
            //if (string.IsNullOrEmpty(_entityPrint))
            //{
            var stringBuilder = new StringBuilder();
            foreach (var item in SubEventInsertRules)
            {
                stringBuilder.AppendFormat("{0}!{1}", item.SubEvent.Name, item.OccurScale);
            }

            return stringBuilder.ToString();
            //}

            //return _entityPrint;
        }

        public virtual void Rebuild()
        {
            _start = default(DateTime);
            _end = default(DateTime);
            _termSet.Clear();
            _termSet.Add(this);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Term;
            if (other == null)
                return base.Equals(obj);
            return other.Text == Text;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual IEnumerable GetAllTerms()
        {
            return this.ToList();
        }

        public virtual IEnumerable Fetch(DateTime start, DateTime end)
        {
            return this.OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
        }

        //IList ITermContainer.Result { get { return null; } }

        //void ITermContainer.SetTime(ITerm term, DateTime start, DateTime end, Action<ITerm, bool> callback) { }
    }
}