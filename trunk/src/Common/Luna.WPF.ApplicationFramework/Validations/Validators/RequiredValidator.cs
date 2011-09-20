namespace Luna.WPF.ApplicationFramework.Validations
{
    public class RequiredValidator : Validator
    {
        public override bool InnerValidate()
        {
            if (Property == null) return false;
            return !string.IsNullOrEmpty(Property.ToString().Trim());
        }

        protected override string DefaultErrorMessage
        {
            get { return ValidatorErrorMessage.RequiredValidatorErrorMessage; }
        }
    }
}
