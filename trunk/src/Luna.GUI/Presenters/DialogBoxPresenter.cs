using System;
using System.ComponentModel;
using System.Windows;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.GUI.Views;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.GUI.Presenters
{
    [View(typeof(DialogBoxView))]
    [PerRequest(typeof(IDialogBoxPresenter))]
    public class DialogBoxPresenter : Presenter, IDialogBoxPresenter, IDataErrorInfo
    {
        private bool? _isCancel;
        private object _invoker;

        public DialogBoxPresenter()
        {
            _isCancel = true;
            _text = string.Empty;
            DialogPosition = VerticalAlignment.Top;
            HasInputField = true;
            CanConfirmDelegate = text => true;
        }

        public bool HasInputField { get; set; }

        public string BackgroundTitle { get; set; }

        public object Invoker
        {
            get { return _invoker; }
            set { _invoker = value; NotifyOfPropertyChange(() => Invoker); }
        }

        private string _confirmDelegate;
        public string ConfirmDelegate
        {
            get { return _confirmDelegate; }
            set { _confirmDelegate = value; NotifyOfPropertyChange(() => ConfirmDelegate); }
        }

        private VerticalAlignment _dialogPosition;
        public VerticalAlignment DialogPosition
        {
            get { return _dialogPosition; }
            set { _dialogPosition = value; NotifyOfPropertyChange(() => DialogPosition); }
        }

        public string Message { get; set; }

        public string PartView { get; set; }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; NotifyOfPropertyChange(() => Text); }
        }
        
        /// <summary>
        /// 三种状态控制, true = 异常阻塞, false = 通过(Pass), null = 取消操作
        /// </summary>
        public bool? IsCancel
        {
            get { return _isCancel; }
            set { _isCancel = value; NotifyOfPropertyChange(() => IsCancel); }
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { _hasError = value; NotifyOfPropertyChange(() => HasError); }
        }

        public Func<string, bool> CanConfirmDelegate { get; set; }

        public bool TextIsNotEmpty()
        {
            if (!HasInputField) return true;

            var trimedText = Text.Trim();

            return !string.IsNullOrEmpty(trimedText) && CanConfirmDelegate(trimedText);
        }

        protected override void OnShutdown()
        {
            if (IsCancel != false) //return; //当按下confrim并且无发生任何错误
                IsCancel = null; // 作为引发Indeterminate事件目的

            Invoker = null;
            base.OnShutdown();
        }

        [Preview("TextIsNotEmpty")]
        [Dependencies("Text")]
        public void Confirm(UIElement element)
        {
            IsCancel = false;
        }

        public void ConfrimCallback(Exception ex)
        {
            if (ex == null)
            {
                CanConfirmDelegate = null;
                Close();
            }
            else
            {
                HasError = true;
                IsCancel = true;
                Error = ex.Message;
            }
        }

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            private set { _error = value; NotifyOfPropertyChange(() => Error); }
        }
    }
}

