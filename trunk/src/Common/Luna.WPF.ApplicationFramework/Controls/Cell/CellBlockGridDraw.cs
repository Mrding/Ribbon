using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Interfaces;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Controls.Cell
{
    public class CellBlockGridDraw : BaseBlockGridDraw
    {
        private Pen _pen;
        private Dictionary<string, Tuple<FormattedText, double>> _formatedTextCaches;


        public CellBlockGridDraw()
        {
            _formatedTextCaches = new Dictionary<string, Luna.Core.Tuple<FormattedText, double>>(100);
            //_pen = Element.GetDashPen(Brushes.Gray,1, new DashStyle( new[]{ 1.0,2.0 }, 0 ));
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(CellBlockGridDraw), new FrameworkPropertyMetadata(Brushes.Transparent, (d, e) =>
                {
                    d.SaftyInvoke<CellBlockGridDraw>(o => { });
                }));


        public bool FixedRowCount
        {
            get { return (bool)GetValue(FixedRowCountProperty); }
            set { SetValue(FixedRowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FixedRowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FixedRowCountProperty =
            DependencyProperty.Register("FixedRowCount", typeof(bool), typeof(CellBlockGridDraw), new UIPropertyMetadata(false));

        protected override int GetTopRowIndex()
        {
            return FixedRowCount ? 0 : base.GetTopRowIndex();
        }

        protected override void RowRender(int index, double top, DrawingContext dc)
        {
            var viewRange = GetViewRange();
            var height = Element.Interval;

            Element.GetItems(index, viewRange).SaftyInvoke<IDateIndexer<ITerm>>(o =>
            {
                var day = viewRange.Start.Date;
                var end = viewRange.End.Date;
                while (day < end)
                {
                    var block = o[day];
                    if (block != null)
                    {
                        var foreground = Element.BlockConverter.GetForeground(block);

                        var rect = day.ToRect(Element.AxisXConverter, top, height);

                        var dateTerm = Element.ItemsSource[index].SaftyGetProperty<DateTerm, IDateIndexer<DateTerm>>(t => t[day]);
                        

                        if (dateTerm != null && Background != Brushes.Transparent && Background != null)
                        {
                            // dc.DrawGuidelineRect(null, _pen, rect.SetMargin(new Thickness(0, -1, 0, 1)));

                            dc.DrawRectangle( Element.BlockConverter.GetBackground(block) ?? Background, null, rect.SetMargin(new Thickness(1, 0, 0, 0.5))); //like dirty cell
                        }

                        if (dateTerm != null && !string.IsNullOrEmpty(dateTerm.Text))
                        {
                            dc.DrawCenterText(dateTerm.Text, rect, foreground);
                        }
                        else
                        {
                            var text = Element.BlockConverter.GetContentText(block);
                            if (!string.IsNullOrEmpty(text))
                            {
                                FormattedText formattedText;

                                if (!_formatedTextCaches.ContainsKey(text))
                                {
                                    formattedText = text.ToFormattedText(foreground);
                                    var charWidth = formattedText.Width / text.Length;
                                    if (formattedText.Width > rect.Width)
                                    {
                                        var maxChar = (int)(rect.Width / charWidth);
                                        formattedText = text.Substring(0, maxChar).ToFormattedText(foreground);
                                    }
                                    _formatedTextCaches[text] = new Tuple<FormattedText, double>(formattedText, ((rect.Height - formattedText.Baseline) / 2));
                                }
                                else
                                    formattedText = _formatedTextCaches[text].Item1;

                                var horizontailMargin = (rect.Width - formattedText.Width) / 2; //margin left & right

                                //Item1 = formatedText , Item2 = top margin
                                if(dateTerm==null)
                                    Element.BlockConverter.GetBackground(block).SaftyInvoke(
                                        b => dc.DrawRectangle(b, null, rect.SetMargin(new Thickness(1, 0, 0, 0.5))));

                                dc.DrawText(formattedText, new Point(rect.Left + horizontailMargin, rect.Top + _formatedTextCaches[text].Item2));
                                
                            }
                        }
                    }
                    day = day.AddDays(1);
                }
            });

            //Element.ItemsSource[index].SaftyInvoke<IDateIndexer<DateTerm>>(o =>
            //{
            //    var viewRange = GetViewRange();
            //    var day = viewRange.Start.Date;
            //    var end = viewRange.End.Date.AddDays(1);
            //    while (day < end)
            //    {
            //        var dateTerm = o[day];
            //        if (dateTerm != null)
            //        {
            //            var rect = dateTerm.Date.ToRect(Element.AxisXConverter, 0, Element.Interval);
            //            dc.DrawRectangle(Background, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));
            //            dc.DrawCenterText(dateTerm.Text, rect, Brushes.White);
            //        }

            //        day = day.AddDays(1);
            //    }
            //});

            //Element.ItemsSource[index].SaftyInvoke<IDateIndexer<DateTerm>>(o =>
            //{
            //    var viewRange = GetViewRange();
            //    var day = viewRange.Start.Date;
            //    var end = viewRange.End.Date.AddDays(1);
            //    while (day < end)
            //    {
            //        var dateTerm = o[day];
            //        if (dateTerm != null)
            //        {
            //            var rect = dateTerm.Date.ToRect(Element.AxisXConverter, 0, Element.Interval);
            //            dc.DrawRectangle(Background, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));

            //        }

            //        day = day.AddDays(1);
            //    }
            //});

            //foreach (var item in Element.GetItems(index, GetViewRange()))
            //{
            //    var block = item as ITerm;

            //    if (block == null)
            //        continue;

            //    if (OutOfVisualRange(block)) continue;
            //    //注意 HrDate

            //    var foreground = Element.BlockConverter.GetForeground(block);

            //    var date = Element.BlockConverter.GetStart(block);

            //    var rect = date.ToRect(Element.AxisXConverter, 0, Element.BlockConverter.GetHeight(block));

            //    var dateTerm = Element.ItemsSource[index].SaftyGetProperty<DateTerm, IDateIndexer<DateTerm>>(o => o[date]);

            //    if (dateTerm != null && Background != Brushes.Transparent && Background != null)
            //        dc.DrawRectangle(Background, null, rect.SetMargin(new Thickness(0, 0, 0, 1)));
            //    if (dateTerm != null && !string.IsNullOrEmpty(dateTerm.Text))
            //    {
            //        dc.DrawCenterText(dateTerm.Text, rect, foreground);
            //    }
            //    else
            //        dc.DrawCenterText(Element.BlockConverter.GetContentText(block), rect, foreground);

            //}
        }
    }
}
