using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Caliburn.Core.Metadata;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain.Model;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IShiftImportPresenter))]
    public partial class ShiftImportPresenter : DefaultPresenter, IShiftImportPresenter
    {
        private readonly IShiftDispatcherModel _shiftDispatcherModel;
        private readonly ICalendarEventModel _calendarEventModel;
        private readonly IOpenFileService _openFileService;
        private IList<IEnumerable> _bindableAgents;
        private BlockToCellConverter _assignmentCellConverter;
        private string _filePath;
        private ITerm _importRange;
        private IList<DateTerm> _dates;
        private string _status;
        private ITerm _enquiryRange;
        private IDictionary<string, AssignmentType> _assignmentTypes;
        private Thread _importingThread;

        public ShiftImportPresenter(IShiftDispatcherModel shiftDispatcherModel, ICalendarEventModel calendarEventModel, IOpenFileService openFileService)
        {
            _shiftDispatcherModel = shiftDispatcherModel;
            _calendarEventModel = calendarEventModel;
            _openFileService = openFileService;
            _bindableAgents = new List<IEnumerable>();
            _openFileService.Filter = "Excel File|*.xls";
            _assignmentCellConverter = new BlockToCellConverter(Brushes.OrangeRed, "#FF77BB44".ToBrush(1), Brushes.DarkGray).ShowDayOffText(false);
        }

        public void OpenFile()
        {
            if (_importingThread.If(t => t.IsAlive) == true)
            {
                _importingThread.Abort();
                Status = null;
                return;
            }

            if (_openFileService.ShowDialog(this) != true)
                return;


            FilePath = _openFileService.FileName;

            Preparing();

            _importingThread = new Thread(Reading) { IsBackground = true }.Self(t => t.Start());

        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
            }
        }

        private string _processInfo;
        public string ProcessInfo
        {
            get { return _processInfo; }
            set { _processInfo = value; NotifyOfPropertyChange(() => ProcessInfo); }
        }

        private double _process;
        public double Process
        {
            get { return _process; }
            set { _process = value; NotifyOfPropertyChange(() => Process); }
        }

        private int _totalProcess = int.MaxValue;
        public int TotalProcess
        {
            get { return _totalProcess; }
            set { _totalProcess = value; NotifyOfPropertyChange(() => TotalProcess); }
        }

        private bool _processing;
        public bool Processing
        {
            get { return _processing; }
            set { _processing = value; NotifyOfPropertyChange(() => Processing); }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (value == null)
                    ProcessInfo = null;
                _status = value; NotifyOfPropertyChange(() => Status);
            }
        }

        public IList<DateTerm> Dates
        {
            get { return _dates; }
        }

        public IList<IEnumerable> BindableAgents
        {
            get { return _bindableAgents; }
            set
            {
                if (value == null) return;
                _bindableAgents = value;
            }
        }

        private bool _showExistShift = true;
        public bool ShowExistShift
        {
            get { return _showExistShift; }
            set
            {
                _showExistShift = !_showExistShift;
                _assignmentCellConverter.Foreground = _showExistShift ? Brushes.Gray : Brushes.Transparent;
                NotifyOfPropertyChange(() => BindableAgents);
            }
        }

        private bool _invalidExcelFileFormat;
        public bool InvalidExcelFileFormat
        {
            get { return _invalidExcelFileFormat; }
            set
            {
                _invalidExcelFileFormat = value;
                NotifyOfPropertyChange(() => InvalidExcelFileFormat);
            }
        }

        public IBlockConverter AssignmentCellConverter
        {
            get { return _assignmentCellConverter; }
        }
    }
}
