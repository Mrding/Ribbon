using Luna.Common;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface ISiteExplorerPresenter : IDockablePresenter, IPredicate<Site>, IRefresh
    {
        ICampaignSites[] CampaignSites { get; }

        object SelectedEntity { get; }

        //void LoadData();

        void LaunchPrioritySetup();

        //object ReloadSiteContentDetail();

        //ISeatingSite ReloadSiteForSeating(ISeatingSite target);
    }
}