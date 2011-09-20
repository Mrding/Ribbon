using System;
using System.Linq;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.Shifts
{
    public class BlockToCellConverter : IBlockConverter
    {
        public static string[] StringColors = typeof(Colors).GetProperties().Select(o => o.Name.ToLower()).ToArray();

        private Brush _offWorkBackground = "#FFF9E7E7".ToBrush(0.4);
        private Brush _dayOffFroeground = Brushes.Silver;
        private Brush _newTermForeground = "#3079ED".ToBrush(1);
        private Brush _foreground = Brushes.Black;
        private bool _showDayOffText = true;

        public BlockToCellConverter(Brush dayOffForeground, Brush newTermForeground, Brush foreground)
        {
            dayOffForeground.Freeze();
            newTermForeground.Freeze();
            foreground.Freeze();

            _foreground = foreground;
            _dayOffFroeground = dayOffForeground;
            _newTermForeground = newTermForeground;
        }

        public Brush Foreground { get { return _foreground; } set { _foreground = value; } }

        public BlockToCellConverter ShowDayOffText(bool value)
        {
            _showDayOffText = value;
            return this;
        }

        public BlockToCellConverter()
        {
            
        }

        public void Dispose()
        {
        }

        public double FontSize
        {
            get { return 12; }
        }

        public DateTime GetEnd(object obj)
        {
            var term = (ITerm)obj;
            return term.Start.Date.AddDays(1);
        }

        public int GetLevel(object obj)
        {
            return 0;
        }

        public DateTime GetStart(object obj)
        {
           return  obj.SaftyGetProperty<DateTime?, IAssignment>(t => t.SaftyGetHrDate()) ?? obj.SaftyGetProperty<DateTime, ITerm>(t => t.Start);
        }

        public double GetTop(object obj)
        {
            return 0;
        }

        public double GetHeight(object obj)
        {
            return 25d; // reference from BrickControl.xaml
        }

        public Brush GetBackground(object obj)
        {
            //var term = obj as IStyledTerm;

            //if (term == null || term.Background == null)
            //    return Brushes.Transparent;
            //return term.Background.ToBrush(1.0);
            return obj.Is<IOffWork>() ? _offWorkBackground : null;
        }

        public Brush GetForeground(object obj)
        {
            if (obj is UnknowAssignment)
                return _dayOffFroeground;
            return obj.SaftyGetProperty<Brush, Term>(t => t.IsNew() ? _newTermForeground : _foreground, () => _foreground);
        }

        public ImageSource GetImage(object obj)
        {
            throw new NotImplementedException();
        }

        public string GetContentText(object obj)
        {
            if (!_showDayOffText && obj is UnknowAssignment)
                return string.Empty;
            return obj.SaftyGetProperty<string, IVisibleTerm>(o => o.Text);
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
            return obj is ITerm;
        }

        public bool IsDirty(object obj)
        {
            return obj.SaftyGetProperty<bool, DateTerm>(d => d.IsDirty);
        }
    }
}
