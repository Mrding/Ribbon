using System;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract partial class AxisPanel
    {
        protected void ZoomX(double screenX, double scaleX)
        {
            //得到在线段中的位置    
            var lineX = screenX + _viewportRangeX.ViewMin;

            //在放大后的线段中的位置
            var newLineX = lineX * scaleX;

            //减掉屏幕坐标得到左边坐标    
            _viewportRangeX.ViewMin = Math.Max(0, newLineX - screenX);
            
            _viewportRangeX.Max = _viewportRangeX.Max * scaleX;

            //数据的左边界值也会变化
            _dataRangeX.ViewMin = ViewportToDataX(_viewportRangeX.ViewMin);

            var viewMaxValue = Math.Min(_viewportRangeX.ViewMin + GetViewportWidth(), _viewportRangeX.Max);

            ViewportRangeX.ViewMax = viewMaxValue;
            DataRangeX.ViewMax = ViewportToDataX(ViewportRangeX.ViewMax);


            ScrollOwner.SaftyInvoke(o => o.InvalidateScrollInfo());
        }

        protected void ZoomX(double screenX, double decreaseScale, double increaseScaleX)
        {
            ZoomX(screenX, decreaseScale);
            ZoomX(screenX, increaseScaleX);
        }
    }
}