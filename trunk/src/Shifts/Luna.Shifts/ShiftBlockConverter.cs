using System;
using System.Collections;
using System.Windows.Media;
using Luna.Common;
using Luna.Shifts.Domain;
using System.Linq;
using Luna.Core.Extensions;
using System.Collections.Generic;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts
{
    public class ShiftBlockConverter : IBlockConverter
    {
        private readonly BrushConverter _brushConverter = new BrushConverter();
        private readonly string[] _stringColors;
        private Action _externalDelegate;
        public ShiftBlockConverter()
        {
            _stringColors = typeof(Colors).GetProperties().Select(o => o.Name.ToLower()).ToArray();
        }

        public string GetContentText(object obj)
        {
            var term = (Term)obj;
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
            var term = (IHierarchicalTerm)obj;
            return term.Level;
        }

        public void Refresh()
        {
        }

        public bool ShowDistance(object obj)
        {
            var term = (IHierarchicalTerm)obj;
            return term.Level == 0;
        }

        public DateTime GetStart(object obj)
        {
            var term = (ITerm)obj;
            return term.Start;
        }

        public double GetTop(object obj)
        {
            var term = (Term)obj;
            var level = term.Level;

            if (level == 0)
                return 6;
            if (level == 1)
                return 4;
            if (level == 2)
                return 1;
            return 6;
        }

        public double GetHeight(object obj)
        {
            return obj.Is<AbsentEvent>() ? 7 : 15;
        }

        public Brush GetBackground(object obj)
        {
            var color = default(Color);
            var term = (Term)obj;
            
            if (term == null || term.Background == null)
                color = Colors.Black;
            else
            {
                if (!_stringColors.Contains(term.Background.ToLower()))
                    color = Colors.Black;
                var colorBrush = _brushConverter.ConvertFromString(term.Background) as SolidColorBrush;
                if (colorBrush != null)
                {
                    return colorBrush;
                }
            }
            return new SolidColorBrush(color);
        }

        public Brush GetForeground(object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(object obj)
        {
            return true;
        }

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
            return true;
        }

        public bool IsDirty(object obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_externalDelegate != null)
            {
                _externalDelegate.GetInvocationList().ForEach(o =>
                {
                    _externalDelegate -= (Action)o;
                });
            }
            _externalDelegate = null;
        }
    }
}