using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Graphics;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class BlockTimeDisplayLayerDraw : ElementDraw<BlockGridLayerBase>
    {
        private readonly BitmapEffect _effect;

        public BlockTimeDisplayLayerDraw()
        {
            _effect = new OuterGlowBitmapEffect { GlowColor = Colors.White, GlowSize = 20, Opacity = 1 };
            _effect.Freeze();
        }

        public override bool CanDraw()
        {
            if (!Element.IsMouseDragging() || Element.Visibility != Visibility.Visible)
                return false;
            return base.CanDraw();
        }

        protected override void InternalDraw(DrawingContext drawingContext)
        {
            var axisXConverter = Element.AxisXConverter;

            var left = Math.Max(0,axisXConverter.DataToScreen(Element.DropedPlacement.Start)); //控制文字显示位置

            var text = string.Format("{0:HH:mm} - {1:HH:mm}", Element.DropedPlacement.Start,
                                        Element.DropedPlacement.End);

            drawingContext.PushEffect(_effect, null);

            text.ToFormattedText(Brushes.Black).Self(fTxt =>
                                                         {
                                                             var topMargin = 0d;

                                                             switch (TextVerticalAlignment)
                                                             {
                                                                 case VerticalAlignment.Bottom:
                                                                     topMargin = Element.GetPointOutY() + (Element.Interval - fTxt.Height / 2);
                                                                     break;
                                                                 case VerticalAlignment.Top:
                                                                     topMargin = -30;
                                                                     break;
                                                                 case VerticalAlignment.Center:
                                                                     topMargin = (Element.BlockConverter.GetHeight(Element.PointOutBlock) - fTxt.Height) / 2; // 0;//Element.Interval - t.Height / 2;
                                                                     topMargin += Element.GetPointOutY();
                                                                     break;
                                                             }
                                                                 
                                                             var leftMargin = 5;
                                                             drawingContext.DrawText(fTxt, new Point(left + leftMargin, topMargin));
                                                         }); 

            drawingContext.Pop();
        }



        public VerticalAlignment TextVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(TextVerticalAlignmentProperty); }
            set { SetValue(TextVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextVerticalAlignmentProperty =
            DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(BlockTimeDisplayLayerDraw), new UIPropertyMetadata(VerticalAlignment.Center));

        
        
    }
}