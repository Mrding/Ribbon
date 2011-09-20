using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ActiproSoftware.Windows.Controls.Ribbon.Controls;
using Luna.Common;
using Luna.Globalization;
using Luna.Core;
using System.Text;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public static class MultipleSelectionBehavior
    {


        public static string GetNoSelectionText(DependencyObject obj)
        {
            return (string)obj.GetValue(NoSelectionTextProperty);
        }

        public static void SetNoSelectionText(DependencyObject obj, string value)
        {
            obj.SetValue(NoSelectionTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for NoSelectionText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoSelectionTextProperty =
            DependencyProperty.RegisterAttached("NoSelectionText", typeof(string), typeof(MultipleSelectionBehavior), new UIPropertyMetadata("?"));



        public static object GetDisplayContent(DependencyObject obj)
        {
            return (object)obj.GetValue(DisplayContentProperty);
        }

        public static void SetDisplayContent(DependencyObject obj, object value)
        {
            obj.SetValue(DisplayContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for PopupContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayContentProperty =
            DependencyProperty.RegisterAttached("DisplayContent", typeof(object), typeof(MultipleSelectionBehavior), new UIPropertyMetadata(null, (d, args) =>
            {
                d.SaftyInvoke<ListBox>(lb =>
                {
                    args.NewValue.SaftyInvoke<PopupButton>(pb =>
                    {
                        pb.PopupClosed += (sender, e) =>
                        {
                            pb.Label = GenerateDisplay(lb.ItemsSource.As<IEnumerable>(),d);
                        };
                        pb.Label = GenerateDisplay(lb.ItemsSource.As<IEnumerable>(),d);
                    });
                });
            }));

        internal static string GenerateDisplay(IEnumerable collection, DependencyObject d)
        {
            if (collection == null) return string.Empty;
            
            var selectedItems = new StringBuilder();

            foreach (ISelectable item in collection.Cast<ISelectable>().Where(item => item.IsSelected == true))
                selectedItems.AppendFormat(",{0}", item);

            if (selectedItems.Length == 0)
                return GetNoSelectionText(d);

            return selectedItems.ToString().Remove(0, 1);
        }
    }



    public static class PopupButtonBehavior
    {
        public static object GetPopupContent(DependencyObject obj)
        {
            return (object)obj.GetValue(PopupContentProperty);
        }

        public static void SetPopupContent(DependencyObject obj, object value)
        {
            obj.SetValue(PopupContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for PopupContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.RegisterAttached("PopupContent", typeof(object), typeof(PopupButtonBehavior), new UIPropertyMetadata(null, (d, args) =>
            {
                PopupButton popup = d as PopupButton;
                var listBox = args.NewValue as System.Windows.Controls.ListBox;
                //for the first time
                popup.Loaded += (sender, e) =>
                {
                    if (popup.DataContext != null)
                    {
                        IEnumerable source = null;
                        var listBoxItemsSourceBinding = BindingOperations.GetBinding(listBox, ItemsControl.ItemsSourceProperty);
                        if (listBoxItemsSourceBinding.Path == null || string.IsNullOrEmpty(listBoxItemsSourceBinding.Path.Path))
                        {
                            source = popup.DataContext as IEnumerable;
                        }
                        else
                        {
                            source = popup.DataContext.GetType().GetProperty(listBoxItemsSourceBinding.Path.Path).GetValue(popup.DataContext, null) as IEnumerable;
                        }
                        if (source != null)
                        {
                            var itemsSource = source.OfType<ISelectable>();
                            if (itemsSource.Count() > 0)
                            {
                                popup.Label = GenerateDisplay(itemsSource.Where(o => o.IsSelected == true).ToList());
                                return;
                            }
                        }
                    }
                    popup.Label = LanguageReader.GetValue("ApplicationFramework_Controls_PopupButtonBehavior_Text");
                };

                if (listBox == null) return;

                listBox.SelectionChanged += (sender, e) =>
                {
                    if (popup.IsPopupOpen)
                        popup.Label = GenerateDisplay(listBox.SelectedItems);
                };
            }));

        internal static string GenerateDisplay(IList list)
        {
            var display = new StringBuilder();
            if (list.Count == 0)
                return LanguageReader.GetValue("ApplicationFramework_Controls_PopupButtonBehavior_Text");

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                var itemText = new Reflector(item).Property<string>("Name") ?? item.ToString();
                display.Append(itemText);
                if (i != list.Count - 1)
                    display.Append(", ");
            }
            return display.ToString();
        }
    }
}
