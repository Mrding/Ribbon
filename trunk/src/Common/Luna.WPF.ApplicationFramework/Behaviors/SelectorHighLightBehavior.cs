using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Markup;
using Luna.WPF.ApplicationFramework.Threads;
using System.Windows.Threading;

namespace Luna.WPF.ApplicationFramework.Behaviors
{
    public class SelectorHighLightBehavior : BehaviorBase<TextBox>
    {
        protected override void Initialize()
        {
            AssociatedObject.KeyUp += AssociatedObjectKeyUp;
            AssociatedObject.Unloaded += (sender, e) =>
                                             {
                                                 if (Selector.Items.Filter != null)
                                                     Selector.Items.Filter -= FilterWithWords;

                                                 ((TextBox) sender).Text = string.Empty;
                                             };
            DelayBinding.SetBinding(AssociatedObject, TextBox.TextProperty, TimeSpan.FromSeconds(0.3), new Binding("SearchText") { Source = this, Mode = BindingMode.TwoWay });
        }

        protected override void Uninitialize()
        {
            BindingOperations.ClearBinding(AssociatedObject, TextBox.TextProperty);
            AssociatedObject.KeyUp -= AssociatedObjectKeyUp;
           
            if (Selector.Items.Filter != null)
                Selector.Items.Filter -= FilterWithWords;
            Selector = null;
            BindingOperations.ClearAllBindings(this);
        }

        void AssociatedObjectKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && Selector.HasItems)
            {
                var currentIndex = 1 + Selector.SelectedIndex;
                if (currentIndex >= Selector.Items.Count)
                    currentIndex = 0;

                if (Selector.SelectedIndex == -1)
                    Selector.SelectedIndex = 0;


                Selector.Focus();

                Selector.ItemContainerGenerator.ContainerFromIndex(currentIndex).SaftyInvoke<IInputElement>(
                    item => item.Focus());
            }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SelectorHighLightBehavior),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                sender.SaftyInvoke<SelectorHighLightBehavior>(o =>
                {
                    o.SetFilter();
                    UIThread.BeginInvoke(() =>
                    {
                        if (o.Selector.Items.Count == 0) return;
                        o.Selector.ScrollIntoView(o.Selector.Items[0]);
                        o.HighLight();
                    });
                });

            }));



        public bool AutoSelectedFirstItem
        {
            get { return (bool)GetValue(AutoSelectedFirstItemProperty); }
            set { SetValue(AutoSelectedFirstItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoSelectedFirstItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSelectedFirstItemProperty =
            DependencyProperty.Register("AutoSelectedFirstItem", typeof(bool), typeof(SelectorHighLightBehavior), new UIPropertyMetadata(true));



        public ListBox Selector
        {
            get { return (ListBox)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        public static readonly DependencyProperty SelectorProperty =
            DependencyProperty.Register("Selector", typeof(ListBox), typeof(SelectorHighLightBehavior),
            new FrameworkPropertyMetadata(null, (d, e) =>
            {
                e.NewValue.SaftyInvoke<ListBox>(l =>
                                                    {
                                                        var textBox = d.SaftyGetProperty<UIElement, SelectorHighLightBehavior>(o => o.AssociatedObject);
                                                        l.KeyDown += (s, arg) =>
                                                                       {
                                                                           if (l.SelectedIndex == 0 && arg.Key == Key.Up)
                                                                           {
                                                                               l.SelectedIndex = -1;
                                                                               textBox.Focus();
                                                                           }
                                                                               
                                                                       };
                                                    });
            }));

        public string GetText()
        {
            return SearchText.Trim().ToLower();
        }

        public string FilterPath { get; set; }

        public string BindableValue
        {
            get { return (string)GetValue(BindableValueProperty); }
            set { SetValue(BindableValueProperty, value); }
        }

        public static readonly DependencyProperty BindableValueProperty =
            DependencyProperty.Register("BindableValue", typeof(string), typeof(SelectorHighLightBehavior),
            new UIPropertyMetadata(string.Empty));

        private void SetFilter()
        {
            Selector.Items.Filter -= FilterWithWords;
            if (string.IsNullOrEmpty(GetText()))
            {
                return;
            }
            Selector.Items.Filter += FilterWithWords;
        }

        private bool FilterWithWords(object obj)
        {
            var origin = obj.ToString();
            if (!string.IsNullOrEmpty(FilterPath))
            {
                BindingOperations.SetBinding(this, BindableValueProperty, new Binding(FilterPath) { Source = obj });
                origin = BindableValue;
            }
            if (origin == null) return false;

            var formatText = origin.ToLower().Trim();
            //return formatText.StartsWith(GetText().ToLower().Trim());
            return formatText.Contains(GetText());
        }

        private void HighLight()
        {
            var searchText = GetText();

            object firstMatchedItem = null;

            foreach (var item in Selector.Items)
            {
                var itemElement = Selector.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (itemElement == null) continue;

                string data;
                if (!string.IsNullOrEmpty(FilterPath))
                {
                    BindingOperations.SetBinding(this, BindableValueProperty, new Binding(FilterPath) { Source = itemElement.DataContext });
                    data = BindableValue;
                }
                else
                    data = itemElement.DataContext.ToString();

                if (searchText.Length <= data.Length)
                {
                    var textblock = itemElement.FindVisualChild<TextBlock>();
                    if (textblock != null) // found match
                    {
                        if (firstMatchedItem == null)
                            firstMatchedItem = item;
                        textblock.Text = string.Empty;

                        var hightlightFirstCharIndex = data.ToLower().IndexOf(searchText);

                        if (hightlightFirstCharIndex == 0)
                        {
                            textblock.Inlines.Add(new Bold(new Run(data.Substring(0, searchText.Length))));
                        }
                        else if (0 < hightlightFirstCharIndex)
                        {
                            var firstPartText = data.Substring(0, hightlightFirstCharIndex);
                            textblock.Inlines.Add(new Run(firstPartText));
                            textblock.Inlines.Add(new Bold(new Run(data.Substring(hightlightFirstCharIndex, searchText.Length))));
                        }

                        var lastPartCharIndex = hightlightFirstCharIndex + searchText.Length;
                        if (lastPartCharIndex < data.Length)
                        {
                            var lastPartText = data.Substring(lastPartCharIndex);
                            textblock.Inlines.Add(new Run(lastPartText));
                        }
                    }
                }
            }

            if (AutoSelectedFirstItem)
                Selector.SelectedItem = firstMatchedItem;
        }
    }
}
