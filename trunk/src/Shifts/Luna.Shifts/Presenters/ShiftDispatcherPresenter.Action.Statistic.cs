using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Luna.Common;
using Caliburn.PresentationFramework.ApplicationModel;
using System.Windows.Data;
using Luna.Common.Extensions;
using Luna.Statistic.Domain.Service;
using Luna.Shifts.Domain;
using Luna.Statistic.Domain;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftDispatcherPresenter
    {
        private IStaffingCalculatorService _staffingCalculatorService;
        private Action _buildStaffingChart;
     
        private IDictionary _staffingCalculatorArgs;

        public void RegisterRunStaffingChart(FrameworkElement el)
        {
            //var scheduleCellCapacity = Convert.ToInt32(_schedule.GetLength().TotalDays + Keys.TailDayAmount) * 96;

            _staffingCalculatorArgs = new Dictionary<string, object>
                                          { 
                                              {"contentView", el},
                                              {"getAgents", new Func<IEnumerable<IAgent>>(() => _attendances)},
                                              {"getViewRange", new Func<DateRange>(() => new DateRange(GetWatchPoint(), ScreenEnd))},
                                        
                                              {"schedule", _schedule},
                                              {"Invoker", this}
                                          };

            _buildStaffingChart = () =>
            {
                if (_staffingCalculatorService != null) return;
                _staffingCalculatorService = Container.Resolve<IStaffingCalculatorService>(_staffingCalculatorArgs);
            };
        }

        public void ShowStaffingCalculatorDialog()
        {
            _buildStaffingChart();
            _staffingCalculatorService.Run(_staffingCalculatorArgs);
        }

       
    }


}
