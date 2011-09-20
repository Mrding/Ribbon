using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Common.Interfaces;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework.Controls;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftComposerPresenter
    {
        private static void MarkAsChanged(IDateIndexer<DateTerm> indexer, DateTime date)
        {
            //用於標示被修改過的cell
            //xif (list.Any(o => o.Equals(value))) return;
            //xlist.Add(value);
            indexer[date] = new DateTerm(date, string.Empty);
        }

        private void ResetAlterDateRange()
        {
            _alterDateRange = new[] { DateTime.MaxValue, DateTime.MinValue };
        }

        private void RetrieveSelectionRange(IList itemsSource)
        {
            var timeRange = SelectionTimeRange;
            var dataRowRange = _selectionDataRowRange;

            var columnsCount = timeRange.GetLength().Days + 1;
            _shiftPainterTemplate = new List<string[]>(dataRowRange[1] - dataRowRange[0] + 1);
            for (var i = dataRowRange[0]; i <= dataRowRange[1]; i++)
            {
                var rootItem = itemsSource[i].As<IAgent>();
                var row = new string[columnsCount];

                rootItem.SaftyInvoke<IIntIndexer>(root =>
                {
                    var day = timeRange.Start.Date;
                    var end = timeRange.End.Date;

                    while (day < end)
                    {
                        var d = day.Subtract(timeRange.Start.Date).Days; // 未来可能会用 HRDate

                        var item = root.GetItem(day.IndexOf(_schedule.Start)) as IAssignment;
                        if (item != null)
                            row[d] = item.NativeName;

                        day = day.AddDays(1);
                    }
                });

                //// 以下搜索不支援HRDate, 将来必须修正
                //foreach (var item in from IAssignment item in rootItem
                //                     let dateKey = item.SaftyGetHrDate()
                //                     where timeRange.Start.Date <= dateKey && dateKey < timeRange.End.Date
                //                     select item)
                //{
                //    var d = item.SaftyGetHrDate().Subtract(timeRange.Start.Date).Days; // 未来可能会用 HRDate
                //    row[d] = item.NativeName;
                //}
                _shiftPainterTemplate.Add(row);
            }
        }



        private void CellTraversal<TValue, TRootItem>(object layer, object newValue, Action<TRootItem> rootItemPreparing, Action<TRootItem> rootItemCellsValueChanged,
            IList itemsSource, System.Func<TValue, TRootItem, ITerm, DateTime?> func, Action callback, CellEditRoutedEventArgs e) where TRootItem : IEnumerable
        {
            if (layer == null) return;

            var timeRange = new Core.Reflector(layer).Property<TimeRange>("TimeRange");
            var dataRowRange = new Core.Reflector(layer).Property<int[]>("DataRowRange");

            CellTraversal(timeRange, dataRowRange, newValue, rootItemPreparing, rootItemCellsValueChanged, itemsSource, (value, item) => Equals(value, item), func, callback, e);
        }

        private void CellTraversal<TValue, TRootItem>(TimeRange timeRange, int[] dataRowRange, object newValue, Action<TRootItem> rootItemPreparing, Action<TRootItem> rootItemCellsValueChanged,
            IList itemsSource, System.Func<TValue, ITerm, bool> eq, System.Func<TValue, TRootItem, ITerm, DateTime?> func, Action callback,
            CellEditRoutedEventArgs e)
            where TRootItem : IEnumerable
        {
            if (ReferenceEquals(newValue, default(TValue))) return;

            var value = newValue.As<TValue>();
            //xvar timeRange = new Core.Reflector(layer).Property<TimeRange>("TimeRange");
            //xvar dataRowRange = new Core.Reflector(layer).Property<int[]>("DataRowRange");

            var dirty = false;

            // TODO:以下搜索不支援HRDate, 将来必须修正
            for (var i = dataRowRange[0]; i <= dataRowRange[1]; i++)
            {
                var anyChanged = false;
                var rootItem = itemsSource[i].As<TRootItem>();
                rootItemPreparing(rootItem);


                rootItem.SaftyInvoke<IIntIndexer>(root =>
                {
                    var day = timeRange.Start.Date;
                    var end = timeRange.End.Date;

                    while (day < end)
                    {
                        var item = root.GetItem(day.IndexOf(_schedule.Start)) as ITerm;
                        var effectDate = func(value, rootItem, item);
                        if (effectDate != null)
                        {
                            rootItem.SaftyInvoke<IDateIndexer<DateTerm>>(r =>
                            {
                                r[effectDate.Value] = new DateTerm(effectDate.Value, string.Empty);
                            });

                            //xMarkAsChanged(changedCell, new Core.Tuple<int, DateTerm>(i, new DateTerm(effectDate.Value,string.Empty)));
                            anyChanged = true;
                        }
                        day = day.AddDays(1);
                    }
                });

                if (anyChanged)
                {
                    rootItemCellsValueChanged(rootItem);
                    dirty = true;
                }
            }
            if (dirty)
            {

                e.SaftyInvoke(o => { o.HasChanged = true; });

                if (timeRange.Start.Date < _alterDateRange[0])
                    _alterDateRange[0] = timeRange.Start.Date;

                if (_alterDateRange[1] < timeRange.End.Date)
                    _alterDateRange[1] = timeRange.End.Date;

                callback();
            }

        }

        private void CellTraversal<TRootItem>(Action<TRootItem> rootItemPreparing, Action<TRootItem> rootItemCellsValueChanged, System.Func<TRootItem, int, string, int, bool> func,
            IList itemsSource, IList<string[]> templateList, Action callback)
        {
            var dirty = false;

            var initalIndexOfData = _dateIndexer[SelectionTimeRange.Start.Date];

            for (var y = 0; y < templateList.Count; y++)
            {
                var anyChanged = false;
                var dataRowInex = _selectionDataRowRange[0] + y;

                if (itemsSource.Count <= dataRowInex) break;

                var agent = itemsSource[dataRowInex].As<TRootItem>();
                rootItemPreparing(agent);
                for (var x = 0; x < templateList[y].Length; x++)
                {
                    var templateItem = templateList[y][x];
                    var indexOfDate = initalIndexOfData + x;

                    if (!func(agent, dataRowInex, templateItem, indexOfDate)) continue;

                    anyChanged = true;
                    var alterDate = SelectionTimeRange.Start.Date.AddDays(x);
                    if (_alterDateRange[1] < alterDate)
                        _alterDateRange[1] = alterDate;
                }

                if (anyChanged)
                {
                    rootItemCellsValueChanged(agent);
                    dirty = true;
                }
            }

            if (dirty)
            {
                if (SelectionTimeRange.Start.Date < _alterDateRange[0])
                    _alterDateRange[0] = SelectionTimeRange.Start.Date;

                callback();
            }
        }

    }


}
