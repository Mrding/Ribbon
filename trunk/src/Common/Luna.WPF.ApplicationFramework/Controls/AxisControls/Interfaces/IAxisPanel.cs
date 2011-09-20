
using System;
using System.Windows.Controls.Primitives;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public interface IAxisPanel : IScrollInfo
    {
        void Add(AxisControl element);
        void AddHorizontalControl(AxisControl element);

        ///<summary>
        ///Get (x,0) row index
        ///</summary>
        ///<returns></returns>
        int GetScreenTopRowIndex();
        //void AddVerticalControl(AxisControl element);

        
    }
}
