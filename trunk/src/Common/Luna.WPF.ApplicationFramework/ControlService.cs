using System.Windows;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework
{
    public class ControlService
    {
        public static string GetCheckedImage(DependencyObject obj)
        {
            return (string)obj.GetValue(CheckedImageProperty);
        }

        public static void SetCheckedImage(DependencyObject obj, string value)
        {
            obj.SetValue(CheckedImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for CheckedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedImageProperty =
            DependencyProperty.RegisterAttached("CheckedImage", typeof(string), typeof(ControlService), new UIPropertyMetadata(string.Empty));


        public static string GetUncheckedImage(DependencyObject obj)
        {
            return (string)obj.GetValue(UncheckedImageProperty);
        }

        public static void SetUncheckedImage(DependencyObject obj, string value)
        {
            obj.SetValue(UncheckedImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for UncheckedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UncheckedImageProperty =
            DependencyProperty.RegisterAttached("UncheckedImage", typeof(string), typeof(ControlService), new UIPropertyMetadata(string.Empty));

        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("Header", typeof(string), typeof(ControlService), new UIPropertyMetadata(string.Empty));



        public static Brush GetPressedBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedBrushProperty);
        }

        public static void SetPressedBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for PressedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressedBrushProperty =
            DependencyProperty.RegisterAttached("PressedBrush", typeof(Brush), typeof(ControlService), new UIPropertyMetadata(null));




        public static Brush GetPressedBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedBorderBrushProperty);
        }

        public static void SetPressedBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedBorderBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for PressedBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.RegisterAttached("PressedBorderBrush", typeof(Brush), typeof(ControlService), new UIPropertyMetadata(null));      



        public static bool GetEnableMouseOverStyle(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableMouseOverStyleProperty);
        }

        public static void SetEnableMouseOverStyle(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableMouseOverStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableMouseOverStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMouseOverStyleProperty =
            DependencyProperty.RegisterAttached("EnableMouseOverStyle", typeof(bool), typeof(ControlService), new UIPropertyMetadata(false));      
    }
}