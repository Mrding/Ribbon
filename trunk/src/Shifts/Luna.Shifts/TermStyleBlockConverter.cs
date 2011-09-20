using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Shifts.Domain;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts
{
    public class TermStyleBlockConverter : IBlockConverter
    {
        private readonly BrushConverter _brushConverter = new BrushConverter();
        private readonly string[] _stringColors;

        public TermStyleBlockConverter()
        {
            _stringColors = typeof(Colors).GetProperties().Select(o => o.Name.ToLower()).ToArray();
        }

        public string GetContentText(object obj)
        {
            var text = obj.SaftyGetProperty<string, IVisibleTerm>(o => o.Text);
            return text;
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
            var term = (IHierarchicalTerm)obj;
            return term.Level;
        }

        public void Refresh()
        {
        }

        public bool ShowDistance(object obj)
        {
            return false;
        }

        public bool GetLocked(object obj)
        {
            return false;
        }

        public bool CanConvert(object obj)
        {
            return true;
        }

        public bool IsDirty(object obj)
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
            var term = (IHierarchicalTerm)obj;
            var level = term.Level;

            if (level == 0)
                return 25;
            if (level == 1)
                return 8;
            if (level == 2)
                return 8;
            return 6;
        }

        public double GetHeight(object obj)
        {
            var term = (IHierarchicalTerm)obj;
            if (term.Level == 3)
                return 7;
            if (term.Level == 0)
                return 18;
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

        public Brush GetBackground(object obj)
        {
            var term = (IStyledTerm)obj;

            if (term == null || string.IsNullOrEmpty(term.Background))
                return new SolidColorBrush(Colors.Black);
            if (!_stringColors.Contains(term.Background.ToLower()))
                return new SolidColorBrush(Colors.Black);
            var colorBrush = _brushConverter.ConvertFromString(term.Background) as SolidColorBrush;
            if (colorBrush != null)
                return new SolidColorBrush((colorBrush.Color));

            return Brushes.Black;
        }

        //public void SetNewTime(IList<IEnumerable> itemsSource, IEnumerable<BlockNewPositionInfo> list)
        //{
        //    var item = list.First();

        //    var m = new Func<DateTime, DateTime, bool>((start, end) =>
        //    {
        //        var targetTerm = (IHierarchicalTerm)item.Target;

        //        var baseTerm = (ITerm)itemsSource[item.Index];

        //        var targetTermNotEqualToContainer = !ReferenceEquals(baseTerm, item.Target);

        //        if (targetTermNotEqualToContainer)
        //            if (!(start >= baseTerm.Start && end <= baseTerm.End))
        //                return false;

        //        foreach (IHierarchicalTerm term in itemsSource[item.Index])
        //        {
        //            if (ReferenceEquals(term, item.Target)) continue;
        //            if (targetTerm.Level == term.Level)
        //            {
        //                if (term.AnyOverlap(start, end)) return false;
        //            }
        //        }
        //        if (((ICanConvertToValueTerm)item.Target).SetNewTime(start, end))
        //        {
        //            ((IEditingObject)itemsSource[item.Index]).IsEditing = true;
        //            return true;
        //        }
        //        return false;                                            
        //    });

        //    if (m(item.NewStart, item.NewEnd) && _externalDelegate != null)
        //        _externalDelegate();
        //}

        public void Dispose()
        {
        }
    }
}
