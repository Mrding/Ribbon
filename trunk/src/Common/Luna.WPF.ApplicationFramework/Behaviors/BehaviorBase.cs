using System.Windows;
using System.Windows.Interactivity;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement
    {
        private bool _isClean = true;

        protected sealed override void OnAttached()
        {
            base.OnAttached();

            //AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            _isClean = false;

            Initialize();
        }

        protected override void OnDetaching()
        {
            CleanUp();
            base.OnDetaching();
        }

        protected virtual void Initialize()
        {
          
        }

        protected virtual void Uninitialize()
        {
          
        }

        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (_isClean) return;

            _isClean = true;

            if (AssociatedObject != null)
                AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;

            Uninitialize();
        }
    }
}