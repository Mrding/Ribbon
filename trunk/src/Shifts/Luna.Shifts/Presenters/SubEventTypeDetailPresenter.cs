using Caliburn.Core.Metadata;
using Luna.Common;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework.Presenters;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(ISubEventTypeDetailPresenter))]
    public class SubEventTypeDetailPresenter : DetailPresenter<TermStyle>, ISubEventTypeDetailPresenter
    {
        public int TimeLength
        {
            get { return Entity.TimeRange.Length; }
            set
            {
                Entity.TimeRange = new TimeValueRange(0, value);
                NotifyOfPropertyChange(() => TimeLength);
            }
        }
    }
}
