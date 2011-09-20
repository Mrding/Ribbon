using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Luna.WPF.ApplicationFramework.Graphics;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public abstract class CalibrationControlDraw<T> : ElementDraw<Ruler<T>>
    {
        private double _maxRenderWidth;
        private DrawingGroup _sectionsCache;
        private double _sectionWidth;

        public override void Attach(FrameworkElement element)
        {
            base.Attach(element);
            if (Element.Zooming == null)
                Element.Zooming = () => { _sectionsCache = null; };
        }

        protected override void InternalDraw(DrawingContext dc)
        {
            var renderSize = Element.RenderSize;

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(renderSize));
            if (_sectionsCache == null || _maxRenderWidth < renderSize.Width)
            {
                _sectionWidth = Element.Graduations.Count * Element.Interval;
                _maxRenderWidth = renderSize.Width;
                DrawSections();
            }

            //var max = Element.Graduations.Max();
            //得到相对于大容器的位置
            var viewPortWidth = Element.AxisPanel.ViewportRangeX.ViewMin;// Element.AxisXConverter.ScreenToViewport(0);//Element.AxisPanel.HorizontalOffset;
            //用段的模求出偏移量
            var xTranslateTransform = -viewPortWidth % (_sectionWidth);
            //Debug.Print(_sectionWidth.ToString());

            dc.PushClip(new RectangleGeometry(new Rect(renderSize)));
            dc.PushTransform(new TranslateTransform(xTranslateTransform, renderSize.Height - 14));//
            dc.DrawDrawing(_sectionsCache);
            dc.Pop();
            dc.Pop();

            MakeGuideLines(xTranslateTransform);

            var y = GetY();

            //draw base line
            if (Element.ShowBaseLine)
                dc.DrawGuideLineLine(CreateSelectedLinePen(), new Point(0, y), new Point(renderSize.Width, y));
        }

        //protected abstract void DrawSelectedBlockLine(DrawingContext dc);

        protected double GetY()
        {
            return Element.RenderSize.Height - Element.GetSelectedLineHeight();
        }

        private Pen _pen;
        private Pen CreatePen()
        {
            if (_pen == null)
            {

                var pen = new Pen { Brush = Element.Foreground, Thickness = 1 };
                pen.Freeze();
                _pen = pen;
            }
            return _pen;
        }

        private Pen _pen2;
        private Pen CreateSelectedLinePen()
        {
            if (_pen2 == null)
            {
                var pen = new Pen { Brush = Element.Foreground, Thickness = 1 };
                pen.Freeze();
                _pen2 = pen;
            }
            return _pen2;
        }



        //根据给定长度画出多个段
        private void DrawSections()
        {
            var sectionCount = (int)(_maxRenderWidth / _sectionWidth + 0.9);

            var section = DrawSingleSection(); // 被重複利用的第一段
            section.Transform = new TranslateTransform(-1, 0);//校正第一條線初始位置
            section.Freeze();

            var sectionGroup = new DrawingGroup();
            sectionGroup.Children.Add(section);
            for (var i = 1; i <= sectionCount; i++)
            {
                //使用Copy不必每个都去计算一遍
                var sectionClone = section.CloneCurrentValue();
                //因为怕在同一个位置，所以用TranslateTransform向后移动
                sectionClone.Transform = new TranslateTransform(_sectionWidth * i, 0);
                sectionClone.Freeze();

                sectionGroup.Children.Add(sectionClone);
            }
            sectionGroup.Freeze();
            _sectionsCache = sectionGroup;
        }

        //根据LineHeights的多少画一段，然后可以重复利用这段，不必每次都计算坐标，和到LineHeights拿值
        private DrawingGroup DrawSingleSection()
        {
            var sectionGroup = new DrawingGroup();
            var renderHight = Element.DesiredSize.Height;

            var selectedLineHeight = Element.GetSelectedLineHeight();

            var pen = CreatePen();

            // one graduations drawing
            for (var i = 0; i < Element.Graduations.Count; i++)
            {
                var graduation = Element.Graduations[i];
                if (graduation <= 0) continue;

                var x = i * Element.Interval;
                var line = new LineGeometry
                               {
                                   StartPoint = new Point(x, renderHight - graduation - selectedLineHeight),
                                   EndPoint = new Point(x, renderHight - selectedLineHeight)
                               };
                line.Freeze();

                var geometryDrawing = new GeometryDrawing(null, pen, line);
                geometryDrawing.Freeze();

                sectionGroup.Children.Add(geometryDrawing);
            }
            return sectionGroup;
        }

        //校正线
        private void MakeGuideLines(double xTranslateTransform)
        {
            var sectionWidth = _sectionWidth;
            var sectionCount = (int)(_maxRenderWidth / sectionWidth + 0.9); //多加了一個Section
            var renderHight = Element.DesiredSize.Height;

            var pen = CreatePen();
            var halfPenWidth = pen.Thickness / 2;

            Element.InitializeGuidelines();
            for (int j = 1; j <= sectionCount; j++)
            {
                for (int i = 0; i < Element.Graduations.Count; i++)
                {
                    var xLocation = i * Element.Interval + _sectionWidth * j + xTranslateTransform;
                    var yLocation1 = renderHight - Element.Graduations[i];
                    var yLocation2 = renderHight - 1;
                    Element.AddXGuidelines(xLocation + halfPenWidth);
                    Element.AddYGuidelines(yLocation1 + halfPenWidth);
                    Element.AddYGuidelines(yLocation2 + halfPenWidth);
                }
            }
            Element.AddXGuidelines(Element.RenderSize.Width);
            Element.AddYGuidelines(Element.RenderSize.Height);
        }
    }
}
