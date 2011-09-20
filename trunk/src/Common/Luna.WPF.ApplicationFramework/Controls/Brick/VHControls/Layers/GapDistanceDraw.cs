using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    internal class GapDistanceDraw : ElementDraw<GapDistance>
    {
        //public override bool CanDraw()
        //{
        //    var blocknewPositionInfoList = Element.BlocknewPositionInfoList;
        //    if (!Element.IsLoaded || blocknewPositionInfoList == null || blocknewPositionInfoList.Count == 0 || Element.Visibility != Visibility.Visible)
        //        return false;
        //    var blockConverter = Element.BlockConverter;

        //    if (!blockConverter.CanConvert(blocknewPositionInfoList[0].Target)) return false;
        //    return true;
        //}

        protected override void InternalDraw(DrawingContext dc)
        {
            //var blocknewPositionInfoList = Element.BlocknewPositionInfoList;
           // var blockConverter = Element.BlockConverter;

            //int currentIndex =Element.AxisYConverter.ScreenToData(0);
            
            //foreach (var item in blocknewPositionInfoList)
            //{
            //    if (!blockConverter.ShowDistance(item.Target))
            //    {
            //        continue;
            //    }
            //    double height = blockConverter.GetHeight(item.Target);
            //    double top = blockConverter.GetTop(item.Target) + (item.Index - currentIndex) *Element.Interval + height / 2;

            //    object before, after;
            //     GetIndex(Element.ItemsSource[item.Index], item.Target, out before, out after);

            //    var startTime = before != null ? blockConverter.GetEnd(before) : Element.MinTime;
            //    var endTime = after != null ? blockConverter.GetStart(after) : Element.MaxTime;

            //    dc.PushTransform(new TranslateTransform(0, top));
            //    if (startTime != default(DateTime))
            //    {
            //        var geometryLeft = DrawArrowLineWithText(startTime, item.NewStart, true);
            //        dc.DrawDrawing(geometryLeft);
            //    }
            //    if (endTime != default(DateTime))
            //    {
            //        var geometryRight = DrawArrowLineWithText(item.NewEnd, endTime, false);
            //        dc.DrawDrawing(geometryRight);
            //    }
            //    dc.Pop();
            //    Element.AddYGuidelines(top + _halfPenWidth);
            //}
            //Element.AddXGuidelines(Element.RenderSize.Width);
            //Element.AddYGuidelines(Element.RenderSize.Height);
            base.InternalDraw(dc);
        }

        private DrawingGroup DrawArrowLineWithText(DateTime leftTime, DateTime rightTime, bool isLeft)
        {
            if (leftTime > rightTime)
                return new DrawingGroup();
            var timeSpan = rightTime.Subtract(leftTime);
            var axisXConverter = Element.AxisXConverter;
            var origStartX = axisXConverter.DataToScreen(leftTime);
            var origEndX = axisXConverter.DataToScreen(rightTime);
            var startX = Math.Max(0, origStartX);
            var endX = Math.Min(Element.RenderSize.Width, origEndX);

            var text = string.Format(Element.TimeSpanFormatter, "{0}", timeSpan);
            double textWidth;

            Rect rect = new Rect(startX, 0, Math.Max(0, endX - startX), 0);

            Element.AddXGuidelines(rect.Left + _halfPenWidth);
            Element.AddXGuidelines(rect.Right + _halfPenWidth);

            //画带有时间的线条，线条被切成两段
            var drawingGroup = DrawBetweenLineTime(rect, text, out textWidth);

            //是否显示箭头
            var origSub = origEndX - origStartX;
            var newSub = endX - startX;
            if (origSub / 2 > textWidth && origSub == newSub)
            {
                Geometry arrowGeometry;
                if (isLeft)
                {
                    arrowGeometry = DrawArrow(new Point(rect.Left, rect.Top), true);
                }
                else
                {
                    arrowGeometry = DrawArrow(new Point(rect.Right, rect.Top), false);
                }

                drawingGroup.Children.Add(CreateDrawing(null, arrowGeometry));
            }
            //如果间距太小就不显示字
            else if (textWidth > newSub)
            {
                return new DrawingGroup();
            }

            return drawingGroup;
        }

        private DrawingGroup DrawBetweenLineTime(Rect rect, string text, out double textWidth)
        {
            var formatted = new FormattedText(text,
                                                        CultureInfo.CurrentCulture,
                                                        FlowDirection.LeftToRight,
                                                        new Typeface("Verdana"),
                                                        9,
                                                        Brushes.Black);
            //formatted.SetFontWeight(FontWeights.Bold);
            formatted.TextAlignment = TextAlignment.Center;
            textWidth = formatted.Width;

            var sectionGroup = new DrawingGroup();
            var textPoint = new Point((rect.Right + rect.Left) / 2, rect.Top - formatted.Height / 2);
            //var textGeometry = formatted.BuildGeometry(textPoint);
            //VisualXSnappingGuidelines.Add(textPoint.X + _halfPenWidth);
            //RenderOptions.SetEdgeMode(textGeometry, EdgeMode.Unspecified);
            var drawingContext = sectionGroup.Open();
            drawingContext.DrawText(formatted, textPoint);
            drawingContext.Close();
            //sectionGroup.Children.Add(CreateDrawing(null, textGeometry));            


            if (rect.Width / 2 > textWidth)
            {
                var line1 = CreateLine(new Point(rect.Left, rect.Top),
                    new Point((rect.Right + rect.Left) / 2 - formatted.Width, rect.Top));
                sectionGroup.Children.Add(CreateDrawing(null, line1));
                var line2 = CreateLine(new Point((rect.Right + rect.Left) / 2 + formatted.Width, rect.Top),
                    new Point(rect.Right, rect.Top));
                sectionGroup.Children.Add(CreateDrawing(null, line2));
                Element.AddXGuidelines(line1.EndPoint.X + _halfPenWidth);
                Element.AddXGuidelines(line2.StartPoint.X + _halfPenWidth);
            }

            return sectionGroup;
        }

        private readonly double _arrowLen = Math.Sqrt(3) * 2;
        private Geometry DrawArrow(Point point, bool isleft)
        {
            var tempLen = isleft ? _arrowLen : -_arrowLen;
            var upPoint = new Point(point.X + tempLen, point.Y - Math.Tanh(30) * _arrowLen);
            var downPoint = new Point(point.X + tempLen, Math.Tanh(30) * _arrowLen + point.Y);

            var g = new StreamGeometry();
            using (StreamGeometryContext context = g.Open())
            {
                context.BeginFigure(point, true, false);
                context.LineTo(upPoint, true, false);
                context.LineTo(downPoint, true, false);
            }
            g.Freeze();
            Element.AddXGuidelines(downPoint.X + _halfPenWidth);
            Element.AddXGuidelines(upPoint.X + _halfPenWidth);

            return g;
        }

        private static LineGeometry CreateLine(Point start, Point end)
        {
            var line = new LineGeometry {StartPoint = start, EndPoint = end};
            line.Freeze();
            return line;
        }

        private Pen _pen;
        private double _halfPenWidth;

        private Pen CreatePen()
        {
            if (_pen == null)
            {
                _pen = new Pen { Brush = Brushes.Black, Thickness = 1 };
                _pen.Freeze();
                _halfPenWidth = _pen.Thickness / 2;
            }
            return _pen;
        }

        private Drawing CreateDrawing(Brush brush, Geometry geometry)
        {
            var pen = CreatePen();
            var geometryDrawing = new GeometryDrawing(Brushes.Black, pen, geometry);
            geometryDrawing.Freeze();
            return geometryDrawing;
        }

        internal static int GetIndex(IEnumerable items, object item, out object before, out object after)
        {
            int i = 0;
            int j = -1;
            before = null;
            after = null;

            foreach (var o in items)
            {
                if (j != -1)
                {
                    after = o;
                    return j;
                }
                else if (o.Equals(item))
                {
                    j = i;
                }
                else
                {
                    before = o;
                }
                i++;
            }
            return j;
        }
    }
}
