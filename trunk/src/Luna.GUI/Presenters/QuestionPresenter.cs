using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.GUI.Views;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.GUI.Presenters
{
    [View(typeof(MessageView))]
    [PerRequest(typeof(IMessagePresenter))]
    public class MessagePresenter : Presenter, IMessagePresenter
    {
        public string Text { get; set; }
        public string Details { get; set; }

        public void CopyDetaiolToClipboard()
        {
            Clipboard.SetText(Details);
        }
    }

    [View(typeof(QuestionView))]
    [PerRequest(typeof(IQuestionPresenter))]
    public class QuestionPresenter : Presenter, IQuestionPresenter
    {
        private Answer _answer;

        public QuestionPresenter()
        {
            SavingConfirmModeOn = true;
            _answer = Answer.Cancel;
        }

        public Answer Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                NotifyOfPropertyChange(() => Answer);
            }
        }

        public string Text { get; set; }

        public string Details { get; set; }

        private bool _editable;
        public bool Editable
        {
            get { return _editable; }
            set
            {
                _editable = value;
                NotifyOfPropertyChange(() => Editable);
            }
        }

        public void Yes()
        {
            Answer = Answer.Yes;
            this.Close();
        }

        public void No()
        {
            Answer = Answer.No;
            Close();
        }

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                NotifyOfPropertyChange(() => Comments);
            }
        }

        private string _confirmDelegate;
        public string ConfirmDelegate
        {
            get { return _confirmDelegate; }
            set { _confirmDelegate = value; NotifyOfPropertyChange(() => ConfirmDelegate); }
        }

        public bool ClosingConfirmModeOn { get; set; }

        public bool SavingConfirmModeOn { get; set; }

        private object _invoker;
        public object Invoker
        {
            get { return _invoker; }
            set { _invoker = value; NotifyOfPropertyChange(() => Invoker); }
        }

        protected override void OnShutdown()
        {
            Invoker = null;
            base.OnShutdown();
        }
    }
}
