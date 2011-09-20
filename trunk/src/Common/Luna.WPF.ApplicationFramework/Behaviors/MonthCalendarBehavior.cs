using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework;
using System.Collections;
using System.Windows;
using ActiproSoftware.Windows.Controls.Editors;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework.Behaviors
{

    public class BackstageService
    {


        public static int GetSelectedIndex(ActiproSoftware.Windows.Controls.Ribbon.Controls.Backstage obj)
        {
            return (int)obj.GetValue(SelectedIndexProperty);
        }

        public static void SetSelectedIndex(ActiproSoftware.Windows.Controls.Ribbon.Controls.Backstage obj, int value)
        {
            obj.SetValue(SelectedIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.RegisterAttached("SelectedIndex", typeof(int), typeof(BackstageService), 
            new FrameworkPropertyMetadata(0, (d,e)=>
                                                 {
                                                    d.SaftyInvoke<ActiproSoftware.Windows.Controls.Ribbon.Controls.Backstage>(backstage=>
                                                    {
                                                        backstage.SelectedIndex = System.Convert.ToInt32(e.NewValue);                                                                                  
                                                    });      
                                                 }));

        
    }

    public class MonthCalendarBehavior
    {
        public static IEnumerable GetCalendarSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(CalendarSourceProperty);
        }

        public static void SetCalendarSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(CalendarSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for CalendarSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CalendarSourceProperty =
            DependencyProperty.RegisterAttached("CalendarSource", typeof(IEnumerable), typeof(MonthCalendarBehavior), new PropertyMetadata(null, null, CalendarCoerceValueCallback));

        private static object CalendarCoerceValueCallback(DependencyObject d, object basevalue)
        {
            var dataList = basevalue as IEnumerable;

            if (PresentationFrameworkModule.IsInDesignMode || dataList == null) return basevalue;

            var monthCalendar = d as MonthCalendar;
            if (monthCalendar == null) return basevalue;
            
            UIThread.BeginInvoke(monthCalendar.Refresh);

            return basevalue;
        }

        public static DateTime GetActiveDate(MonthCalendar obj)
        {
            return (DateTime)obj.GetValue(ActiveDateProperty);
        }

        public static void SetActiveDate(MonthCalendar obj, DateTime value)
        {
            obj.SetValue(ActiveDateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ActiveDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveDateProperty =
            DependencyProperty.RegisterAttached("ActiveDate", typeof(DateTime), typeof(MonthCalendarBehavior),
            new FrameworkPropertyMetadata(DateTime.Today, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) => 
                    d.SaftyInvoke<MonthCalendar>(mc =>
                                                    {
                                                        if (!mc.IsLoaded)
                                                        {
                                                            var dpd = DependencyPropertyDescriptor.FromProperty(MonthCalendar.ActiveDateProperty, typeof(MonthCalendar));

                                                            mc.Loaded += delegate
                                                                             {
                                                                                 if (dpd != null)
                                                                                     dpd.AddValueChanged(mc, SetActiveDate);
                                                                             };
                                                            mc.Unloaded+=delegate
                                                                             {
                                                                                 if (dpd != null)
                                                                                     dpd.RemoveValueChanged(mc, SetActiveDate);
                                                                             };
                                                        }

                                                        var dateValue = Convert.ToDateTime(e.NewValue);
                                                        //if (mc.ActiveDate != dateValue)
                                                        mc.ActiveDate = dateValue;
                                                    })));

        private static void SetActiveDate(object sender, EventArgs e)
        {
            sender.SaftyInvoke<MonthCalendar>(mc=> SetActiveDate(mc,mc.ActiveDate));
            
        }
    }
}
