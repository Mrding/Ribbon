using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;

namespace Luna.WPF.ApplicationFramework.Validations
{
    using Luna.WPF.ApplicationFramework.Validations;

    public class ValidatorCollection : FreezableCollection<Validator>
    {
      
        protected override void OnChanged()
        {
            
            base.OnChanged();
        }
    }

    public class ValidationService
    {
        /// <summary>
        /// Get the Error's Collection of Source,This is a dependency property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ObservableCollection<PriorityValidationError> GetErrors(DependencyObject obj)
        {
            if (obj.GetValue(ErrorsProperty) == null)
                obj.SetValue(ErrorsProperty, new ObservableCollection<PriorityValidationError>());
            return (ObservableCollection<PriorityValidationError>)obj.GetValue(ErrorsProperty);
        }

        private static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.RegisterAttached("Errors", typeof(ObservableCollection<PriorityValidationError>), typeof(ValidationService),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Get Validators of the TriggerElement,This is a dependency property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ValidatorCollection GetValidators2(DependencyObject obj)
        {
           
            if (obj.GetValue(Validators2Property) == null)
                obj.SetValue(Validators2Property, new ValidatorCollection());
            return (ValidatorCollection)obj.GetValue(Validators2Property);
        }

        public static readonly DependencyProperty Validators2Property =
            DependencyProperty.RegisterAttached("Validators2Internal", typeof(ValidatorCollection), typeof(ValidationService),
            new UIPropertyMetadata(null));

        public static List<Validator> GetValidators(DependencyObject obj)
        {
            if (obj.GetValue(ValidatorsProperty) == null)
                obj.SetValue(ValidatorsProperty, new List<Validator>());
            return (List<Validator>)obj.GetValue(ValidatorsProperty);
        }

        public static readonly DependencyProperty ValidatorsProperty =
            DependencyProperty.RegisterAttached("Validators", typeof(List<Validator>), typeof(ValidationService),
            new UIPropertyMetadata(null));

        public static void Validate(FrameworkElement element)
        {
            var validators = ValidationService.GetValidators(element);
            if (validators.Count == 0) return;
            var errList = new List<bool>();
            ValidationService.SetIsPause(element, true);
            foreach (var item in validators)
            {
                item.Validate();
                if (!item.IsValid)
                    errList.Add(item.IsValid);
            }
            ValidationService.SetIsPause(element, false);
            if (errList.Count > 0)
                validators[0].ValidShow();
        }

        public static bool GetIsPause(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPauseProperty);
        }

        public static void SetIsPause(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPauseProperty, value);
        }

        public static readonly DependencyProperty IsPauseProperty =
            DependencyProperty.RegisterAttached("IsPause", typeof(bool), typeof(ValidationService),
            new UIPropertyMetadata(false));


        public static bool GetHasError(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(HasErrorProperty);
        }

        internal static readonly DependencyPropertyKey HasErrorPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HasError", typeof(bool), typeof(ValidationService),
            new UIPropertyMetadata(false));

        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

    }
}
