using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ActiproSoftware.Windows.Controls.Docking;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    using Luna.WPF.ApplicationFramework.Behaviors;

    //[ContentProperint("Content"), Localizabiliint(LocalizationCategory.None, Readabiliint = Readabiliint.Unreadable), DefaultProperint("Content")]
    public abstract partial class AxisPanel : ScrollViewer, ILogicalParent, IInitialize, IAxisPanel, IHorizontalControl, IVerticalControl
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly LimitRange<DateTime> _dataRangeX;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly LimitRange<int> _dataRangeY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly LimitRange<double> _viewportRangeX;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private LimitRange<double> _viewportRangeY;

        private UIElement _horizontalMainElement;
        private UIElement _verticalMainElement;

        protected AxisPanel()
        {
            //AddBehavior
            Interaction.GetBehaviors(this).Add(new InitializeBehavior());

            ScrollInfo = this;
            ScrollOwner = this;
            //定数量16是因为太少的话反而会有效率影响，一般控件会在16个以内
            //VerticalControls = new List<AxisControl>(16);
            HorizontalControls = new List<AxisControl>(16);
            _children = new List<AxisControl>(16);

            _viewportRangeX = new LimitRange<double>();
            _dataRangeX = new LimitRange<DateTime>();
            _viewportRangeY = new LimitRange<double>();
            _dataRangeY = new LimitRange<int>();
        }

        public void Initialize()
        {
            this.FindAncestor<DockSite>().SaftyInvoke(o => { o.Workspace.DockSite.WindowClosed += DockSiteWindowClosing; });
        }

        private void DockSiteWindowClosing(object sender, DockingWindowEventArgs e)
        {
            var dockingWindow = this.FindAncestorByLogicalTree<DockingWindow>();
            if (dockingWindow != e.Window)
                return;

            //VerticalControls.ForEach<IDisposable>(o => o.Dispose());
            _children.ForEach<IDisposable>(o => o.Dispose());
            HorizontalControls.ForEach<IDisposable>(o => o.Dispose());

            //VerticalControls.Clear();
            _children.Clear();
            HorizontalControls.Clear();

            e.OriginalSource.SaftyInvoke<DockSite>(o => o.WindowClosed -= DockSiteWindowClosing);

            DataContext = null;
        }

        

        public UIElement HorizontalMainElement
        {
            get { return _horizontalMainElement ?? this; }
            protected set
            {
                _horizontalMainElement = value;
            }
        }

        public UIElement VerticalMainElement
        {
            get { return _verticalMainElement ?? this; }
        }

        public double HorizontalOffSetValue
        {
            get { return ScrollViewerUtilities.GetHorizontalOffset(this); }
            set { ScrollViewerUtilities.SetHorizontalOffset(this, value); }
        }

        public abstract double VerticalOffSetValue { get; }

        internal LimitRange<DateTime> DataRangeX
        {
            get { return _dataRangeX; }
        }

        internal LimitRange<int> DataRangeY
        {
            get { return _dataRangeY; }
        }

        internal LimitRange<double> ViewportRangeX
        {
            get { return _viewportRangeX; }
        }

        internal virtual LimitRange<double> ViewportRangeY { get { return _viewportRangeY; } }

        private List<AxisControl> _children;

        protected List<AxisControl> HorizontalControls { get; private set; }

        //protected List<AxisControl> VerticalControls { get; private set; }

        public void AddHorizontalControl(AxisControl element)
        {
            if (element != null) // && !HorizontalControls.Contains(element)
                HorizontalControls.Add(element);
        }

        //public void AddVerticalControl(AxisControl element)
        //{
        //    if (element != null) //&& !VerticalControls.Contains(element)
        //        VerticalControls.Add(element);
        //}

        public void Add(AxisControl element)
        {
            _children.Add(element);
        }

        void ILogicalParent.AddLogicalChild(object child)
        {
            base.AddLogicalChild(child);
        }

        void ILogicalParent.RemoveLogicalChild(object child)
        {
            base.RemoveLogicalChild(child);
        }

        void ILogicalParent.ReplaceLogicalChild(object oldchild, object newChild)
        {
            base.RemoveLogicalChild(oldchild);
            AddLogicalChild(newChild);
        }

        public void RefreshX()
        {
            //Debug.Print(string.Format("{0}#{1} => RefreshHorizontalControls", Name, GetHashCode()));
            HorizontalControls.ForEach(o =>
            {
                if (o.IsArrangeValid && o.IsVisible)
                {
                    o.InvalidateVisual();
                }
            });
        }

        public void Refresh()
        {
            _children.ForEach(o =>
                     {
                         if (o.IsArrangeValid && o.IsVisible)
                             o.InvalidateVisual();
                     });
        }

        public void SetVerticalMain(UIElement element)
        {
            if (_verticalMainElement != element && _verticalMainElement != null)
                throw new ArgumentException("already have VerticalMainElementMain Element");
            _verticalMainElement = element;
        }

        public void RemoveVerticalMain(UIElement element)
        {
            if (_verticalMainElement == element)
                _verticalMainElement = null;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        //private Size? _givedSize;
        //protected override Size MeasureOverride(Size constraint)
        //{

        //    if (constraint.Height != 0 && constraint.Width != 0 && !(double.MaxValue == constraint.Width || double.MaxValue == constraint.Height) && _givedSize == null)
        //    {
        //        _givedSize = constraint;
        //    }

        //    return base.MeasureOverride(constraint);
        //}

        //protected override Size ArrangeOverride(Size arrangeBounds)
        //{
        //    return base.ArrangeOverride(_givedSize.Value);
        //}
         
        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (_viewportRangeY == null)
        //        _viewportRangeY = new LimitRange<double> { Max = constraint.Height };

        //    return base.MeasureOverride(constraint);
        //}
    }
}