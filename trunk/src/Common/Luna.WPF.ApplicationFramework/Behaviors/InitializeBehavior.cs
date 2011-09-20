using System.Windows;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public class InitializeBehavior : BehaviorBase<FrameworkElement>
    {
        protected override void Initialize()
        {
            AssociatedObject.Loaded += ObjectLoaded;
        }

        void ObjectLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SaftyInvoke<IInitialize>(o => o.Initialize());
            AssociatedObject.Loaded -= ObjectLoaded;
        }
    }

}
