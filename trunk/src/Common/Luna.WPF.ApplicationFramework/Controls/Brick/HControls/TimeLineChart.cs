using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Luna.Common.Constants;
using Luna.WPF.ApplicationFramework.Extensions;
using System.Windows.Controls;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class TimeLineChart : LineChartBase<DateTime>
    {
        private DateTime _startTime;
        private DateTime _enquiryStartTime;

        public override void Initialize()
        {
            base.Initialize();
            _startTime = AxisXConverter.ScreenToData(Math.Min(0, -AxisPanel.HorizontalOffset));//AxisXConverter.ViewportToData(0);
            _enquiryStartTime = _startTime.AddDays(Global.HeadDayAmount);
        }

        public double CorrectionValue
        {
            get { return (double)GetValue(CorrectionValueProperty); }
            set { SetValue(CorrectionValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CorrectionValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CorrectionValueProperty =
            DependencyProperty.Register("CorrectionValue", typeof(double), typeof(TimeLineChart), new UIPropertyMetadata(0.0));

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(TimeLineChart), new UIPropertyMetadata(default(DateTime)));

        /// <summary>
        /// PerHour Draw Point
        /// </summary>
        public double PerHourCount
        {
            get { return (double)GetValue(PerHourCountProperty); }
            set { SetValue(PerHourCountProperty, value); }
        }

        public static readonly DependencyProperty PerHourCountProperty =
            DependencyProperty.Register("PerHourCount", typeof(double), typeof(TimeLineChart),
            new UIPropertyMetadata(4.0));

        protected override double GetCellInterval()
        {
            var dateTime = AxisXConverter.ScreenToData(0);
            var hourWidth = AxisXConverter.DataToScreen(dateTime.AddHours(1));
            //每个小时的长度除以PerHourCount;
            return hourWidth / PerHourCount;
        }

        protected override double GetXTransform(double cellWidth)
        {
            //得到相对于大容器的位置
            var viewPortWidth = AxisPanel.HorizontalOffset;//AxisXConverter.ScreenToViewport(0);
            //用段的模求出偏移量
            return -(viewPortWidth % cellWidth);

            //return xTranslateTransform;
        }

        protected override int GetStartIndex(double xTransform)
        {
            //得到相对于大容器的位置
            //var startTime = AxisXConverter.ScreenToData(-AxisPanel.HorizontalOffset);//AxisXConverter.ViewportToScreen(0)
            //var screenTime = AxisXConverter.ScreenToData(xTransform);
            // index = screenTime.Subtract(startTime).TotalHours * PerHourCount;
            var screenStartTime = AxisXConverter.ScreenToData(0);

            return (int)(screenStartTime.Subtract(_enquiryStartTime).TotalMinutes / 15);

            //return (int)index;)
            return 0;
        }

        protected override int GetCurrentIndex(Point point)
        {
            // var viewPortWidth = AxisPanel.HorizontalOffset;//AxisXConverter.ScreenToViewport(0);
            //var viewMin = AxisXConverter.ViewportToData(0); ;//AxisXConverter.ViewportToData(0);
            var datetime = AxisXConverter.ToData(point.X + AxisPanel.HorizontalOffset);
            var remains = datetime.Minute%15;
             if (remains < 3 )
             {
               datetime =  datetime.AddMinutes(-remains);
             }
            else if(12 < remains)
            {
                datetime = datetime.AddMinutes(15 - remains);
            }
            else if(remains == 0)
            {
            }
            else
            {
                return -1;
            }

            datetime = datetime.AddSeconds(-datetime.Second);
            // System.Diagnostics.Debug.Print(datetime.ToString());
             return (int)(datetime.Subtract(_enquiryStartTime).TotalHours * PerHourCount);
           
        }

        protected override double CalibrateValue(double value)
        {
            if (CorrectionValue == 0) return value;

            return -(value - CorrectionValue);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }

        protected override void WhenMouseOverValuePoint(int valueIndex)
        {
            //15为每15分钟的坐标点
            CurrentTime = _enquiryStartTime.AddMinutes(15 * valueIndex);
        }

        protected override void OnDispose()
        {
            
        }
    }
}
