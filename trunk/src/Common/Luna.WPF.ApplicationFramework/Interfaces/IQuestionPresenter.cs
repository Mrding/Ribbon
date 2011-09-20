using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Interfaces
{
    public interface IQuestionPresenter : IMessagePresenter
    {
        Answer Answer { get; set; }

        bool Editable { get; set; }

        string Comments { get; set; }

        bool ClosingConfirmModeOn { get; set; }

        bool SavingConfirmModeOn { get; set; }

        string ConfirmDelegate { get; set; }

        object Invoker { get; set; }
    }

    public interface IMessagePresenter : IPresenter
    {
        string Text { get; set; }

        /// <summary>
        /// More detail text
        /// </summary>
        string Details { get; set; }
    }
}
