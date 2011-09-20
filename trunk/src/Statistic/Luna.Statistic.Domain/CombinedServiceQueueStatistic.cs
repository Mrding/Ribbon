using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Core.Extensions;

namespace Luna.Statistic.Domain
{
    public class CombinedServiceQueueStatistic : IServiceQueueStatistic
    {
        private double[] _ahts;
        private double[] _cvs;
        private double[] _goalsExt;
        private double[] _dailyCVs;
        private int _coverageDays;

        private IEnumerable<IServiceQueueStatistic> _serviceQueues;

        public CombinedServiceQueueStatistic(IEnumerable<IServiceQueueStatistic> serviceQueues, int coverageDays)
        {
            _serviceQueues = serviceQueues;
            _coverageDays = coverageDays;

            (_coverageDays * 96).Self(c =>
                                         {
                                             Capacity = c;
                                             _ahts = new double[c];
                                             _cvs = new double[c];
                                             _goalsExt = new double[c];
                                             ForceastStaffing = new double[c];
                                             AssignedStaffing = new double[c];
                                             AssignedServiceLevel = new double[c];
                                             AssignedMaxStaffing = new double[c];
                                         });
            _dailyCVs = new double[_coverageDays];
            DailyAssignedServiceLevel = new double[_coverageDays];
        }

        public int Capacity { get; private set; }

        public double[] AHT { get { return _ahts; } }
        public double[] CV { get { return _cvs; } }

        public double[] DailyCV { get { return _dailyCVs; } }

        public double[] ServiceLevelGoal { get { return _goalsExt; } }

        public virtual double[] ForceastStaffing { get; set; }

        public virtual double[] AssignedStaffing { get; private set; }

        public double[] AssignedMaxStaffing { get; private set; }

        public virtual double[] AssignedServiceLevel { get; private set; }

        public double[] DailyAssignedServiceLevel { get; private set; }

        public void Concat(IDailyObject dailyObject)
        {
            throw new NotImplementedException();
        }

        public object Entity
        {
            get { return _serviceQueues; }
        }

        public virtual void Output()
        {
            var selectedQueueNames = new StringBuilder();
            var selectedQueues = _serviceQueues.Where(o =>
                                                          {
                                                              var selected = o.SaftyGetProperty<bool, ISelectable>(t => t.IsSelected == true);
                                                              if (selected)
                                                                  selectedQueueNames.AppendFormat(",{0}", o);
                                                              return selected;

                                                          }).ToArray();
            
            _displayText = selectedQueueNames.Length == 0 ? string.Empty : selectedQueueNames.ToString().Remove(0,1);

            var weightedSL = new double[Capacity];
            for (var i = 0; i < Capacity; i++)
            {
                var index = i;
                _cvs[i] = selectedQueues.Sum(o => o.CV[index]);

                if (_cvs[i] != 0)
                {
                    _ahts[i] = selectedQueues.Sum(o => o.AHT[index] * o.CV[index]) / _cvs[i];
                    _goalsExt[i] = selectedQueues.Sum(o => o.ServiceLevelGoal[index] * o.CV[index]) / _cvs[i];
                    AssignedServiceLevel[i] = selectedQueues.Sum(o => o.AssignedServiceLevel[index] * o.CV[index]) / _cvs[i];
                    weightedSL[i] = _cvs[i] * AssignedServiceLevel[i];
                }

                ForceastStaffing[i] = selectedQueues.Sum(o => o.ForceastStaffing[index]);
                AssignedStaffing[i] = selectedQueues.Sum(o => o.AssignedStaffing[index]);
                AssignedMaxStaffing[i] = selectedQueues.Sum(o => o.AssignedMaxStaffing[index]);
            }

            for (var i = 0; i < _coverageDays; i++)
            {
                var indexOfDate = i;
                _dailyCVs[i] = selectedQueues.Sum(o => o.DailyCV[indexOfDate]);

                if(_dailyCVs[i] == 0) continue;

                var dailyWeightedSL = new double[96];
                Array.Copy(weightedSL, indexOfDate * 96, dailyWeightedSL, 0, 96);
                DailyAssignedServiceLevel[indexOfDate] = _dailyCVs[i] == 0 ? 0d : dailyWeightedSL.Sum() / _dailyCVs[i];
            }
        }

        public void Reset(int index)
        {
            throw new NotImplementedException();
        }

        public void CalculateDailyMisc(int dayOfIndex)
        {
            throw new NotImplementedException();
        }

        private string _displayText;

        public override string ToString()
        {
            return _displayText;
        }
    }
}