using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Luna.Common;
using Luna.Common.Domain;
using Luna.Core.Extensions;
using Nh = Iesi.Collections.Generic;

namespace Luna.Shifts.Domain
{
     [DebuggerDisplay("{Seat}")]
    public partial class SeatBox : Entity, ITermContainer, ISelectable
    {
        protected SeatBox() { }

        public SeatBox(Seat seat)
        {
            _seat = seat;
        }

        public virtual bool? OperationFail { get; set; }

        private Seat _seat;

        public virtual Seat Seat
        {
            get { return _seat; }
            protected set { _seat = value; }
        }

        private Nh.ISet<Occupation> _occupationSet;

        private Nh.ISet<Occupation> _mixed;

        protected virtual Nh.ISet<Occupation> OccupationSet
        {
            get { return _occupationSet; }
            set { _occupationSet = value; }
        }

        public virtual IEnumerable<Occupation> Occupations
        {
            get
            {
                //if (_mixed == null)
                //    RetainSeatEventsOnly();
                return _mixed;
            }
        }

        public virtual Int64 Version { get; set; }

        public override string GetUniqueKey()
        {
            return Seat.ExtNo;
        }

        public virtual bool UpdateOccupation<T>(T occupation) where T : Occupation
        {
            var target = _mixed.FirstOrDefault(o => o.Id == occupation.Id) as SeatEvent;
            if (target != null)
            {
                target.Start = occupation.Start;
                target.End = occupation.End;

                if (occupation is SeatEvent)
                {
                    target.Category = (occupation as SeatEvent).Category;
                    target.Description = (occupation as SeatEvent).Description;
                }
                return true;
            }
            return false;
        }

        public virtual bool RemoveOccupation(Occupation occupation)
        {
            _fetchTermSetCache = null;
            if (occupation is SeatEvent)
            {
                if (!_occupationSet.Remove(occupation))
                    return false;
            }

            return _mixed.Remove(occupation);
        }

        public virtual void RetainSeatEventsThenAddBackOthers()
        {
            _fetchTermSetCache = null;
            if (_mixed != null)
            {
                var occupations = _mixed.OfType<SeatArrangement>().ToArray();
                if (occupations.Length > 0)
                {
                    _mixed = new TermSet<Occupation>(_occupationSet);
                    foreach (var item in occupations)
                        _mixed.Add(item);
                    return;
                }
            }

            _mixed = new TermSet<Occupation>(_occupationSet);
        }

        public virtual void Initial()
        {
            _mixed = new TermSet<Occupation>(_occupationSet);
        }

        public virtual void AddOccupations(Occupation[] occupations)
        {
            _fetchTermSetCache = null;
            foreach (SeatEvent t in occupations.OfType<SeatEvent>())
                AddOccupation(t);

            foreach (var item in occupations.OfType<SeatArrangement>())
                _mixed.Add(item);
        }

        public virtual bool AddOccupation(Occupation occupation)
        { 
            _fetchTermSetCache = null;
            if (occupation is SeatEvent)
            {
                if (!_occupationSet.Add(occupation))
                    return false;

                //if(withInitilOccupations)
                //    RetainSeatEventsOnly();
            }
           
            return _mixed.Add(occupation);
        }

        public virtual IEnumerator<Occupation> GetEnumerator()
        {
            if (_fetchTermSetCache == null)
                _fetchTermSetCache = _mixed.OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
            return _fetchTermSetCache.GetEnumerator();
        }
        private List<Occupation> _fetchTermSetCache;

        //todo don't forget this
        //IEnumerable ITermContainer.GetAllTerms()
        //{
        //    return Occupations;
        //}

        public virtual IEnumerable Fetch(DateTime start, DateTime end)
        {
            _fetchTermSetCache = _mixed.OrderBy(o => o.Start).ThenBy(o => o.Level).ToList();
            return _fetchTermSetCache;
        }

        public virtual IList Result { get { return _fetchTermSetCache ?? _mixed.ToList(); } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool? ISelectable.IsSelected
        {
            get
            {
                bool? isSelected = false;
                Seat.SaftyInvoke<ISelectable>(o => { isSelected = o.IsSelected; });

                return isSelected;
            }
            set
            {
                Seat.SaftyInvoke<ISelectable>(o => { o.IsSelected = value; });
            }
        }


    }
}
