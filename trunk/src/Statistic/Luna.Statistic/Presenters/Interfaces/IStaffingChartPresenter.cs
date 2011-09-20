using System.ComponentModel;
using System.Windows.Input;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Statistic.Domain;

namespace Luna.Statistic.Presenters.Interfaces
{
    public interface IStaffingChartPresenter : IPresenter
    {
        CommandBindingCollection CommandBindings { get; }

        ICollectionView StaffingStatistics { get; }

        bool AnalyisComplete { get; }
        
        //object GetCurrentQueue();

        //void Estimate(IStaffingStatistic staffingStatistic);

    }

 
}