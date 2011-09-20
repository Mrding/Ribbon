using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class PostionAdorner : Adorner
    {
        private readonly ContentPresenter _contentPresenter;
        private readonly AdornerLayer _adornerLayer;

        public PostionAdorner(UIElement adornedElement, AdornerLayer adornerLayer, DataTemplate dataTemplate)
            : base(adornedElement)
        {
            _adornerLayer = adornerLayer;
            _contentPresenter = new ContentPresenter();
            _contentPresenter.ContentTemplate = dataTemplate;

            //加入层中
            _adornerLayer.Add(this);
        }

        public double Xoffset
        {
            get;
            set;
        }

        public double Yoffset
        {
            get;
            set;
        }

        public object Content
        {
            get
            {
                return _contentPresenter.Content;
            }
            set
            {
                _contentPresenter.Content = value;
            }
        }


        private Point _positon;
        public Point Positon
        {
            get { return _positon; }
            set
            {
                _positon = value;
                _adornerLayer.Update(this.AdornedElement);
            }
        }

        private void AdornedElementLayoutUpdated(object sender, EventArgs e)
        {
            if (_adornerLayer != null)
            {
                _adornerLayer.Update(this.AdornedElement);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Point point = Positon;
            point.X += Xoffset;
            point.Y += Yoffset;

            if (point.X + _contentPresenter.DesiredSize.Width > finalSize.Width)
                point.X = finalSize.Width - _contentPresenter.DesiredSize.Width;
            else if (point.X < 0)
                point.X = 0;

            _contentPresenter.Arrange(new Rect(point, _contentPresenter.DesiredSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public void Detach()
        {
            _adornerLayer.Remove(this);
        }
    }
}
