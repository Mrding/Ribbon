namespace Luna.WPF.ApplicationFramework.Validations
{
    using System.Windows;

    public interface IValidator : IPriority, IValidatorTrigger
    {
        void Validate();

        void ValidShow();

        string ErrorMessage { get; set; }

        object Property { get; set; }

        void SetErrorStyleOrTemplate();

        /// <summary>
        /// Gets or sets a value that indicates whether a control will do validate when first loaded
        /// </summary>
        bool InitializeValidate { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a control is valided
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether Property Changed,the control will do validate after GapValidateSecond time
        /// </summary>
        double GapValidateSecond { get; set; }
        
    }

    public interface IValidatorTrigger
    {
        FrameworkElement TriggerElement { get; set; }
    }


}
