using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    public class ServiceQueueStatistic : IServiceQueueStatistic, ISelectable
    {
        private readonly IDictionary<DateTime, int> _dateIndexer;
        private double[] _ahts;
        private double[] _cvs;
        private double[] _goals;
        private double[] _goalsExt;
        private double[] _weightedSL;

        private readonly int _hashCode;
        private ServiceQueue _serviceQueue;
        private double[] _dailyCVs;

        public ServiceQueueStatistic(ServiceQueue serviceQueue, IDictionary<DateTime, int> dateIndexer)
        {
            _dateIndexer = dateIndexer;
            AbadonRate = serviceQueue.AbandonRate / 100.0;

            (_dateIndexer.Count * 96).Self(c =>
                                               {
                                                   Capacity = c;
                                                   _ahts = new double[c];
                                                   _cvs = new double[c];
                                                   _weightedSL = new double[c];
                                                   AssignedStaffing = new double[c];
                                                   AssignedServiceLevel = new double[c];
                                                   AssignedMaxStaffing = new double[c];
                                                   //StaffingRequirement = new double[c];
                                               });

            (_dateIndexer.Count * 24).Self(c =>
                                               {

                                                   _goals = new double[c];

                                               });
            _dailyCVs = new double[dateIndexer.Count];
            DailyAssignedServiceLevel = new double[dateIndexer.Count];
            DailyForecastStaffing = new double[dateIndexer.Count];
            DailyAssignedStaffing = new double[dateIndexer.Count];


            _hashCode = serviceQueue.GetHashCode();
            _serviceQueue = serviceQueue;
            _dailyCalculationActions = new Dictionary<int, Action>();
        }

        public int Capacity { get; private set; }

        public int ServiceLevelSecond { get; private set; }
        public double AbadonRate { get; private set; }

        public double[] AHT { get { return _ahts; } }
        public double[] CV { get { return _cvs; } }

   

        public double[] ServiceLevelGoal { get { return _goalsExt; } }
        public virtual double[] ForceastStaffing { get; set; }

        public virtual double[] AssignedStaffing { get; private set; }

        //public virtual double[] StaffingRequirement { get; private set; }

        public double[] AssignedMaxStaffing { get; private set; }

        public virtual double[] AssignedServiceLevel { get; private set; }

        public virtual double[] DailyCV { get { return _dailyCVs; } }

        public virtual double[] DailyAssignedServiceLevel { get; private set; }

        public virtual double[] DailyForecastStaffing { get; private set; }

        public virtual double[] DailyAssignedStaffing { get; private set; }

        public virtual void Concat(IDailyObject dailyObject)
        {
            dailyObject.SaftyInvoke<ForecastTraffic>(t =>
                                                         {
                                                             var dateIndex = _dateIndexer[dailyObject.Date] * 96;
                                                             Array.Copy(t.AHTs, 0, _ahts, dateIndex, 96);
                                                             Array.Copy(t.CVs, 0, _cvs, dateIndex, 96);
                                                             _dailyCVs[_dateIndexer[dailyObject.Date]] = t.SumOfCVs; // daily CVs

                                                         });
            dailyObject.SaftyInvoke<ForecastServiceLevelGoal>(t =>
                                                                  {
                                                                      var dateIndex = _dateIndexer[dailyObject.Date] * 24;
                                                                      Array.Copy(t.Goals, 0, _goals, dateIndex, 24);
                                                                  });
        }

        public object Entity
        {
            get { return _serviceQueue; }
        }

        public virtual void CalculateForecastStaffing(double[][] shrinkage, int startWeekDayIndex)
        {
            var count = _dateIndexer.Count * 96;
            ForceastStaffing = ForecastLibrary.ForecastStaffing(count, _cvs, _ahts, AbadonRate, ServiceLevelSecond, _goals, shrinkage, startWeekDayIndex, out _goalsExt);
        }

        //public void CalculateStaffingRequirement(int index)
        //{
        //    StaffingRequirement[index] = ForceastStaffing[index] - AssignedStaffing[index];
        //}

        public virtual void CalculateAssignedServiceLevel(int index)
        {
            //(SL +  Math.Max( SL, Math.Min( Staff/CV , 1.0 ))) / 2

            var cv = _cvs[index];
            var staffing = this.AssignedStaffing[index];

            var staffRate = Math.Min(cv == 0 ? 0 : staffing / cv, 1.0);

            var sl = ForecastLibrary.GetServiceLevel(cv, _ahts[index], AbadonRate, ServiceLevelSecond, staffing);

            (sl < staffRate ? (sl + staffRate) / 2 : sl).Self(r =>
                                                                  {
                                                                      AssignedServiceLevel[index] = r;
                                                                      _weightedSL[index] = cv * r;
                                                                  });
        }

        public void Reset(int index)
        {
            AssignedMaxStaffing[index] = 0;
            AssignedStaffing[index] = 0;
            AssignedServiceLevel[index] = 0;
        }

        public void CalculateDailyMisc(int dayOfIndex)
        {
            if (!_dailyCalculationActions.ContainsKey(dayOfIndex))
            {
                _dailyCalculationActions[dayOfIndex] = () =>
                                                           {
                                                               //add below
                                                               ConvertToDailyAssignedServiceLevel(dayOfIndex);
                                                               //DailyForecastStaffing[dayOfIndex] = ConvertToDaily(dayOfIndex, ForceastStaffing);
                                                               //DailyAssignedStaffing[dayOfIndex] = ConvertToDaily(dayOfIndex, AssignedStaffing);

                                                           };
                _calculate += _dailyCalculationActions[dayOfIndex];
            }
        }


        private void ConvertToDailyAssignedServiceLevel(int dayOfIndex)
        {
            var dailyAnswered = new double[96];
            Array.Copy(_weightedSL, dayOfIndex * 96, dailyAnswered, 0, 96);
            DailyAssignedServiceLevel[dayOfIndex] = _dailyCVs[dayOfIndex] == 0 ? 0d : dailyAnswered.Sum() / _dailyCVs[dayOfIndex];
        }

        private static double ConvertToDaily(int dayOfIndex, Array values)
        {
            var dailyValues = new double[96];
            Array.Copy(values, dayOfIndex * 96, dailyValues, 0, 96);
            return dailyValues.Sum();
        }

        private IDictionary<int, Action> _dailyCalculationActions;
        private Action _calculate = () => { };

        public virtual void Output()
        {
            _calculate();
            _calculate = () => { };
            _dailyCalculationActions.Clear();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || _hashCode == obj.GetHashCode();
        }

        public override string ToString()
        {
            return _serviceQueue.Name;
        }

        public bool? IsSelected
        {
            get { return _serviceQueue.SaftyGetProperty<bool?, ISelectable>(o => o.IsSelected); }
            set { _serviceQueue.SaftyInvoke<ISelectable>(o => o.IsSelected = value); }
        }
    }
}