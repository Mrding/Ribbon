using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Luna.Core.Extensions;
using Luna.Statistic.Domain;

namespace Luna.Statistic.Presenters
{
    public partial class StaffingChartPresenter
    {
        public ICollectionView StaffingStatistics { get; private set; }

        private void LoadEstimationNeeds()
        {
            if (StaffingStatistics == null)
            {
                StaffingStatistics = CollectionViewSource.GetDefaultView(_serviceQueueContainer.GetStaffingStatistics());
                _staffingCalculatorData.CurrentChanged += StaffingCalculatorDataCurrentChangedOnEstimationMode;
            }

            if (StaffingStatistics.CurrentItem == null)
                StaffingStatistics.MoveCurrentToFirst();

            //StaffingStatistics.CurrentItem.SaftyInvoke<IStaffingStatistic>(o => o.Output().Invoke());
        }

        void StaffingCalculatorDataCurrentChangedOnEstimationMode(object sender, EventArgs e)
        {
            _staffingCalculatorData.CurrentItem.SaftyInvoke<StatisticRaw>(q =>
            {
                StaffingStatistics.OfType<IStaffingStatistic>().FirstOrDefault(o => o.Entity.Equals(q.Source.Entity))
                    .SaftyInvoke(o =>
                    {
                        o.Output().Invoke();
                        StaffingStatistics.MoveCurrentTo(o);
                    });
            });
        }
    }
}
