using System.ComponentModel;
using Caliburn.Core.Metadata;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Shifts.Presenters
{
    [Singleton("Infomation", typeof(IBackstageTabPresenter))]
    public class ScheduleInfomationPresenter : DefaultPresenter, IBackstageTabPresenter
    {
        private readonly IShellPresenter _shell;
        private readonly IMaintenanceScheduleModel _maintenanceModel;

        public ScheduleInfomationPresenter(IShellPresenter shell, IMaintenanceScheduleModel maintenanceModel)
        {
            _shell = shell;
            _maintenanceModel = maintenanceModel;

            _shell.PropertyChanged += ShellPropertyChanged;
        }

        private void ShellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ActivePlan") return;

            CurrentPresenter = _shell.ActivePlan as IShiftComposerPresenter;
        }

        protected Schedule Entity { get { return _currentPresenter.Schedule as Schedule; } }


        private IShiftComposerPresenter _currentPresenter;
        public object CurrentPresenter
        {
            get { return _currentPresenter; }
            set { _currentPresenter = value as IShiftComposerPresenter; NotifyOfPropertyChange(() => CurrentPresenter); }
        }


        public void Post()
        {
            _maintenanceModel.Announce(Entity);
        }

        public void ClosePost()
        {
            _maintenanceModel.CloseAnnouncement(Entity);
        }

    }
}