using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using Caliburn.Core.Invocation;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Presenters
{
    public class DayItemModel : List<CalendarEvent>, INotifyPropertyChanged
    {
        public DayItemModel(DateTime date)
        {
            Date = date;
        }

        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(this).Self(v =>
                                                               {
                                                                   v.Refresh();
                                                                   if (v.CurrentItem == null)
                                                                       v.MoveCurrentToFirst();
                                                               });
            NotifyOfPropertyChange(() => IsHolidayWeekend);
            NotifyOfPropertyChange(() => IsDaylightSavingDay);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            Execute.OnUIThread(() => RaisePropertyChangedEventImmediately(propertyName));
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            NotifyOfPropertyChange(propertyExpression.GetMemberInfo().Name);
        }

        public virtual void RaisePropertyChangedEventImmediately(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime Date { get; private set; }

        public bool IsHolidayWeekend { get { return this.Any(c => c.IsWeekendHoliday); } }

        public bool IsDaylightSavingDay { get { return this.Any(c => c.Is<DaylightSavingTime>()); } }

        public bool IsHoliday { get { return this.Any(c => c.Is<Holiday>()); } }
    }
}