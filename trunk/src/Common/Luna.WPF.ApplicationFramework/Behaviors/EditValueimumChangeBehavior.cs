namespace Luna.WPF.ApplicationFramework.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;

    using ActiproSoftware.Windows.Controls.Editors;

    using System.Windows.Threading;

    public class EditValueimumChangeBehavior : Behavior<Int32EditBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(AssociatedObject_DataContextChanged);
            base.OnAttached();
        }

        public int? Minimum
        {
            get { return (int?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int?), typeof(EditValueimumChangeBehavior),
            new UIPropertyMetadata(null));


        public int? Maximum
        {
            get { return (int?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int?), typeof(EditValueimumChangeBehavior),
            new UIPropertyMetadata(null));

        void AssociatedObject_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            Minimum = AssociatedObject.Minimum;
            Maximum = AssociatedObject.Maximum;
            AssociatedObject.Minimum = null;
            AssociatedObject.Maximum = null;
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ChangeValue();
            }), DispatcherPriority.Normal);
        }

        private void ChangeValue()
        {
            if (AssociatedObject.Value < Minimum)
            {
                AssociatedObject.Value = Minimum;
            }
            else if (AssociatedObject.Value > Maximum)
            {
                AssociatedObject.Value = Maximum;
            }
            AssociatedObject.Minimum = Minimum;
            AssociatedObject.Maximum = Maximum;
        }
    }
}
