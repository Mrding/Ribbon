using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Luna.Core.Extensions;
using System.Linq;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Infrastructure.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Presenters;

namespace Luna.Infrastructure.Presenters
{
    public class ConflictCalendarEvent
    {
        private CalendarEvent _entity;

        public ConflictCalendarEvent(CalendarEvent entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// new from excel row
        /// </summary>
        public CalendarEvent Entity { get { return _entity; } }

        /// <summary>
        /// exist(db,old) data
        /// </summary>
        public CalendarEvent ConflictWith { get; set; }

        public bool CanBeImport { get { return ConflictWith == null; } }
    }

    [PerRequest(typeof(ICalendarEventImportPresenter))]
    public class CalendarEventImportPresenter : OpenExcelFilePresenter<CalendarEvent>, ICalendarEventImportPresenter
    {
        private readonly ICalendarEventModel _calendarEventModel;
        private IList<CalendarEvent> _existEntities;
        private readonly Country _country;
        private IList<ConflictCalendarEvent> _loadedEntities;
        private bool _hasConflict;
        private string _successInfo;

        public CalendarEventImportPresenter(Country country, IOpenFileService openFileService, ICalendarEventModel calendarEventModel)
            : base(openFileService)
        {
            _country = country;
            _calendarEventModel = calendarEventModel;
            _nameOfSheets = new Dictionary<string, string[]>
                                {
                                    {"calendarevent", new[] {"EventName", "StartDate", "EndDate", "TimeZoneId"}}
                                };
            _existEntities = new List<CalendarEvent>();
            _loadedEntities = new List<ConflictCalendarEvent>(100);
            _hasConflict = false;
        }

        protected override void OnInitialize()
        {

            LoadedEntitiesView.GroupDescriptions.Add(new PropertyGroupDescription("CanBeImport"));
            base.OnInitialize();
        }




        public ICollectionView Errors
        {
            get { return CollectionViewSource.GetDefaultView(_exceptions); }
        }

        public ICollectionView LoadedEntitiesView
        {
            get { return CollectionViewSource.GetDefaultView(_loadedEntities); }
        }

        public bool HasConflict
        {
            get { return _hasConflict; }
        }

        public string SuccessInfo
        {
            get { return _successInfo; }
            set { _successInfo = value; NotifyOfPropertyChange(()=>SuccessInfo);}
        }

        public bool HasErrors
        {
            get
            {
                if (_exceptions != null)
                {
                    return _exceptions.Count != 0;
                }
                return false;
            }
        }

        protected override CalendarEvent ConvertToEntity(Func<string, bool, string> getField)
        {
            var timeZoneId = getField("TimeZoneId", false);
            var eventName = getField("EventName", false);
            var endDate = getField("EndDate", false);

            CalendarEvent newEntity;

            if (string.IsNullOrEmpty(timeZoneId)) // when timezoneid is null means Holiday
            {
                if (string.IsNullOrEmpty(eventName))
                    throw new NoNullAllowedException();

                newEntity = new Holiday { Country = _country.ToString(), EventName = eventName };
            }
            else
            {
                if (string.IsNullOrEmpty(endDate))
                    throw new NoNullAllowedException();

                newEntity = new DaylightSavingTime { EventName = "DST", TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId) };
            }

            newEntity.Start = Convert.ToDateTime(getField("StartDate", true));
            newEntity.End = string.IsNullOrEmpty(endDate) ? newEntity.Start.AddDays(1) : Convert.ToDateTime(endDate);
            return newEntity;
        }

        protected override void ReadingCompleted(IList<CalendarEvent> foundEntities)
        {
            _loadedEntities.Clear();
            _existEntities = _calendarEventModel.GetByCountry(_country);
            foreach (var item in foundEntities)
            {
                var getSameItem = _existEntities.FirstOrDefault(e => e.Equals(item));
                var loadedEntity = new ConflictCalendarEvent(item)
                                       {
                                           ConflictWith = getSameItem
                                       };
                if (!_hasConflict)
                _hasConflict = getSameItem != null;
                _loadedEntities.Add(loadedEntity);
            }
            LoadedEntitiesView.Refresh();
            NotifyOfPropertyChange(() => Errors);
            NotifyOfPropertyChange(() => HasErrors);
            NotifyOfPropertyChange(() => HasConflict);
        }

        public bool CanSave()
        {
            if (_loadedEntities == null || _loadedEntities.Count == 0)
                return false;

            var hasVaildRows = _loadedEntities.Any(o => o.ConflictWith == null);

            var overrid = _loadedEntities.Any(o => o.ConflictWith != null) && OverridExistData;

            return (_exceptions == null || _exceptions.Count == 0 || IgnoreInvalidExcelRows) && (overrid || hasVaildRows);
        }

        [Preview("CanSave")]
        [Dependencies("HasConflict", "IgnoreInvalidExcelRows", "OverridExistData")]
        public void Save()
        {
            var existEntity = _calendarEventModel.GetByCountry(_country);
            foreach (var entity in _loadedEntities)
            {
                if ((OverridExistData || IgnoreInvalidExcelRows) && entity.ConflictWith!=null)
                {
                    var sameEntity = existEntity.Where(e => e.Equals(entity.Entity)).FirstOrDefault();
                    _calendarEventModel.Delete(sameEntity);
                }
                _calendarEventModel.Save(entity.Entity);
            }
            SuccessInfo = "导入成功！";
        }
    }
}
