using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public delegate void ItemDropEventHandler(object sender, ItemDropEventArgs e);

    public class ItemDropEventArgs : RoutedEventArgs
    {
        public int RowIndex
        {
            get;
            private set;
        }

        public DateTime DropTime
        {
            get;
            private set;
        }


        public object DropData
        {
            get;
            set;
        }

        public ItemDropEventArgs(int rowIndex, DateTime dropTime)
        {
            RowIndex = rowIndex;
            DropTime = dropTime;
        }
    }
}
