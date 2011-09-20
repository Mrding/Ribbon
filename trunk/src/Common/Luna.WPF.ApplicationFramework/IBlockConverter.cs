using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections;

namespace Luna.WPF.ApplicationFramework
{

    //public class BlockNewPositionInfo
    //{
    //    public DateTime NewStart { get; set; }

    //    public DateTime NewEnd { get; set; }

    //    public int Index { get; set; }

    //    public object Target { get; set; }

    //    public override string ToString()
    //    {
    //        return String.Format("Start {0}, End {1}, Index {2}, Target {3}", NewStart, NewEnd, Index, Target);
    //    }
    //}


    public interface IBlockConverter : IDisposable
    {
        double FontSize { get; }

        DateTime GetEnd(object obj);

        int GetLevel(object obj);

        DateTime GetStart(object obj);

        double GetTop(object obj);

        double GetHeight(object obj);

        Brush GetBackground(object obj);

        Brush GetForeground(object obj);

        ImageSource GetImage(object obj);

        //void SetNewTime(IList<IEnumerable> itemsSource, IEnumerable<BlockNewPositionInfo> list);

        string GetContentText(object obj);

        //
        //string GetContentText(object obj);

        //int GetLevel(object obj);

        bool ShowDistance(object obj);

        //double GetTop(object obj);

        //double GetHeight(object obj);

        ////bool IsVisible(object obj);

        //ImageSource GetImage(object obj);

        //bool SetNewTime(IEnumerable container, object obj, DateTime start, DateTime end);

        bool GetLocked(object obj);

        bool CanConvert(object obj);

        bool IsDirty(object obj);
    }
}
