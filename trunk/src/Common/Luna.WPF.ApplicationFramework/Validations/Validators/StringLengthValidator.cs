using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Validations
{
    public class StringLengthValidator : Validator
    {


        public int MaximumLength
        {
            get { return (int)GetValue(MaximumLengthProperty); }
            set { SetValue(MaximumLengthProperty, value); }
        }

        public static readonly DependencyProperty MaximumLengthProperty =
            DependencyProperty.Register("MaximumLength", typeof(int), typeof(StringLengthValidator), 
            new UIPropertyMetadata(10));

        public int MinimumLength
        {
            get { return (int)GetValue(MinimumLengthProperty); }
            set { SetValue(MinimumLengthProperty, value); }
        }

        public static readonly DependencyProperty MinimumLengthProperty =
            DependencyProperty.Register("MinimumLength", typeof(int), typeof(StringLengthValidator), 
            new UIPropertyMetadata(5));


        public override bool InnerValidate()
        {
            ErrorMessage = string.Format(ErrorMessage, MinimumLength, MaximumLength);
            int num = (Property == null) ? 0 : ((string)Property).Length;
            return ((Property == null) || ((num >= this.MinimumLength) && (num <= this.MaximumLength)));
        }

        protected override string DefaultErrorMessage
        {
            get { return ValidatorErrorMessage.StringLengthValidatorErrorMessage; }
        }
    }
}
