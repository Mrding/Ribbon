using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Infrastructure.Domain;
using Luna.Common;
using System;
using System.Collections.Generic;
using Luna.Shifts.Domain;
using System.Collections;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IAddSeatEventPresenter : IPresenter
    {
        ITerm CanChooseRange { get; set; }

        DateTime SelectedDate { get; set; }

        IEnumerable SelectedSeats { get; set; }
    }
}