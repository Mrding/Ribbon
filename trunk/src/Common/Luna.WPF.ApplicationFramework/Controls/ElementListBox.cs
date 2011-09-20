using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls
{

    /// <summary>
    /// 使用时不要直接放在Scrollviewer下面,需要放在一个容器里面
    /// </summary>
    public class ElementListBox : ElementItemsControl
    {


        private void ChildrenOperate(Action<int> addOrRemoveDesiredDisplayItems, Action childrenLayout, ref int childRenderedCount)
        {
            var desiredDisplayCount = GetDesiredDisplayCount();

            addOrRemoveDesiredDisplayItems(desiredDisplayCount);


            // 之前已measure25,当desiredCount=10，将remove掉15;等下次再desiredCount=33时，将不会measure，所以要childrenMeasureCount=10，以便下次在重新measure
            if (desiredDisplayCount < childRenderedCount)
            {
                //xchildrenSignCount = Children.Count;
                childRenderedCount = Children.Count;
            }
            else
            {
                // 已measure20次，界面调整至desiredCount=33时，只需再多measure13次
                while (childRenderedCount < desiredDisplayCount)
                {
                    //xsetChildrenSignCount(childrenSignCount);
                    if (childRenderedCount < Children.Count)
                        childrenLayout();
                    childRenderedCount++;
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _childrenArrangeCount = 0;

            var childSize = new Size(finalSize.Width, RowHeight);

            ChildrenOperate(RemoveVisiableChildren, () =>
                       {
                           Children[_childrenArrangeCount].Arrange(new Rect(new Point(0, RowHeight * _childrenArrangeCount), childSize));
                       }, ref _childrenArrangeCount);

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _childrenMeasureCount = 0;

            var childSize = new Size(availableSize.Width, RowHeight);

            ChildrenOperate(x =>
                {

                }, () => Children[_childrenMeasureCount].Measure(childSize), ref _childrenMeasureCount);

            availableSize.Height = ScrollViewer.ViewportRangeY.ViewMax;

            return availableSize;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (ItemsSource == null || ItemsSource.Count == 0 || ScrollViewer == null) return;

            var screenTopRowIndex = GetScreenTopRowIndex();
            var count = GetDesiredDisplayCount();

            if (ItemsSource.Count <= (screenTopRowIndex + count) && sizeInfo.PreviousSize.Height < sizeInfo.NewSize.Height)
            {
                var gap = sizeInfo.NewSize.Height - sizeInfo.PreviousSize.Height;
                if (gap < RowHeight)
                {
                    if (sizeInfo.PreviousSize.Height < RowHeight)
                        ScrollViewer.ScrollToEnd();
                }
                else
                {
                   //以下2行位查明意图
                   //xvar offset = gap / RowHeight * ScrollViewer.ExtentHeight / ItemsSource.Count;
                   //xScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - offset);
                    Refresh(false);
                }
            }
            else
                Refresh(false);

            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
