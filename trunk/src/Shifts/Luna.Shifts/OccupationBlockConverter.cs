using System;
using System.Collections;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Common.Domain;
using System.Linq;
using System.Collections.Generic;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts
{
    public class OccupationBlockConverter : IBlockConverter
    {
        public static bool ShowBgColorByMarkProperty;

        private Brush _lightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        private Brush _lightSteelBlueBrush = new SolidColorBrush(Colors.LightSteelBlue);
        private Brush _silverBrush = new SolidColorBrush(Colors.Silver);


        public event Action<bool> BlockChanged;
        private Action _externalDelegate;
        public string GetContentText(object obj)
        {
            var term = (IVisibleTerm)obj;
            return term.Text;
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
            if (obj.IsNot<IWritableTerm>())
                return 1;
            return 0;
        }

        public void Refresh()
        {
            if (_externalDelegate != null)
                _externalDelegate();
        }

        public bool ShowDistance(object obj)
        {
            var term = obj as IHierarchicalTerm;

            if (term == null) return false;
            return term.Level == 0;
        }

        public DateTime GetStart(object obj)
        {
            var term = (ITerm)obj;
            return term.Start;
        }

        public double GetTop(object obj)
        {
            var term = (IHierarchicalTerm)obj;
            var level = term.Level;

            if (level == 0)
                return 4;
            if (level == 1)
                return 3;
            if (level == 2)
                return 1;
            return 4;
        }

        public double GetHeight(object obj)
        {
            return 15;
        }

        public Brush GetForeground(object obj)
        {
            throw new NotImplementedException();
        }

        public ImageSource GetImage(object obj)
        {
            return null;
        }

        public bool IsVisible(object obj) { return true; }

        public bool GetLocked(object obj)
        {
            return obj.IsNot<IWritableTerm>();
        }

        public bool CanConvert(object obj)
        {
            return true;
        }

        public bool IsDirty(object obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.BlockChanged = null;
            if (_externalDelegate != null)
            {
                _externalDelegate.GetInvocationList().ForEach(o =>
                {
                    _externalDelegate -= (Action)o;
                });
            }
            _externalDelegate = null;
            _lightGreenBrush = null;
            _lightSteelBlueBrush = null;
            _silverBrush = null;
        }


        public Brush GetBackground(object obj)
        {
            if (!ShowBgColorByMarkProperty)
            {
                return obj.Is<IWritableTerm>() ? _lightSteelBlueBrush : _lightGreenBrush;
            }
            var occupation = obj as IVisibleTerm;
            if (occupation != null && occupation.Remark == "ByEngine")
            {
                return _lightGreenBrush;
            }

            return _silverBrush;
        }

        //public void SetNewTime(IList<IEnumerable> itemsSource, IEnumerable<BlockNewPositionInfo> list)
        //{
        //    foreach (var item in list)
        //    {
        //        var start = item.NewStart;
        //        var end = item.NewEnd;

        //        var occupation = item.Target as IWritableTerm;

        //        if (occupation == null || start == end) return;
        //        if (occupation.Start == start && occupation.End == end)
        //            return;

        //        var container = itemsSource[item.Index];
        //        container.SaftyInvoke<ITermContainer>(o => o.SetTime(occupation, start, end, (t, success) =>
        //                                                {
        //                                                    if (BlockChanged != null && success)
        //                                                        BlockChanged(true);
        //                                                }));

        //    }
        //}
    }
}