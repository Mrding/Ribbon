using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Shifts.Domain.Model;
using System.Collections;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftDispatcherPresenter
    {
        private DateTime? _lastWatchPoint;

        //private IRealTimeAdherenceModel _readTimeAdherenceModel;

        private void UpdateAdherence(IEnumerable agents)
        {
          
            //reading date from db
            /*var results = _readTimeAdherenceModel.GetAdherenceBlocks(
                agents,
                o => o.SaftyGetProperty<TimeBox, IAgent>(a => a.Schedule),
                GetMonitoringRange(), FullyRefresh.If(f => f.Invoke()) == true,
                (o, seat) => o.SaftyInvoke<IWorkingAgent>(a => a.CurrentSeat = seat));

            AgentAdherences = results;
            FullyRefresh = () =>
                               {
                                   return GetMonitoringRange().End != _lastWatchPoint;
                               };

            UpdateRtaaStatistic(results);*/
        }

        private void UpdateRtaaStatistic(IList<IEnumerable> list)
        {
            //var rtaaStatistic = Container.Resolve<IRtaaStatisticPresenter>();
            //(list == null ? null : this).Self(o =>
            //{
            //    if (o == null)
            //        rtaaStatistic.Shutdown();
            //    else
            //    {
            //        rtaaStatistic.ShiftViewerPresenter = o;
            //        rtaaStatistic.MonitoringTime = MonitoringPoint;
            //        rtaaStatistic.UpdateData(list);
            //    }
            //});
        }

        private void StartMonitoringAdherence()
        {
            
            /*if (!EnableRtaa)
            {
                //try set rtaa auto runing, if true means rtaa can auto running
                if (!RedirectToScreenStart(true))
                {
                    //try set monitoring time as a center of screen at first time
                    MonitoringPoint = _lastWatchPoint ?? GetCenterOfScreenTime();
                }
                if (_readTimeAdherenceModel == null)
                    _readTimeAdherenceModel = Container.Resolve<IRealTimeAdherenceModel>();
                _refresh += UpdateAdherence;
                UpdateAdherence(BindableAgents);
            }
            else
            {
                _refresh -= UpdateAdherence;
                AgentAdherences = null;
                FullyRefresh = () => true; //initial refresh always true
                UpdateRtaaStatistic(null);
                _readTimeAdherenceModel.SaftyInvoke<IDisposable>(d => d.Dispose());
            }

            _lastWatchPoint = MonitoringPoint;*/
        }


        private void OpenRtaaStatistic()
        {
            //var current = FirstOrDefault<IRtaaStatisticPresenter>(o => o != null);
            //if (current != null)
            //    Activate(current);
            //else
            //    Open<IRtaaStatisticPresenter>();
        }
    }
}
