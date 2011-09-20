using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using Luna.Common;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;

namespace Luna.GUI.Views.Shifts
{
    public class OccupationMaskConverter : IBlockConverter
    {
        private static Brush GbColor = new SolidColorBrush(Colors.Silver) { Opacity = 0.9 };

        public string GetContentText(object obj)
        {
            var oc = obj as SeatArrangement;
            return oc.Seat.Number;
        }

        public void Refresh()
        {

        }

        public double FontSize
        {
            get { return 10; }
        }

        public DateTime GetEnd(object obj)
        {
            var term = (ITerm)obj;
            return term.End;
        }

        public int GetLevel(object obj)
        {
            return 0;
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
            return 6;
        }

        public double GetHeight(object obj)
        {
            return 13;
        }

        public Brush GetBackground(object obj)
        {
            return GbColor;
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
            return true;
        }

        public bool IsDirty(object obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}