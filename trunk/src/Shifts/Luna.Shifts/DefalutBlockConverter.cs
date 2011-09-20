using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.Common.Domain;

namespace Luna.Shifts
{
    public interface IDefalutBlockConverter : IBlockConverter
    {
        event Action<object, bool> BlockChanged;
        bool ShowText { set; get; }
    }

    public class DefalutBlockConverter : IDefalutBlockConverter
    {
        public event Action<object, bool> BlockChanged;

        private readonly BrushConverter _brushConverter = new BrushConverter();
        private readonly string[] _stringColors;
    
        private static readonly GifBitmapDecoder LockIcon = new GifBitmapDecoder(new Uri("pack://application:,,,/Luna.GUI;component/Resources/Images/locker_icon.gif", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.None, BitmapCacheOption.Default);

        public DefalutBlockConverter()
        {
            _stringColors = typeof(Colors).GetProperties().Select(o => o.Name.ToLower()).ToArray();
         
            ShowText = true;
        }

        public bool ShowText { set; get; }

        public string GetContentText(object obj)
        {
            if (ShowText)
            {
                return obj.SaftyGetProperty<string, IVisibleTerm>(o => o.Text);
                //return FilterWithOcupyStatus && term.Is<AssignmentBase>() && term.OcuppiedAndHasSeat() ? string.Format("{0}(已排座)", term.Text) : term.Text;
            }
            return null;
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
            return obj.SaftyGetProperty<int, IHierarchicalTerm>(o => o.Level) == 0;
        }

        public bool GetLocked(object obj)
        {
            return false;
        }

        public bool CanConvert(object obj)
        {
            return obj is Term;
        }

        public bool IsDirty(object obj)
        {
            throw new NotImplementedException();
        }

        public DateTime GetStart(object obj)
        {
            var term = (ITerm)obj;
            return term.Start;
        }

        public double GetTop(object obj)
        {
            if (obj.Is<IImmutableTerm>())
                return 0;
            var level = obj.SaftyGetProperty<int, IHierarchicalTerm>(o => o.Level);

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
            if (obj is UnknowAssignment)
                return -1;
            //if (obj.Is<IImmutableTerm>())
            //    return 20;
            return obj.Is<AbsentEvent>() ? 5 : 15;
        }


        public Brush GetForeground(object obj)
        {
            throw new NotImplementedException();
        }

        public ImageSource GetImage(object obj)
        {
            var term = (Term)obj;

            if (term.Locked && term.IsNot<AbsentEvent>())
                return LockIcon.Frames[0];
            return null;
        }

        public Brush GetBackground(object obj)
        {
            var color = default(Color);
            var term = obj as IStyledTerm;

            //if (FilterWithOcupyStatus && (term.OcuppiedAndHasSeat() || (term.IsNeedSeat && (term.GetLowestTerm() as Term).OcuppiedAndHasSeat())))
            //{
            //    return new SolidColorBrush(Colors.Silver);
            //}

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

        public void Dispose(){}
    }
}
