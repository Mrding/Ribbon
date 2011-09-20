using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Luna.WPF.ApplicationFramework.Validations
{
    public interface IValidateUI
    {
        FrameworkElement Element { get; set; }

        void FailShow();
        
        void SuccessShow();
    }

    public class DefaultValidateUI : IValidateUI
    {
        public FrameworkElement Element { get; set; }

        public void FailShow()
        {
            ShowToolTip();
        }

        public void SuccessShow()
        {
            // throw new NotImplementedException();
        }

        private DispatcherTimer timer;
        private ToolTip tooltip;

        private void ShowToolTip()
        {
            if (tooltip == null)
            {
                tooltip = new ToolTip();
                tooltip.StaysOpen = true;
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1.5);
                tooltip.PlacementTarget = Element;
                tooltip.Placement = PlacementMode.Right;
                timer.Tick += delegate
                {
                    tooltip.IsOpen = false;
                    timer.Stop();
                };
            }
            var list = ValidationService.GetErrors(Element);
            if (list.Count == 0) return;
            tooltip.Content = list.OrderBy(e => e.Priority).First().ErrorContent;
            if (tooltip.Content != null)
            {
                tooltip.IsOpen = true;
                timer.Start();
            }
        }
    }
}
