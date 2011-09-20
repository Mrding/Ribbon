using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiproSoftware.Windows.Controls.Docking;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Luna.WPF.ApplicationFramework
{
    public interface IDockSitePresenter
    {
        DateTime WatchPoint { get; set; }

        IPresenter ActivePresenter { get; }

        Action<DateTime> WatchPointChanged { get; set; }

        void Show(object presenter);

        void Activate(object presenter);

        T GetActiveModel<T>() where T : class;

        T FirstOrDefault<T>(Func<T, bool> predicate) where T : IPresenter;

        bool Contains<T>() where T : IPresenter;

        bool TryCloseAllWindows();

        //DockingWindow Get
    }
}