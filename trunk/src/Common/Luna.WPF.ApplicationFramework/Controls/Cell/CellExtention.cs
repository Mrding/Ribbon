using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Luna.WPF.ApplicationFramework.Controls.Cell
{
    public static class CellExtention
    {
        /// <summary>
        /// CellDesiredSzie
        /// </summary>
        /// <param name="point">Absolute point</param>
        /// <param name="rowHeight"></param>
        /// <param name="axisXConverter"></param>
        /// <param name="renderSize"></param>
        /// <param name="scrollInfo"></param>
        /// <returns></returns>
        public static Rect? CellDesiredSzie(this Point point, double rowHeight, IHorizontalControl axisXConverter, Size renderSize, double yOffset, double xOffset, double maxOfy)
        {
            var cellStart = axisXConverter.ToData(point.X).Date;

            //xvar y = ((int)(point.Y / rowHeight)) * rowHeight;
            var y = point.Y - (point.Y % rowHeight);

            if (maxOfy <= y)
                y -= rowHeight;

            var x = axisXConverter.DataToScreen(cellStart) + xOffset;
            var xEnd = axisXConverter.DataToScreen(cellStart.AddDays(1)) + xOffset; // 一天宽度x轴座标, 未来期望可以不是一天固定死

            var rect = new Rect(x, y, xEnd - x, rowHeight);

            if (renderSize.Width < rect.X - xOffset || renderSize.Height < rect.Y - yOffset)
                return null;

            return rect;
        }
    }
}