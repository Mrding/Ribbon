using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Luna.Common;
using Luna.Shifts.Domain;

namespace Luna.Statistic.Domain.Service
{
    public interface IStaffingCalculatorService
    {
        bool Ready { get; }

        void Run(IDictionary args);

        DateRange GetViewRange();

        IEnumerable<IAgent> GetAgents();

        void Hide();

        void Show(double height);

        IStaffingStatistic BeginEstimation();

        void Shutdown();

        void Callback();
    }
}
