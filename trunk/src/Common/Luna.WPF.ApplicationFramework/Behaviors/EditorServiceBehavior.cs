using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ActiproSoftware.Windows;
using ActiproSoftware.Windows.Controls.Editors;
using System.ComponentModel;
using ActiproSoftware.Windows.Controls.Editors.Parts;
namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public class EditorServiceBehavior
    {

        public static int GetIntStepLimit(DependencyObject obj)
        {
            return (int)obj.GetValue(IntStepLimitProperty);
        }

        public static void SetIntStepLimit(DependencyObject obj, int value)
        {
            obj.SetValue(IntStepLimitProperty, value);
        }

        public static readonly DependencyProperty IntStepLimitProperty =
            DependencyProperty.RegisterAttached("IntStepLimit", typeof(int), typeof(EditorServiceBehavior),
            new UIPropertyMetadata(1, (o, a) =>
            {
                var element = o as FrameworkElement;
                StepBehaviorFactory.CreateBehavior(element, (int)a.NewValue);
            }));

        public static int GetDateTimeEditBoxStep(DependencyObject obj)
        {
            return (int)obj.GetValue(DateTimeEditBoxStepProperty);
        }

        public static void SetDateTimeEditBoxStep(DependencyObject obj, int value)
        {
            obj.SetValue(DateTimeEditBoxStepProperty, value);
        }

        public static readonly DependencyProperty DateTimeEditBoxStepProperty =
            DependencyProperty.RegisterAttached("DateTimeEditBoxStep", typeof(int), typeof(EditorServiceBehavior),
            new UIPropertyMetadata(1, (o, a) =>
            {
                var element = o as DateTimeEditBox;
                new DateTimeEditBoxStepBehavior(element, (int)a.NewValue);
            }));

        internal class IntStepLimitBehavior
        {
            private int _step;
            FrameworkElement _element;
            public IntStepLimitBehavior(FrameworkElement element,int step)
            {
                _step = step;
                _element = element;
                (_element as Int32EditBox).ValueChanged += new EventHandler<PropertyChangedRoutedEventArgs<int?>>(IntStepLimitBehavior_ValueChanged);
            }

            void IntStepLimitBehavior_ValueChanged(object sender, PropertyChangedRoutedEventArgs<int?> e)
            {
                if (e.NewValue.HasValue)
                {
                    var mod = -e.NewValue.Value % 5;
                    (sender as Int32EditBox).Value = e.NewValue.Value + mod;
                }
            }

            private void editor_TextChanging(object sender, ActiproSoftware.Windows.StringPropertyChangingRoutedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.NewValue))
                {
                    int val = Convert.ToInt32(e.NewValue);
                    if ((val % _step) != 0)
                    {
                        e.Cancel = true;
                        e.Handled = true;
                    }
                    else
                    {
                        var box = _element as Int32EditBox;
                        if (box != null)
                            box.Value = val;
                    }
                }
            }
        }

        internal class DateTimeEditBoxStepBehavior
        {
            private DateTimeEditBox box;
            private int step;
            public DateTimeEditBoxStepBehavior(DateTimeEditBox box, int step)
            {
                this.box = box;
                this.step = step;
                box.ValueChanging += new EventHandler<ActiproSoftware.Windows.PropertyChangingRoutedEventArgs<DateTime?>>(box_ValueChanging);
                box.ValueChanged += new EventHandler<ActiproSoftware.Windows.PropertyChangedRoutedEventArgs<DateTime?>>(box_ValueChanged);
            }

            void box_ValueChanged(object sender, ActiproSoftware.Windows.PropertyChangedRoutedEventArgs<DateTime?> e)
            {
                if (box.Value.HasValue)
                {
                    var val = (int)box.Value.Value.Minute % step;
                    if (val != 0)
                    {
                        if (box.Value > previousTime)
                            box.Value = box.Value.Value.AddMinutes(step - val);
                        else
                            box.Value = box.Value.Value.AddMinutes(-val);
                    }
                }
            }

            private DateTime previousTime;

            void box_ValueChanging(object sender, ActiproSoftware.Windows.PropertyChangingRoutedEventArgs<DateTime?> e)
            {
                if (box.Value.HasValue)
                    previousTime = box.Value.Value;
            }
        }

        internal class TimeSpanStepBehavior
        {
            private TimeSpanEditBox box;
            private int step;
            public TimeSpanStepBehavior(TimeSpanEditBox box, int step)
            {
                this.box = box;
                this.step = step;
                box.ValueChanging += new EventHandler<ActiproSoftware.Windows.PropertyChangingRoutedEventArgs<TimeSpan?>>(box_ValueChanging);
                box.ValueChanged += new EventHandler<ActiproSoftware.Windows.PropertyChangedRoutedEventArgs<TimeSpan?>>(box_ValueChanged);
            }

            void box_ValueChanged(object sender, ActiproSoftware.Windows.PropertyChangedRoutedEventArgs<TimeSpan?> e)
            {
                if (box.Value.HasValue)
                {
                    var val = (int)box.Value.Value.Minutes % step;
                    if (val != 0)
                    {
                        if (box.Value > previousTime)
                            box.Value = box.Value.Value.Add(TimeSpan.FromMinutes(step - val));
                        else
                            box.Value = box.Value.Value.Add(TimeSpan.FromMinutes(-val));
                    }
                }
            }

            private TimeSpan previousTime;

            void box_ValueChanging(object sender, ActiproSoftware.Windows.PropertyChangingRoutedEventArgs<TimeSpan?> e)
            {
                if (box.Value.HasValue)
                    previousTime = box.Value.Value;
            }
        }

        internal class DateTimePartGroupBehavior
        {
            private DateTimePartGroup _dateTimePartGroup;
            private int step;
            private DependencyPropertyDescriptor dpDesc;
            public DateTimePartGroupBehavior(DateTimePartGroup dateTimePartGroup, int step)
            {
                _dateTimePartGroup = dateTimePartGroup;
                this.step = step;
                dpDesc = DependencyPropertyDescriptor.FromProperty(DateTimePartGroup.ValueProperty, typeof(DateTimePartGroup));

                _dateTimePartGroup.Unloaded += _dateTimePartGroup_Unloaded;
                _dateTimePartGroup.Loaded += _dateTimePartGroup_Loaded;
            }

            private DateTime previousTime;

            void _dateTimePartGroup_Loaded(object sender, RoutedEventArgs e)
            {
                if (_dateTimePartGroup.Value.HasValue)
                {
                    previousTime = _dateTimePartGroup.Value.Value;
                }
                dpDesc.AddValueChanged(_dateTimePartGroup, OnValueChanged);
            }

            void _dateTimePartGroup_Unloaded(object sender, RoutedEventArgs e)
            {
                dpDesc.RemoveValueChanged(_dateTimePartGroup, OnValueChanged);
            }

            private void OnValueChanged(object sender, EventArgs args)
            {
                if (_dateTimePartGroup.Value.HasValue)
                {
                    var val = (int)_dateTimePartGroup.Value.Value.Minute % step;
                    if (val != 0)
                    {
                        if (_dateTimePartGroup.Value > previousTime)
                            _dateTimePartGroup.Value = _dateTimePartGroup.Value.Value.AddMinutes(step - val);
                        else
                            _dateTimePartGroup.Value = _dateTimePartGroup.Value.Value.AddMinutes(-val);
                    }
                }
                previousTime = _dateTimePartGroup.Value.Value;
            }
        }

        public class StepBehaviorFactory
        {
            public static void CreateBehavior(FrameworkElement editBox, int step)
            {
                if (editBox is DateTimeEditBox)
                {
                    new DateTimeEditBoxStepBehavior(editBox as DateTimeEditBox, step);
                }
                if (editBox is TimeSpanEditBox)
                {
                    new TimeSpanStepBehavior(editBox as TimeSpanEditBox, step);
                }
                if (editBox is Int32EditBox)
                {
                    new IntStepLimitBehavior(editBox, step);
                }
                if (editBox is DateTimePartGroup)
                {
                    new DateTimePartGroupBehavior(editBox as DateTimePartGroup, step);
                }
            }
        }
    }
}
