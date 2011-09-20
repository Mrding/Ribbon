using System;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Luna.WPF.ApplicationFramework.Interfaces
{
    public interface IDialogBoxPresenter : IExtendedPresenter
    {
        string BackgroundTitle { get; set; }

        string Message { get; set; }

        string Text { get; set; }

        string PartView { get; set; }
       
        bool? IsCancel { get; }

        object Invoker { get; set; }

        string ConfirmDelegate { get; set; }

        VerticalAlignment DialogPosition { get; set; }

        bool HasInputField { get; set; }

        void ConfrimCallback(Exception ex);

        Func<string, bool> CanConfirmDelegate { get; set; }

    }

    public interface IOpenExcelFilePresenter : IExtendedPresenter
    {
        string FilePath { get; }
    }
}