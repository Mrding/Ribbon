using System.Collections.Generic;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
 
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IEditSchedulePresenter : IPresenter
    {
        IList<Organization> OrganizationTree { get; set; }

        Schedule Schedule { get; }

        IEnumerable<Campaign> Campaigns { get; set; }

        void ChangeCampaign(Campaign campaign);

        void Acept(RoutedEventArgs e);
    }
}
