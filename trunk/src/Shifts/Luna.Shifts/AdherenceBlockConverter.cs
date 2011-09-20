using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using Luna.Shifts.Domain;
using Luna.Common;
using Luna.WPF.ApplicationFramework;
using Luna.Core.Extensions;

namespace Luna.Shifts
{
    public class AdherenceBlockConverter : IDefalutBlockConverter
    {
        //public void SetNewTime(IList<IEnumerable> itemsSource, IEnumerable<BlockNewPositionInfo> list)
        //{
        //    var valueChanged = false;
        //    foreach (var item in list)
        //    {
        //        var start = item.NewStart;
        //        var end = item.NewEnd;
        //        var term = item.Target as IVisibleTerm;

        //        if (term == null || (term.Start == start && term.End == end))
        //            continue;

        //        item.Target.SaftyInvoke<AdherenceEvent>(x =>
        //                                  {
        //                                      x.Start = start;
        //                                      x.End = end;
        //                                      valueChanged = true;

        //                                      BlockChanged(x, valueChanged);
        //                                  });
        //    }
        //    if(valueChanged)
        //        Refresh();
            
        //}

        public string GetContentText(object obj)
        {
            return null;
        }

        public void Refresh()
        {
        }

        public double FontSize
        {
            get { return 12; }
        }

        public DateTime GetEnd(object obj)
        {
            var term = (ITerm)obj;
            return term.End;
        }

        public int GetLevel(object obj)
        {
            return obj.SaftyGetProperty<int, IHierarchicalTerm>(o => o.Level);
        }

        public bool ShowDistance(object obj)
        {
            return false;
        }

        public DateTime GetStart(object obj)
        {
            var term = (ITerm)obj;
            return term.Start;
        }

        public double GetTop(object obj)
        {
            return 19;
        }

        public double GetHeight(object obj)
        {
            return 4;
        }

        public Brush GetBackground(object obj)
        {
            SolidColorBrush brush = null;

            obj.SaftyInvoke<AdherenceTerm>(o => {
                                                   brush = new SolidColorBrush(o.Text == "LateToWork" ? Colors.Tomato : Colors.Green);
                                                });
            obj.SaftyInvoke<AdherenceEvent>(o =>
            {
                //if (o.Remark == "added")
                    brush = new SolidColorBrush(o.Text == "LateToWork" ? Colors.LightPink : Colors.LightGreen);
            });

            return brush ?? new SolidColorBrush(Colors.White) { Opacity = 0.9 };

        }

        public Brush GetForeground(object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(object obj) { return true; }


        public ImageSource GetImage(object obj)
        {
            return null;
        }

        public bool GetLocked(object obj)
        {
            return false;
        }

        public bool CanConvert(object obj)
        {
            return obj is AdherenceEvent;
        }

        public bool IsDirty(object obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (BlockChanged != null)
            {
                BlockChanged.GetInvocationList().ForEach(o =>
               {
                   BlockChanged -= (Action<object, bool>)o;
               });
            }
            BlockChanged = null;
        }

        public event Action<object , bool> BlockChanged;

        public bool ShowText { get; set; }
    }
}
