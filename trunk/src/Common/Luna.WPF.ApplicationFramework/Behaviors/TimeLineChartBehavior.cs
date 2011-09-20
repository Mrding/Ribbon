using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    //抄袭RadioButton的GroupName理念
    public static class TimeLineChartBehavior
    {
        #region MaxValue

        public static double GetMaxValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxValueProperty);
        }

        public static void SetMaxValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached("MaxValue", typeof(double),
            typeof(TimeLineChartBehavior), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, ((d, e) =>
                                              {
                                                  //var maualValue = GetManualMaxValue(d);
                                                  //SetManualMaxValue(d, maualValue >> 1);

                                                  return e;
                                              })));

        #endregion

        #region Data

        public static IList<double> GetData(DependencyObject obj)
        {
            return (IList<double>)obj.GetValue(DataProperty);
        }

        public static void SetData(DependencyObject obj, IList<double> value)
        {
            obj.SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.RegisterAttached("Data", typeof(IList<double>),typeof(TimeLineChartBehavior), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
                {
                    //var lineChart = d;
                    //var value = e.NewValue as IList<double>;
                    //if (value == null)
                    //    return;

                    //var maualValue = GetManualMaxValue(d);

                    //if (maualValue == 1)
                    //{
                    //    SetManualMaxValue(d, 2);
                    //    SetMaxValue(lineChart, value.Max());
                    //}

                   

                }));

        #endregion
    }
}
