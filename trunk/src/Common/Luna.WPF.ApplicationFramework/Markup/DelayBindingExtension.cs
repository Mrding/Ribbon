using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Markup
{

    public class DelayBinding
    {
        private BindingExpressionBase _bindingExpression;
        private DispatcherTimer _timer;
        private DependencyPropertyDescriptor _descriptor;
        private TimeSpan _delay;

        protected DelayBinding(BindingExpressionBase bindingExpression, DependencyObject bindingTarget, DependencyProperty bindingTargetProperty, TimeSpan delay)
        {
            _bindingExpression = bindingExpression;
            _delay = delay;

            // Subscribe to notifications for when the target property changes. This event handler will be 
            // invoked when the user types, clicks, or anything else which changes the target property
            _descriptor = DependencyPropertyDescriptor.FromProperty(bindingTargetProperty, bindingTarget.GetType());


            // Add support so that the Enter key causes an immediate commit


            bindingTarget.SaftyInvoke<FrameworkElement>(el =>
                                                            {
                                                                el.Loaded += OnBindingTargetLoaded;
                                                            });

            // Setup the timer, but it won't be started until changes are detected

            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            _timer.Interval = _delay;
        }

        private void OnBindingTargetLoaded(object sender, RoutedEventArgs e)
        {
            sender.SaftyInvoke<FrameworkElement>(el =>
                                                     {
                                                         _timer.Start();




                                                         el.KeyUp += BindingTargetKeyUp;
                                                         el.Unloaded += OnBindingTargetUnloaded;
                                                         _descriptor.AddValueChanged(el, BindingTargetTargetPropertyChanged);
                                                         el.Loaded -= OnBindingTargetLoaded;
                                                     });
        }

        private void BindingTargetKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _timer.Stop();
            _bindingExpression.UpdateSource();
        }

        private void OnBindingTargetUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            //_timer = null;
            //x_bindingExpression = null;


            sender.SaftyInvoke<FrameworkElement>(el =>
            {
                _descriptor.RemoveValueChanged(el, BindingTargetTargetPropertyChanged);

                el.KeyUp -= BindingTargetKeyUp;
                el.Unloaded -= OnBindingTargetUnloaded;
                el.Loaded += OnBindingTargetLoaded;
            });

        }

        private void BindingTargetTargetPropertyChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            _bindingExpression.UpdateSource();
        }

        public static object SetBinding(DependencyObject bindingTarget, DependencyProperty bindingTargetProperty, TimeSpan delay, Binding binding)
        {
            // Override some specific settings to enable the behavior of delay binding
            //xbinding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            // Apply and evaluate the binding
            var bindingExpression = BindingOperations.SetBinding(bindingTarget, bindingTargetProperty, binding);

            // Setup the delay timer around the binding. This object will live as long as the target element lives, since it subscribes to the changing event, 
            // and will be garbage collected as soon as the element isn't required (e.g., when it's Window closes) and the timer has stopped.
            new DelayBinding(bindingExpression, bindingTarget, bindingTargetProperty, delay);

            // Return the current value of the binding (since it will have been evaluated because of the binding above)
            return bindingTarget.GetValue(bindingTargetProperty);
        }
    }

    [MarkupExtensionReturnType(typeof(object))]
    public class DelayBindingExtension : MarkupExtension
    {
        public DelayBindingExtension()
        {
            Delay = TimeSpan.FromSeconds(0.5);
        }

        public DelayBindingExtension(PropertyPath path)
            : this()
        {
            Path = path;
        }

        public IValueConverter Converter { get; set; }
        public object ConverterParamter { get; set; }
        public string ElementName { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public object Source { get; set; }
        public bool ValidatesOnDataErrors { get; set; }
        public bool ValidatesOnExceptions { get; set; }
        public TimeSpan Delay { get; set; }
        public BindingMode Mode { get; set; }
        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueProvider != null)
            {
                var bindingTarget = valueProvider.TargetObject as DependencyObject;
                var bindingProperty = valueProvider.TargetProperty as DependencyProperty;
                if (bindingProperty == null || bindingTarget == null)
                {
                    throw new NotSupportedException(string.Format(
                        "The property '{0}' on target '{1}' is not valid for a DelayBinding. The DelayBinding target must be a DependencyObject, "
                        + "and the target property must be a DependencyProperty.",
                        valueProvider.TargetProperty,
                        valueProvider.TargetObject));
                }

                var binding = new Binding
                                  {
                                      Path = Path,
                                      Converter = Converter,
                                      ConverterCulture = ConverterCulture,
                                      ConverterParameter = ConverterParamter,
                                      Mode = Mode
                                  };
                if (ElementName != null) binding.ElementName = ElementName;
                if (RelativeSource != null) binding.RelativeSource = RelativeSource;
                if (Source != null) binding.Source = Source;
                binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
                binding.ValidatesOnExceptions = ValidatesOnExceptions;

                return DelayBinding.SetBinding(bindingTarget, bindingProperty, Delay, binding);
            }
            return null;
        }
    }
}