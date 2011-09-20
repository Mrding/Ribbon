namespace Luna.WPF.ApplicationFramework.Validations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Reflection;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Luna.WPF.ApplicationFramework.Threads;
    using Luna.WPF.ApplicationFramework.Extensions;

    public abstract partial class Validator : Control, IValidator
    {
        static Validator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Validator), new FrameworkPropertyMetadata(typeof(Validator)));
        }

        public Validator()
        {
            Key = Guid.NewGuid();
        }

        private static void InitializeValidatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Validator validator = d as Validator;
            if ((bool)e.NewValue)
                validator.LoadOnce(validator.Validate, () => { return validator.FindAncestor<Window>() != null; });
        }

        public void Validate()
        {
            if (HasSourceContext)
            {
                var tempValid = IsValid;
                IsValid = InnerValidateWithError();
                if (tempValid == IsValid && IsValid == false)
                {
                    ValidPropertyChanged();
                }
            }
            else
            {
                IsValid = true;
            }
        }

        public void ValidShow()
        {
            if (ValidateUI == null) return;
            ValidateUI.FailShow();
        }

        private void ValidPropertyChanged()
        {
            Source.SetValue(ValidationService.HasErrorPropertyKey,!IsValid);
            
            if (IsValid)
                RemoveError();
            else
            {
                AddError();
                if (!ValidationService.GetIsPause(Source))
                {
                    if (ValidateUI == null) return;
                    ValidateUI.FailShow();
                }
            }
        }

        private void RemoveError()
        {
            var errors = ValidationService.GetErrors(Source);
            var error = errors.Where(e => e.ValidatorKey == this.Key).ToArray();
            if (error.Length > 0)
                errors.Remove(error[0]);
        }

        private void AddError()
        {
            var errors = ValidationService.GetErrors(Source);
            var validationError = new PriorityValidationError() { Priority = this.Priority, ErrorContent = ErrorMessage, ValidatorKey = this.Key };
            if (!errors.Any(e => e.ValidatorKey == this.Key))
                errors.Add(validationError);
        }

        public int Priority { get; set; }
        public Guid Key { get; set; }
        private DateTime gapTime = DateTime.MinValue;

        private void SetSourceFromProperty()
        {
            if (FrameworkElementService.GetIsFirstVisit(this))
            {
                var expression = this.GetBindingExpression(PropertyProperty);
                if (expression != null && this.Source == null)
                    this.SetValue(Validator.SourceProperty, expression.DataItem as FrameworkElement);
            }
            
        }

        protected static void ValidPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Validator validator = d as Validator;
            validator.SetSourceFromProperty();

            if (validator.HasSourceContext)
            {
                UIThread.BeginInvoke(() =>
                {
                    var isValid = validator.IsValid;
                    validator.IsValid = validator.InnerValidateWithError();
                    if (!isValid && !validator.IsValid && !FrameworkElementService.GetIsFirstVisit(validator))
                    {
                        validator.GapValidate();
                    }
                }, DispatcherPriority.Normal);
            }
            else
            {
                validator.IsValid = true;
            }
            FrameworkElementService.SetIsFirstVisit(validator, false);
        }

        /// <summary>
        /// Get Whether Source has DataContext
        /// </summary>
        private bool HasSourceContext
        {
            get
            {
                return this.EnableNotSourceContext || (!this.EnableNotSourceContext && this.Source.DataContext != null);
            }
        }

        private GapDispatcherTimer gapValidateTimer = null;

        /// <summary>
        /// display error message with gap time when not valid
        /// </summary>
        public double GapValidateSecond { get; set; }

        private void GapValidate()
        {
            if (gapValidateTimer == null) gapValidateTimer = new GapDispatcherTimer();
            gapValidateTimer.Invoke(GapValidateSecond, ValidPropertyChanged);
        }

        private static void TriggerElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Validator validator = d as Validator;

            if (FrameworkElementService.GetIsFirstVisit(validator.TriggerElement))
            {
                validator.RegisterTriggerValidationEvent();
                FrameworkElementService.SetIsFirstVisit(validator.TriggerElement, false);
            }

            ValidationService.GetValidators(validator.TriggerElement).Add(validator);
        }

        private void RegisterTriggerValidationEvent()
        {
            TriggerElement.PreviewMouseLeftButtonDown += TriggerElementValidate;
        }

        private void CancelTriggerValidationEvent()
        {
            TriggerElement.PreviewMouseLeftButtonDown -= TriggerElementValidate;
        }

        private void TriggerElementValidate(object sender, MouseButtonEventArgs e)
        {
            TriggerElement.Focus();
            e.Handled = !TriggerValidate(this.TriggerElement);
        }

        //success return true
        public static bool TriggerValidate(FrameworkElement TriggerElement)
        {
            var errList = new List<bool>();
            var sourceList = new List<FrameworkElement>();
            foreach (var item in ValidationService.GetValidators(TriggerElement))
            {
                if (!sourceList.Contains(item.Source))
                {
                    sourceList.Add(item.Source);
                    ValidationService.Validate(item.Source);
                }
                //ValidationService.Validate(item.Source);
                // item.Validate();
                errList.Add(item.IsValid);
            }
            if (errList.Count == 0) return true;
            return !errList.Any(err => err == false);
        }

        public static bool TriggerOuterValidate(FrameworkElement TriggerElement)
        {
            var errList = new List<bool>();
            foreach (var item in ValidationService.GetValidators(TriggerElement))
            {
                item.Validate();
                errList.Add(item.IsValid);
            }
            if (errList.Count == 0) return true;
            return !errList.Any(err => err == false);
        }

        private static void TriggerTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement target = sender as FrameworkElement;
            UIThread.BeginInvoke(() =>
            {
                var validators = ValidationService.GetValidators(target);
                if (validators.Count == 0) return;
                var triggerType = (TriggerType)args.NewValue;
                validators[0].CancelTriggerValidationEvent();
                if (triggerType == TriggerType.Trigger)
                    validators[0].RegisterTriggerValidationEvent();
            });
        }

        private static void OnValidateTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement outer = d as FrameworkElement;
            FrameworkElement innter = e.NewValue as FrameworkElement;
            if (innter != null)
            {
                Validator.SetTriggerType(innter, TriggerType.Store);
                outer.SetValue(Validator.OuterTiggerProperty, innter);
                new ExterValidateBehavior(outer, innter);
            }
        }

        public static FrameworkElement GetOuterTigger(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(OuterTiggerProperty);
        }

        public static void SetOuterTigger(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(OuterTiggerProperty, value);
        }

        
        public static readonly DependencyProperty OuterTiggerProperty =
            DependencyProperty.RegisterAttached("OuterTigger", typeof(FrameworkElement), typeof(Validator), 
            new UIPropertyMetadata(null));

        

        class ExterValidateBehavior
        {
            private FrameworkElement _outerElement;
            private FrameworkElement _innerElement;
            public ExterValidateBehavior(FrameworkElement outerElement, FrameworkElement innerElement)
            {
                _outerElement = outerElement;
                _innerElement = innerElement;
                _outerElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(_outerElement_PreviewMouseLeftButtonDown);
            }

            void _outerElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                _outerElement.Focus();
                var errList = new List<bool>();
                foreach (var item in ValidationService.GetValidators(_innerElement))
                {
                    item.Validate();
                    errList.Add(item.IsValid);
                }
                e.Handled = errList.Any(err => err == false);
            }
        }

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(Validator), 
            new UIPropertyMetadata());

        protected virtual string DefaultErrorMessage
        {
            get
            {
                return ValidatorErrorMessage.DefaultErrorMessage;
            }
        }

        #region IValidator Members

        private bool InnerValidateWithError()
        {
            if (Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode)return true;
            SetErrorMessage();
            return InnerValidate();
        }

        public abstract bool InnerValidate();
        public virtual void SetErrorMessage()
        {
            if (string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = DefaultErrorMessage;
        }

        private void SetSourceHasError(bool val)
        {
            Source.SetValue(sourceHasErrorPropertyKey, val);
        }

        private DependencyPropertyKey sourceHasErrorPropertyKey
        {
            get
            {
                return typeof(System.Windows.Controls.Validation).
                  GetField("HasErrorPropertyKey", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as DependencyPropertyKey;
            }
        }

        private void SetSourceErrors(ReadOnlyObservableCollection<ValidationError> val)
        {
            Source.SetValue(sourceErrorsPropertyKey, val);
        }

        private DependencyPropertyKey sourceErrorsPropertyKey
        {
            get
            {
                return typeof(System.Windows.Controls.Validation).
                  GetField("ErrorsPropertyKey", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as DependencyPropertyKey;
            }
        }

        private void AddTextBoxErrResource()
        {
            Style style = new Style(typeof(TextBox));
            Trigger trigger = new Trigger() { Property = Validation.HasErrorProperty, Value = true };
            Setter triggerSetter = new Setter(Control.ToolTipProperty, ErrorMessage);
            trigger.Setters.Add(triggerSetter);
            style.Triggers.Add(trigger);
            Source.Resources.Add(typeof(TextBox), style);
        }

        public virtual void SetErrorStyleOrTemplate()
        {
            AddTextBoxErrResource();
        }

        #endregion
    }
}
