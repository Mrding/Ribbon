using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class MaskAttach : Freezable
    {
        private TemplatedAdorner _maskAdorner;
        private UIElement _target;

        public bool Open
        {
            get { return (bool)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(bool), typeof(MaskAttach),
                                        new UIPropertyMetadata((d, e) => d.Dispatcher.BeginInvoke((Action)delegate
                                        {
                                            var processAttach = (MaskAttach)d;

                                            if (e.NewValue.Equals(true))
                                            {
                                                //得到装饰层容器，为层赋值
                                                var adornerLayer = AdornerLayer.GetAdornerLayer(processAttach._target);
                                                processAttach._maskAdorner = new TemplatedAdorner(processAttach._target, adornerLayer);
                                            }
                                            else
                                            {
                                                //关闭层，移除层装饰
                                                var adorner = processAttach._maskAdorner;
                                                if (adorner != null)
                                                    adorner.Detach();
                                            }

                                        }, System.Windows.Threading.DispatcherPriority.Render)));


        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template",
                                        typeof(DataTemplate),
                                        typeof(MaskAttach),
                                        new UIPropertyMetadata());

        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object),
                                        typeof(MaskAttach),
                                        new UIPropertyMetadata());

        public static MaskAttach GetContent(DependencyObject obj)
        {
            return (MaskAttach)obj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject obj, MaskAttach value)
        {
            obj.SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached("Content", typeof(MaskAttach),
                typeof(MaskAttach), new UIPropertyMetadata((d, e) =>
                {
                    if (e.NewValue == null)
                        return;

                    var maskAttach = (MaskAttach)e.NewValue;
                    maskAttach._target = d as UIElement;

                    //第一次赋值
                    d.SetValue(MaskAttach.TemplateProperty, maskAttach.Template);
                    d.SetValue(MaskAttach.DataContextProperty, maskAttach.DataContext);

                    //当属性改变的时候侦测为_target属性赋值，因为_target非FrameworkElement派生元素无法使用Binding
                    maskAttach.RegisterEvent();
                }));
        private DependencyPropertyDescriptor dpdDataContext;
        private DependencyPropertyDescriptor dpdTemplate;

        private void RegisterEvent()
        {
            dpdDataContext = DependencyPropertyDescriptor.FromProperty(MaskAttach.DataContextProperty, this.GetType());
            dpdDataContext.AddValueChanged(this, DataContextChanged);
            dpdTemplate = DependencyPropertyDescriptor.FromProperty(MaskAttach.TemplateProperty, this.GetType());
            dpdTemplate.AddValueChanged(this, TemplateChanged);
            (_target as FrameworkElement).Loaded -= new RoutedEventHandler(MaskAttach_Loaded);
            (_target as FrameworkElement).Unloaded += new RoutedEventHandler(MaskAttach_Unloaded);
        }

        private void DataContextChanged(object sender, EventArgs e)
        {
            _target.SetValue(MaskAttach.DataContextProperty, this.DataContext);
        }

        private void TemplateChanged(object sender, EventArgs e)
        {
            _target.SetValue(MaskAttach.TemplateProperty, this.Template);
        }

        void MaskAttach_Unloaded(object sender, RoutedEventArgs e)
        {
            dpdDataContext.RemoveValueChanged(this, DataContextChanged);
            dpdTemplate.RemoveValueChanged(this, TemplateChanged);
            (_target as FrameworkElement).Unloaded -= new RoutedEventHandler(MaskAttach_Unloaded);
            (_target as FrameworkElement).Loaded += new RoutedEventHandler(MaskAttach_Loaded);
        }

        void MaskAttach_Loaded(object sender, RoutedEventArgs e)
        {
            (_target as FrameworkElement).Loaded -= MaskAttach_Loaded;
            RegisterEvent();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MaskAttach();
        }
    }

    public class TemplatedAdorner : Adorner
    {
        private readonly ContentPresenter _contentPresenter;
        private readonly AdornerLayer _adornerLayer;

        public TemplatedAdorner(UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            _adornerLayer = adornerLayer;
            _contentPresenter = new ContentPresenter();

            //绑定内容
            var contentBinding = new Binding
            {
                Mode = BindingMode.TwoWay,
                Source = adornedElement,
                Path = new PropertyPath(MaskAttach.DataContextProperty)
            };
            _contentPresenter.SetBinding(ContentPresenter.ContentProperty, contentBinding);

            //绑定模板
            var contentTemplateBinding = new Binding
            {
                Mode = BindingMode.TwoWay,
                Source = adornedElement,
                Path = new PropertyPath(MaskAttach.TemplateProperty)
            };
            _contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, contentTemplateBinding);

            //当控件发生大小变化的时候需要强制刷新，不过加这句效率会有影响
            //AdornedElement.LayoutUpdated += AdornedElementLayoutUpdated;
            //加入层中
            _adornerLayer.Add(this);
            //加入可视树是为了防止点击穿过
            AddVisualChild(_contentPresenter);

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
            _contentPresenter.Arrange(new Rect(finalSize));
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
            //AdornedElement.LayoutUpdated -= AdornedElementLayoutUpdated;
            _adornerLayer.Remove(this);
        }

    }
}