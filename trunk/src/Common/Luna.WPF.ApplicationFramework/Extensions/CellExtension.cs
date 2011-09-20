using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Luna.WPF.ApplicationFramework.Controls;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class CellExtension
    {
        //bool isNumPadNumeric = (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
        //bool isNumeric = ((e.Key >= Key.D0 && e.Key <= Key.D9) && (e.KeyboardDevice.Modifiers == ModifierKeys.None));
        //bool isDecimal = ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && (((TextBox)sender).Text.IndexOf('.') < 0));

        public static bool IsAlphabetKey(this Key key)
        {
            return (key > Key.A && key < Key.Z);
        }

        public static bool IsNumberKey(this KeyEventArgs e)
        {
            bool isNumPadNumeric = (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
            bool isNumeric = ((e.Key >= Key.D0 && e.Key <= Key.D9) && (e.KeyboardDevice.Modifiers == ModifierKeys.None));

            return isNumPadNumeric || isNumeric;
        }

        public static bool IsNavigateKey(this Key key)
        {
            return key == Key.Left || key == Key.Up || key == Key.Down || key == Key.Right;
        }

        public static bool IsMultiSelected(this FrameworkElement element, bool CanMultiSelected)
        {
            return CanMultiSelected && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        public static bool IsMultiSelected(this FrameworkElement element)
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }


    }

    public static class AxisExtension
    {
        public static Point AbsolutPosition(this Point point, BlockGridLayerBase layer)
        {
            point.Y += layer.VerticalOffset;
            point.X += layer.HorizontalOffset;
            return point;
        }
    }
}
