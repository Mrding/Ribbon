namespace Luna.WPF.ApplicationFramework.Behaviors
{
    using System.Windows.Interactivity;
    using System.Windows;

    using Luna.WPF.ApplicationFramework.Threads;

    public class ReadOnlyPropertyAction : TargetedTriggerAction<FrameworkElement>
    {
        public object BindablePropertyItem
        {
            get { return (object)GetValue(BindablePropertyItemProperty); }
            set { SetValue(BindablePropertyItemProperty, value); }
        }

        public static readonly DependencyProperty BindablePropertyItemProperty =
            DependencyProperty.Register("BindablePropertyItem", typeof(object), typeof(ReadOnlyPropertyAction),
            new UIPropertyMetadata(new PropertyChangedCallback((obj,args)=>{
                var element = obj as ReadOnlyPropertyAction;
                if (element.Reverse && args.NewValue != null)
                {
                    element.ReverseProperty();
                }
            })));

        private void ReverseProperty()
        {
            Reverse = false;
            AssociatedObject.GetType().GetProperty(Property).SetValue(AssociatedObject, BindablePropertyItem, null);
            
        }

        protected override void Invoke(object parameter)
        {
            UIThread.BeginInvoke(() => {
                if (Value != null)
                    BindablePropertyItem = Value;
                else
                {
                    if (DpProperty == null)
                        BindablePropertyItem = AssociatedObject.GetType().GetProperty(Property).GetValue(AssociatedObject, null);
                    else
                        BindablePropertyItem = AssociatedObject.GetValue(DpProperty);
                }
            });
      
        }

        public bool Reverse { get; set; }

        public string Value { get; set; }



        public DependencyProperty DpProperty
        {
            get { return (DependencyProperty)GetValue(DpPropertyProperty); }
            set { SetValue(DpPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DpProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DpPropertyProperty =
            DependencyProperty.Register("DpProperty", typeof(DependencyProperty), typeof(ReadOnlyPropertyAction), 
            new UIPropertyMetadata(null));

        

        //public DependencyProperty DpProperty { get; set; }

        public string Property { get; set; }
    }
    public enum ElementType
    {
        TreeView,
        Selector,
        Other
    }
}
