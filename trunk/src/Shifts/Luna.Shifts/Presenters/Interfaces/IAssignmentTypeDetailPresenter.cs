using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    using Luna.WPF.ApplicationFramework.Interfaces;
    using System.ComponentModel;

    public interface IAssignmentTypeDetailPresenter : IDetailPresenter<BasicAssignmentType>
    {
        //xIBlockConverter BlockConverter { get; set; }

        object SelectedBlock { get; set; }

        ICollectionView SubEventRulesView { get; }
    }
}
