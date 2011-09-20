using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Luna.Core.Extensions;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class PointOutLayer : BlockGridLayerBase
    {
        private DispatcherTimer _timer;
        private object _pointOutBlockOld;

        public override void Initialize()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.8) };
            _timer.Tick += SetToolTip;

            base.Initialize();
        }

        protected override void AddToPanel(IAxisPanel axisPanel)
        {
            axisPanel.Add(this);
            axisPanel.ScrollOwner.ScrollChanged += delegate { CloseToolTip(); };
        }

        private void SetToolTip(object sender, EventArgs e)
        {
            _timer.Stop();

            if (PointOutBlock == null)
                return;

            var point = Mouse.GetPosition(this);

            if (!AxisYConverter.IsInViewRagne(point.Y) || !AxisXConverter.IsInViewRagne(point.X))
                return;

            ToolTip.SaftyInvoke<ToolTip>(t =>
                                                {
                                                    t.IsOpen = true;
                                                    t.Placement = PlacementMode.Mouse;
                                                    t.Content = PointOutBlock;
                                                });
        }

        protected override void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseToolTip();
        }

        protected override void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            CloseToolTip();
        }

        protected override void OnParentMouseMove(object sender, MouseEventArgs e)
        {
            CloseToolTip();

            if (!ReferenceEquals(_pointOutBlockOld, PointOutBlock))
                _pointOutBlockOld = PointOutBlock;

            _timer.Start();
        }

        private void CloseToolTip()
        {
            ToolTip.SaftyInvoke<ToolTip>(t =>
            {
                t.IsOpen = false;
                if (_timer != null)
                    _timer.Stop();
            });
        }

        protected override void OnDispose()
        {
            CloseToolTip();

            ToolTip = null;

            _timer.Tick -= SetToolTip;
            _timer = null;
            base.OnDispose();
        }
    }
}