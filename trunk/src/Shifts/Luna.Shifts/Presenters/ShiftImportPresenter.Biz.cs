using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Common.Extensions;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Helpers;

namespace Luna.Shifts.Presenters
{
    public partial class ShiftImportPresenter
    {
        private int _passedRows;
        private int _unreachableRows;
        private int _hasArrangedRows;
        private int _duplicatedRows;
        private int _operationFailRows;

        public int OperationFailRows
        {
            get { return _operationFailRows; }
        }

        public int DuplicatedRows
        {
            get { return _duplicatedRows; }
        }

        public int HasArrangedRows
        {
            get { return _hasArrangedRows; }
        }

        public int UnreachableRows
        {
            get { return _unreachableRows; }
        }

        public int PassedRows
        {
            get { return _passedRows; }
        }

        private void Preparing()
        {
            _shiftDispatcherModel.Release();

            _bindableAgents.Clear();
            _importRange = null;
            _enquiryRange = null;
            _dates = new List<DateTerm>(30);
            _passedRows = 0;
            _unreachableRows = 0;
            _hasArrangedRows = 0;
            _duplicatedRows = 0;
            _operationFailRows = 0;
        }

        private void Reading()
        {
            //run
            InvalidExcelFileFormat = !ColumnNameMapping.ReadAssignmentExcelSheets(FilePath, date => _dates.Add(new DateTerm(date, _dates.Count == 0 ? "{0:M/d}" : "{0:d }")
            {
                IsHoliday = date.IsHoliday(Country.Local),
                IsDaylightSaving = date.IsDaylightSaving(TimeZoneInfo.Local)
            }), rows =>
                    {
                        Status = "Running";
                        ProcessInfo = "Loading assignment types";
                        Processing = true;
                        _assignmentTypes = _shiftDispatcherModel.GetAllShiftTypes().ToDictionary(o => o.Name);

                        Processing = false;
                        ProcessInfo = null;
                        Process = 0;
                        TotalProcess = rows;
                        _passedRows = rows;
                        _calendarEventModel.LoadGlobalCalendar(ImportRange.Start, ImportRange.End);
                    }, ExamingItemArray, DuplicationRowFound, DuplicationCheck);

            //completed
            Status = "Ready";
            _imported = false;

            NotifyOfPropertyChange(() => ImportRange);
            NotifyOfPropertyChange(() => Dates);
            NotifyOfPropertyChange(() => PassedRows);
            NotifyOfPropertyChange(() => UnreachableRows);
            NotifyOfPropertyChange(() => HasArrangedRows);
            NotifyOfPropertyChange(() => OperationFailRows);

            this.QuietlyReload(ref _bindableAgents, "BindableAgents");
        }

        private Core.Tuple<object, int[]> DuplicationCheck(int excelRowIndex, string queryNames)
        {
            object employee = null;
            var found = _bindableAgents.Where(o => o.SaftyGetProperty<bool, PlanningAgent>(d =>
            {
                if (d.Index != excelRowIndex) return false;
                employee = d.Profile;
                return true;
            }) || o.SaftyGetProperty<bool, DummyAgent>(d =>
            {
                if (d.ToString() != queryNames) return false;

                if (d.Profile != null)
                    employee = d.Profile;

                return true;
            }))
                 .Select(o => o.SaftyGetProperty<int, IIndexable>(d => d.Index))
                 .ToArray();
            return new Tuple<object, int[]>(employee, found);
        }

        private void DuplicationRowFound(string queryNames, int excelRowIndex, IList<int> duplicationWith, object employee)
        {
            _bindableAgents.Add(new DummyAgent(queryNames, excelRowIndex, duplicationWith).Self(o => o.Profile = employee).As<IEnumerable>()); // 注意IEnumerable类型转换, 不可修改
            Process += 1;
            _passedRows--;
            _unreachableRows += employee == null ? 1 : 0;
            _duplicatedRows++;
        }

