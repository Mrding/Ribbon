using System;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Luna.WPF.ApplicationFramework.Interfaces
{
    public interface IShellPresenter : IPresenter
    {
        object ActivePlan { get; set; }
    }

    public interface IBackstageTabPresenter
    {

    }

    public interface  IShiftImportPresenter
    {
        
    }

   
}
