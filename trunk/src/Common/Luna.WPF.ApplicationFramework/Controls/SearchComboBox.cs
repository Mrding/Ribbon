using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Luna.Common;
using System.Collections;
using System.ComponentModel;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public class SearchComboBox : ComboBox
    {
        public SearchComboBox()
        {
            this.IsEditable = true;
            this.IsTextSearchEnabled = false;
            this.Loaded += new RoutedEventHandler(SearchComboBox_Loaded);

        }

        void SearchComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(new FrameworkElement())) return;
            if (File.Exists(SaveModelFolder))
            {
                Config = serializer.Deserialize<KeyConfig>(SaveModelFolder);
                this.ItemsSource = Config.LatestSearchList;
            }
            else
            {
                Config = new KeyConfig() { Key = SearchModel, LatestSearchList = new ObservableCollection<string>() };
                this.ItemsSource = Config.LatestSearchList;
            }
            this.Loaded -= SearchComboBox_Loaded;
        }

        private TextBox _searchTextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _searchTextBox = base.GetTemplateChild("PART_EditableTextBox") as TextBox;
            _searchTextBox.BorderThickness = new Thickness(0);
            _searchTextBox.BorderBrush = Brushes.Transparent;
            _searchTextBox.KeyUp += new KeyEventHandler(_searchTextBox_KeyUp);
            _searchTextBox.PreviewKeyDown += new KeyEventHandler(_searchTextBox_PreviewKeyDown);
        }

        void _searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _tempText = _searchTextBox.Text;
        }

        private string _tempText;

        void _searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_searchTextBox.Text.Equals(_tempText))
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SelectedIndex = -1;
                    if (_searchTextBox.Text.Trim() == string.Empty)
                        DisplayAll();
                    else
                    {
                        this.Items.Filter = new Predicate<object>((object obj) =>
                        {
                            return obj.ToString().Contains(_searchTextBox.Text.Trim());
                        });
                    }
                    if (_searchTextBox.IsFocused)
                    {
                        if (this.Items.Count > 0)
                            this.IsDropDownOpen = true;
                    }
                }), DispatcherPriority.Render);
            };
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {

            if (this.SelectedIndex == -1) return;
            base.OnSelectionChanged(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            if (_searchTextBox.Text.Length > 0)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _searchTextBox.SelectionStart = _searchTextBox.Text.Length;
                }), DispatcherPriority.Normal);
            }
            base.OnDropDownOpened(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            DisplayAll();
            base.OnDropDownClosed(e);
        }

        private void DisplayAll()
        {
            this.Items.Filter = new Predicate<object>((object obj) =>
            {
                return true;
            });
        }

        public FrameworkElement TriggerElement
        {
            get { return (FrameworkElement)GetValue(TriggerElementProperty); }
            set { SetValue(TriggerElementProperty, value); }
        }

        public static readonly DependencyProperty TriggerElementProperty =
            DependencyProperty.Register("TriggerElement", typeof(FrameworkElement), typeof(SearchComboBox),
            new UIPropertyMetadata((o, a) =>
            {
                var element = o as SearchComboBox;
                element.RegisterTriggerEvent();
            }));

        private IList _historyList
        {
            get
            {
                return ItemsSource as IList;
            }
        }

        private void RegisterTriggerEvent()
        {
            TriggerElement.PreviewMouseLeftButtonDown += delegate
            {
                var text = _searchTextBox.Text.Trim();
                if (text.Length == 0) return;
                if (_historyList.Count == 20)
                    _historyList.RemoveAt(19);
                if (_historyList.Contains(text))
                {
                    _historyList.Remove(text);
                }
                _historyList.Insert(0, text);
                SaveModel();
            };
        }

        private static void Ensure(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(SearchComboBox),
            new UIPropertyMetadata("Resources"));

        [Serializable]
        public class KeyConfig
        {
            [XmlElement]
            public string Key { get; set; }

            [XmlArray]
            public ObservableCollection<string> LatestSearchList { get; set; }
        }

        public KeyConfig Config { get; set; }

        private string SaveFolder
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + Folder;
            }
        }

        private void EnsureFolder()
        {
            Ensure(SaveFolder);
        }

        private string SaveModelFolder
        {
            get
            {
                EnsureFolder();
                return string.Format("{0}/{1}.xml", SaveFolder, SearchModel);
            }
        }

        private IXmlSerializer serializer = new DefaultXmlSerializer();

        private void SaveModel()
        {
            EnsureFolder();
            serializer.Serialize(Config, SaveModelFolder);
        }


        public string SearchModel
        {
            get { return (string)GetValue(SearchModelProperty); }
            set { SetValue(SearchModelProperty, value); }
        }

        public static readonly DependencyProperty SearchModelProperty =
            DependencyProperty.Register("SearchModel", typeof(string), typeof(SearchComboBox),
            new UIPropertyMetadata("Global"));

    }
}
