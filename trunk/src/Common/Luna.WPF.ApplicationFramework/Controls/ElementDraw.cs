using System.Windows;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract class ElementDraw : DependencyObject
    { 
        public abstract void Attach(FrameworkElement element); 

        public virtual bool CanDraw()
        {
            return true;
        }

        protected virtual void InternalDraw(DrawingContext dc) { }

        public void Draw(DrawingContext dc)
        {
            if (CanDraw())
                InternalDraw(dc);
        }
    }

    public class ElementDraw<T> : ElementDraw where T : FrameworkElement
    {
        private T _element;
        public override void Attach(FrameworkElement element)
        {
            _element = element as T;
        }

        public T Element
        {
            get { return _element; }
        }
    }
}
