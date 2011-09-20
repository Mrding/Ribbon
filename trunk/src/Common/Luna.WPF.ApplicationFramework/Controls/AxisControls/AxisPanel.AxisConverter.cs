using System;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public abstract partial class AxisPanel
    {
        #region IHorizontalControl

        Func<DateTime, double> IHorizontalControl.DataToScreen
        {
            get { return DataToScreenX; }
        }

        Func<double, DateTime> IHorizontalControl.ScreenToData
        {
            get { return x => ScreenToDataX(x, false); }
        }

        Func<double, DateTime> IHorizontalControl.ToData
        {
            get { return x => ScreenToDataX(x, true); }
        }

        public Func<double> Step
        {
            get { return ComputeStep; }
            set { throw new NotImplementedException(); }
        }

        bool IVerticalControl.IsInViewRagne<T>(T value)
        {
            var dataType = typeof(T);
            if (dataType == typeof(int))
            {
                var rowIndex = Convert.ToInt32(value);
                return DataRangeY.Min <= rowIndex && rowIndex < DataRangeY.Max;
            }
            if (dataType == typeof(double))
            {
                // y always is relative point
                var y = Convert.ToDouble(value) + ViewportRangeY.ViewMin;

                //xy -= ViewportRangeY.ViewMin % VerticalOffSetValue;

                return ViewportRangeY.ViewMin <= y && y < ViewportRangeY.ViewMax;
            }
            return false;
        }

        

        bool IHorizontalControl.IsInViewRagne(double value)
        {
            // x always is relative point
            var x = value + ViewportRangeX.ViewMin;
            return ViewportRangeX.ViewMin <= x && x < ViewportRangeX.ViewMax;
        }

        bool IHorizontalControl.IsInDataRagne<T>(T data, bool absolutPosition)
        {
            var dataType = typeof(T);
            if (dataType == typeof(DateTime))
            {
                var dateTime = Convert.ToDateTime(data);
                return DataRangeX.Min <= dateTime && dateTime < DataRangeX.Max;
            }
            if (dataType == typeof(double))
            {
                var offset = absolutPosition ? 0 : ViewportRangeX.ViewMin; // 与 ScreenToDataX 方法内容逻辑一样

                var value = Convert.ToDouble(data) + offset;
                return ViewportRangeX.Min <= value && value < ViewportRangeX.Max;
            }
            return false;
        }

        #endregion

        #region IVerticalControl

        Func<int, double> IVerticalControl.DataToScreen
        {
            get { return DataToScreenY; }
        }

        Func<double, int> IVerticalControl.ScreenToData
        {
            get { return y=> ScreenToDataY(y, false); }
        }

        Func<double, int> IVerticalControl.ToData
        {
            get { return y => ScreenToDataY(y, true); }
        }

        #endregion

        #region Core method
        /// <summary>
        /// AbsoluteX Point
        /// </summary>
        /// <param name="viewporDateTime"></param>
        /// <returns>Absolute DataTime</returns>
        protected abstract DateTime ViewportToDataX(double viewporDateTime);
        /// <summary>
        /// AbsoluteX DataTime
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Absolute Point</returns>
        protected abstract double DataToViewportX(DateTime data);

        protected virtual DateTime ScreenToDataX(double screenX, bool absolutPosition)
        {
            var offset = absolutPosition ? 0 : _viewportRangeX.ViewMin;

            return ViewportToDataX(screenX + offset);
        }

        protected virtual double DataToScreenX(DateTime data)
        {
            return DataToViewportX(data) - _viewportRangeX.ViewMin;
        }
        protected abstract double DataToAbsoluteY(int data);

        /// <summary>
        /// AbsoluteYToDataRowIndex
        /// </summary>
        /// <param name="y">Absolute Y</param>
        /// <returns>DataRow Index</returns>
        protected abstract int AbsoluteYToDataRowIndex(double y);

        protected virtual double DataToScreenY(int data)
        {
            return DataToAbsoluteY(data) - _viewportRangeY.ViewMin;
        }

        /// <summary>
        /// Get DataRow Index (Viewport)
        /// </summary>
        /// <param name="screenY">Relative Y</param>
        /// <param name="absolutPosition">y + VertialOffset</param>
        /// <returns>DataRow Index</returns>
        protected virtual int ScreenToDataY(double screenY, bool absolutPosition)
        {
            if (screenY == 0d)
                return DataRangeY.ViewMin;

            //if (screenY == ViewportRangeY.ViewMax)
            //    return DataRangeY.ViewMax;

            var offset = absolutPosition ? 0 : _viewportRangeY.ViewMin;

            return AbsoluteYToDataRowIndex(offset + screenY);
        }

        public abstract int GetScreenTopRowIndex();

        protected abstract double ComputeStep();

        #endregion
    }
}
