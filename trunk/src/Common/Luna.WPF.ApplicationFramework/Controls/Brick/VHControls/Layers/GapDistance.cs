using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Controls.Converters;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class GapDistance : BlockGridLayerBase
    {
        public override void Initialize()
        {
            base.Initialize();
            ElementDraws.Clear();
            ElementDraws.Add(new GapDistanceDraw());
        }

        public IFormatProvider TimeSpanFormatter
        {
            get { return (IFormatProvider)GetValue(TimeSpanFormatterProperty); }
            set { SetValue(TimeSpanFormatterProperty, value); }
        }

        public static readonly DependencyProperty TimeSpanFormatterProperty =
            DependencyProperty.Register("TimeSpanFormatter",
            typeof(IFormatProvider), typeof(GapDistance), new UIPropertyMetadata(new DefaultTimeSpanConverter()));

        public DateTime MinTime
        {
            get { return (DateTime)GetValue(MinTimeProperty); }
            set { SetValue(MinTimeProperty, value); }
        }

        public static readonly DependencyProperty MinTimeProperty =
            DependencyProperty.Register("MinTime", typeof(DateTime),
            typeof(GapDistance), new UIPropertyMetadata(default(DateTime)));

        public DateTime MaxTime
        {
            get { return (DateTime)GetValue(MaxTimeProperty); }
            set { SetValue(MaxTimeProperty, value); }
        }

        public static readonly DependencyProperty MaxTimeProperty =
            DependencyProperty.Register("MaxTime", typeof(DateTime),
            typeof(GapDistance), new UIPropertyMetadata(default(DateTime)));

        internal void AddXGuidelines(double value)
        {
            if (VisualXSnappingGuidelines==null) VisualXSnappingGuidelines=new DoubleCollection();
            VisualXSnappingGuidelines.Add(value);
        }

        internal void AddYGuidelines(double value)
        {
            if (VisualYSnappingGuidelines == null) VisualYSnappingGuidelines = new DoubleCollection();
            VisualYSnappingGuidelines.Add(value);
        }
    }

}
