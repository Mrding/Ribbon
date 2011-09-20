namespace Luna.WPF.ApplicationFramework.Validations
{
    using System.Windows;
    using System.Windows.Controls;

    public abstract partial class Validator : Control
    {
        /// <summary>
        /// Get or set this validation is passed
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(Validator),
            new UIPropertyMetadata(false, new PropertyChangedCallback(IsValidPropertyChanged)));

        private static void IsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Validator validator = d as Validator;
            validator.ValidPropertyChanged();
        }

        /// <summary>
        ///Get or Set the property's origin element where from
        ///If you set the property's bindingexpression and Source has the property
        ///Source will be setted automation  This is a dependency property
        /// </summary>
        public FrameworkElement Source
        {
            get { return (FrameworkElement)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(FrameworkElement), typeof(Validator),
            new UIPropertyMetadata((o,a)=>{
                if(a.NewValue==null) return;
                var source=a.NewValue as DependencyObject;
                var oldsource = a.OldValue as DependencyObject;
                var element = o as Validator;
                var list = ValidationService.GetValidators(source);
                element.ValidateUI = new DefaultValidateUI() { Element = source as FrameworkElement };
                if (!list.Contains(element))
                {
                    list.Add(element);
                }
                if (oldsource != null)
                {
                    list.Remove(element);
                }
            }));

        /// <summary>
        /// The Property want to valid
        /// </summary>
        public object Property
        {
            get { return (object)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof(object), typeof(Validator),
            new UIPropertyMetadata(new PropertyChangedCallback(ValidPropertyPropertyChanged)));

        /// <summary>
        /// external triggerElement will do valid
        /// </summary>
        public FrameworkElement TriggerElement
        {
            get { return (FrameworkElement)GetValue(TriggerElementProperty); }
            set
            {
                SetValue(TriggerElementProperty, value);
            }
        }

        public static readonly DependencyProperty TriggerElementProperty =
            DependencyProperty.Register("TriggerElement", typeof(FrameworkElement), typeof(Validator),
            new UIPropertyMetadata(new PropertyChangedCallback(TriggerElementPropertyChanged)));

        public IValidateUI ValidateUI
        {
            get { return (IValidateUI)GetValue(ValidateUIProperty); }
            set { SetValue(ValidateUIProperty, value); }
        }

        public static readonly DependencyProperty ValidateUIProperty =
            DependencyProperty.Register("ValidateUI", typeof(IValidateUI), typeof(Validator),
            new UIPropertyMetadata());

        public Style ToolTipStyle
        {
            get { return (Style)GetValue(ToolTipStyleProperty); }
            set { SetValue(ToolTipStyleProperty, value); }
        }
        public static readonly DependencyProperty ToolTipStyleProperty =
            DependencyProperty.Register("ToolTipStyle", typeof(Style), typeof(Validator),
            new UIPropertyMetadata(null));


        /// <summary>
        /// Get or set the value which indicate that when validator in current visualtree,validator will do valid on onload event only once
        /// </summary>
        public bool InitializeValidate
        {
            get { return (bool)GetValue(InitializeValidateProperty); }
            set { SetValue(InitializeValidateProperty, value); }
        }

        public static readonly DependencyProperty InitializeValidateProperty =
            DependencyProperty.Register("InitializeValidate", typeof(bool), typeof(Validator),
            new UIPropertyMetadata(false, new PropertyChangedCallback(InitializeValidatePropertyChanged)));

        /// <summary>
        ///Get or set EnableNotSourceContext of an element
        ///When Set false,if Source don't have DataContext,validator will not valid.In case of empty valid.
        ///This is a dependency property
        /// </summary>
        public bool EnableNotSourceContext
        {
            get { return (bool)GetValue(EnableNotSourceContextProperty); }
            set { SetValue(EnableNotSourceContextProperty, value); }
        }

        public static readonly DependencyProperty EnableNotSourceContextProperty =
            DependencyProperty.Register("EnableNotSourceContext", typeof(bool), typeof(Validator),
            new UIPropertyMetadata(true));

        public static TriggerType GetTriggerType(DependencyObject obj)
        {
            return (TriggerType)obj.GetValue(TriggerTypeProperty);
        }

        /// <summary>
        /// Set TriggerElement's type.when set store,triggerElement will not do valid
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTriggerType(DependencyObject obj, TriggerType value)
        {
            obj.SetValue(TriggerTypeProperty, value);
        }

        public static readonly DependencyProperty TriggerTypeProperty =
            DependencyProperty.RegisterAttached("TriggerType", typeof(TriggerType), typeof(Validator),
            new UIPropertyMetadata(TriggerType.Trigger, TriggerTypePropertyChanged));

        public static object GetValidateTarget(DependencyObject obj)
        {
            return (object)obj.GetValue(ValidateTargetProperty);
        }

        public static void SetValidateTarget(DependencyObject obj, object value)
        {
            obj.SetValue(ValidateTargetProperty, value);
        }

        public static readonly DependencyProperty ValidateTargetProperty =
            DependencyProperty.RegisterAttached("ValidateTarget", typeof(object), typeof(Validator),
            new UIPropertyMetadata(new PropertyChangedCallback(OnValidateTargetPropertyChanged)));

    }

    public enum TriggerType
    {
        Trigger,
        Store
    }
}
