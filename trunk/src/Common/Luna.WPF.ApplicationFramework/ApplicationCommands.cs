using ActiproSoftware.Windows.Controls.Ribbon.Input;
using System.Windows.Input;
using ActiproSoftware.Windows.Controls.Ribbon;

namespace Luna.WPF.ApplicationFramework
{
    public class ApplicationCommands
    {

        private static RibbonCommand _setDayOff;
        public static RoutedCommand SetDayOff
        {
            get { return _setDayOff ?? (_setDayOff = new RibbonCommand("SetDayOff", typeof(Ribbon), "DayOff")); }
        }

        private static RibbonCommand _lockTerm;

        public static RoutedCommand LockTerm
        {
            get
            {
                return _lockTerm ?? (_lockTerm = new RibbonCommand("LockTermButton", typeof(Ribbon),
                                                                   "Lock"));
            }
        }

        private static RibbonCommand _sortByTermName;

        public static RoutedCommand SortByTermName
        {
            get { return _sortByTermName ?? (_sortByTermName = new RibbonCommand("SortByTermNameButton", typeof(Ribbon))); }
        }

        private static RibbonCommand _sortByAgentName;

        public static RoutedCommand SortByAgentName
        {
            get { return _sortByAgentName ?? (_sortByAgentName = new RibbonCommand("SortByAgentName", typeof(Ribbon))); }
        }

        private static RibbonCommand _sortByTermStart;

        public static RoutedCommand SortByTermStart
        {
            get { return _sortByTermStart ?? (_sortByTermStart = new RibbonCommand("SortByTermStart", typeof(Ribbon))); }
        }


        private static RibbonCommand _showTermName;

        public static RoutedCommand ShowTermName
        {
            get { return _showTermName ?? (_showTermName = new RibbonCommand("ShowOrHideTermName", typeof(Ribbon))); }
        }

        private static RibbonCommand _addingEvent;

        public static RoutedCommand AddingEvent
        {
            get { return _addingEvent ?? (_addingEvent = new RibbonCommand("AddingEvent", typeof(Ribbon))); }
        }

        private static RibbonCommand _adherenceMonitoring;

        public static RoutedCommand AdherenceMonitoring
        {
            get
            {
                return _adherenceMonitoring ??
                       (_adherenceMonitoring = new RibbonCommand("AdherenceMonitoring", typeof(Ribbon)));
            }
        }
        private static RibbonCommand _adherenceStatistic;

        public static RoutedCommand AdherenceStatistic
        {
            get
            {
                return _adherenceStatistic ??
                       (_adherenceStatistic = new RibbonCommand("AdherenceStatistic", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _rescheduleSeat;

        public static RoutedCommand RescheduleSeat
        {
            get
            {
                return _rescheduleSeat ??
                       (_rescheduleSeat = new RibbonCommand("RescheduleSeat", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _cancelSeat;

        public static RoutedCommand CancelSeat
        {
            get
            {
                return _cancelSeat ??
                       (_cancelSeat = new RibbonCommand("CancelSeat", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _assignSeat;

        public static RoutedCommand AssignSeat
        {
            get
            {
                return _assignSeat ??
                       (_assignSeat = new RibbonCommand("AssignSeat", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _seatDispatcher;

        public static RoutedCommand SeatDispatcher
        {
            get
            {
                return _seatDispatcher ??
                       (_seatDispatcher = new RibbonCommand("SeatDispatcher", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _openStaffingStatistic;
        public static RoutedCommand OpenStaffingStatistic
        {
            get
            {
                return _openStaffingStatistic ??
                       (_openStaffingStatistic = new RibbonCommand("OpenStaffingStatistic", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _openCompositiveServiceQueue;
        public static RibbonCommand OpenCompositiveServiceQueue
        {
            get
            {
                return _openCompositiveServiceQueue ??
                    (_openCompositiveServiceQueue = new RibbonCommand("OpenCompositiveServiceQueue", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _showEstimateShift;
        public static RoutedCommand ShowEstimateShift
        {
            get
            {
                return _showEstimateShift ??
                       (_showEstimateShift = new RibbonCommand("ShowEstimateShift", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _estimateShift;
        public static RoutedCommand EstimateShift
        {
            get
            {
                return _estimateShift ??
                       (_estimateShift = new RibbonCommand("EstimateShift", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _switchView;
        public static RoutedCommand SwitchView
        {
            get
            {
                return _switchView ??
                       (_switchView = new RibbonCommand("SwitchView", typeof(Ribbon)));
            }
        }


        private static RibbonCommand _exportSvcLevelData;
        public static RoutedCommand ExportSvcLevelData
        {
            get
            {
                return _exportSvcLevelData ??
                       (_exportSvcLevelData = new RibbonCommand("ExportSvcLevelData", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _openAgentFinder;
        public static RoutedCommand OpenAgentFinder
        {
            get
            {
                return _openAgentFinder ??
                       (_openAgentFinder = new RibbonCommand("OpenAgentFinder", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _shiftPainter;
        public static RoutedCommand ShiftPainter
        {
            get
            {
                return _shiftPainter ??
                       (_shiftPainter = new RibbonCommand("ShiftPainter", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _openTermStyleManager;
        public static RoutedCommand OpenTermStyleManager
        {
            get
            {
                return _openTermStyleManager ??
                       (_openTermStyleManager = new RibbonCommand("OpenTermStyleManager", typeof(Ribbon)));
            }
        }
        private static RibbonCommand _openCalendarEvent;
        public static RoutedCommand OpenCalendarEvent
        {
            get
            {
                return _openCalendarEvent ??
                       (_openCalendarEvent = new RibbonCommand("OpenCalendarEvent", typeof(Ribbon)));
            }
        }

        private static RibbonCommand _openShiftImport;
        public static RoutedCommand OpenShiftImport
        {
            get
            {
                return _openShiftImport ??
                    (_openShiftImport = new RibbonCommand("OpenShiftImport", typeof(Ribbon)));
            }
        }
    }
}
