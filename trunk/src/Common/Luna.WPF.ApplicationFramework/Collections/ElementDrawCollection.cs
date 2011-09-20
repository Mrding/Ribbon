using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Controls;

namespace Luna.WPF.ApplicationFramework.Collections
{
    public sealed class ElementDrawCollection : FreezableCollection<ElementDraw>
    {
        private readonly FrameworkElement _element;
        public ElementDrawCollection(){}

        public ElementDrawCollection(FrameworkElement element)
        {
            _element = element;
            ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ElementDraw local in e.NewItems)
                        local.Attach(_element);
                    break;
            }
        }

        public void Draw(DrawingContext dc)
        {
            foreach (var elementDraw in this)
                elementDraw.Draw(dc);
        }
    }
}