        private void ExamingItemArray(object[] itemArray, string[] queryParams, int rowIndex, int shiftStartIndex)
        {

            var agent = _shiftDispatcherModel.GetPlanningAgent(queryParams, EnquiryRange);

            var validRangeLength = _dates.Count + shiftStartIndex; // 用连续日期长度来决定读取班表的格子数量

            var step = 1.0 / validRangeLength;
            var completeValue = Process + 1;

            var dirty = false; //用於BuildOnlines
            for (var i = shiftStartIndex; i < validRangeLength; i++)
            {
                if (agent == null) break;

                var dateIndex = i - shiftStartIndex;

                if (agent[dateIndex].SaftyGetProperty(o => o.IsNot<UnknowAssignment>())) // 已经有班 brak
                {
                    //xagent.OperationFail = true;
                    agent.Tag = "HasArranged";
                    _hasArrangedRows++;
                    break;
                }
                //Thread.Sleep(200);
                Process += step;

                if (itemArray[i].IsNullValue()) continue;

                string[] range;
                var assignmentTypeValue = ResolveShiftCellValue(itemArray[i].ToString(), out range); // 解析班表字串

                var hrDate = _dates[dateIndex].Date;

                if (!_assignmentTypes.ContainsKey(assignmentTypeValue)) //无效班型
                {
                    agent[hrDate] = new CellTerm(hrDate, assignmentTypeValue) { ErrorInfo = "NotExist"};
                    //xagent.OperationFail = true;
                    agent.Tag = "InvalidShiftCell";
                    continue; // 不结束循环继续往下找
                }

                var newAssignment = default(AssignmentBase);
                var assignmentType = _assignmentTypes[assignmentTypeValue];

                if (range == null) // 無自定義時間
                    newAssignment = _shiftDispatcherModel.CreateAssignmentWithSenser(hrDate, assignmentType);
                else
                {
                    var start = StringToDateTime(range[0], assignmentType.TimeRange.StartValue, hrDate);
                    var end = StringToDateTime(range[1], assignmentType.TimeRange.EndValue, hrDate);

                    if (end < start) // 跨天
                        end = end.AddDays(1);

                    Term.NewAssignment(start, end, assignmentType).SaftyInvoke<AssignmentBase>(o =>
                    {
                        newAssignment = o;
                        o.HrDate = hrDate;
                    });
                }

                if (newAssignment != null)
                {
                    agent.Schedule.Create(newAssignment,
                         (t, success) =>
                         {
                             if (success)
                             {
                                 dirty = true;
                                 t.SaftyInvoke<IAssignment>(a => agent.Schedule.ArrangeSubEvent(a, assignmentType.GetSubEventInsertRules(), null));
                             }
                             else // 安插班表失败
                             {
                                 agent.OperationFail = true;
                                 agent[hrDate] = new CellTerm(hrDate, assignmentTypeValue) { ErrorInfo = "Overlap" };
                             }
                         }, true);
                }
            }

            if (agent != null)
            {
                agent.Index = rowIndex;
                if (dirty)
                    agent.BuildOnlines();
                _bindableAgents.Add(agent.As<IEnumerable>()); // 注意IEnumerable类型转换, 不可修改
                if(agent.OperationFail == true)
                    _operationFailRows++;
            }
            else
                _bindableAgents.Add(new DummyAgent(queryParams, rowIndex).As<IEnumerable>()); // 注意IEnumerable类型转换, 不可修改

            _passedRows -= agent == null || agent.OperationFail == true || agent.Tag != null ? 1 : 0;
            _unreachableRows += agent == null ? 1 : 0;
            
            Process = completeValue;
        }


        public bool BindableAgentsAllRight()
        {
            return !_imported && _bindableAgents != null && _bindableAgents.Count != 0 && _totalProcess == _passedRows;
        }

        [Preview("BindableAgentsAllRight")]
        [Dependencies("BindableAgents")]
        [SuperRescue("SubmitChangesFail", "Shifts_ShiftDispatcher_SubmitChangesFail", "Save failed", false)]
        public void Improt()
        {
            _imported = true;
            NotifyOfPropertyChange(() => BindableAgents);

            Processing = true;
            ProcessInfo = "Saving";

            _shiftDispatcherModel.UpdateSchedule(true);
            _shiftDispatcherModel.Release();

            ProcessInfo = "Success";
            Processing = false;
        }

        private bool _imported;

        public ITerm ImportRange
        {
            get
            {
                if (_importRange == null && 0 < _dates.Count)
                    _importRange = new DateRange(_dates[0].Date, _dates[_dates.Count - 1].Date.AddDays(1)); // end 日期定义校正补一天
                return _importRange;
            }
        }

        public ITerm EnquiryRange
        {
            get
            {
                if (ImportRange != null && 0 < _dates.Count)
                    _enquiryRange = new DateRange(ImportRange.Start.AddDays(Global.HeadDayAmount), ImportRange.End);

                return _enquiryRange;
            }
        }

        public virtual void SubmitChangesFail(Exception ex)
        {
            ProcessInfo = "Fail";
            Processing = false;
        }

        #region String Reslove

        private static DateTime StringToDateTime(string timeValue, int rangeValue, DateTime date)
        {
            return string.IsNullOrEmpty(timeValue)
                                    ? date.AddMinutes(rangeValue)
                                    : Convert.ToDateTime(string.Format("{0:yyyy/MM/dd} {1}", date, timeValue));
        }

        private static string ResolveShiftCellValue(string cellContent, out string[] rangeValue)
        {
            var charIndex = cellContent.IndexOf(';');

            var endWithChar = cellContent.Length - 1 == charIndex;

            if (charIndex == -1 || endWithChar) //A8;
            {
                rangeValue = null;
                return cellContent.TrimEnd(';'); // A8
            }

            // 以下为自定义时间
            var strs = cellContent.Split(';'); // A8;08:00 自定义时间

            var dateTimeValue = strs[1]; // 08:00

            if (!dateTimeValue.Contains('~')) // 自定义开始时间 08:00 没有 '~' 符号
                rangeValue = new[] { dateTimeValue, string.Empty };
            else
            {
                if (dateTimeValue.StartsWith("~")) // 自定义结束时间 A8;~20:00
                    rangeValue = new[] { string.Empty, dateTimeValue.TrimStart('~') }; // 20:00
                else // 自定义开始结束时间 A8;08:00~20:00
                    rangeValue = dateTimeValue.Split('~');
            }
            return strs[0];// A8
        }
        #endregion
    }
}
