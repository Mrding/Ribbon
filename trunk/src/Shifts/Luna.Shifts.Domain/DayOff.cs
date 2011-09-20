using System;

namespace Luna.Shifts.Domain
{
    /// <summary>
    /// Having holiday without paid
    /// </summary>
    public class DayOff : Term, IOffWork, IIndependenceTerm
    {
        static readonly WorkHourType _payment;
        //static TermStyle _defaultStyle;


        static DayOff()
        {
            _payment = WorkHourType.Unpaid;
            //_defaultStyle = new TermStyle
            //                    {
            //                        Background = "White",
            //                        Name = "DayOff",
            //                        Occupied = false,
            //                        OnService = false
            //                    };
        }

        public DayOff()
        {
            //_style = _defaultStyle;
        }

        public DayOff(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 0;
            //_style = _defaultStyle;
            Background = "IndianRed";
            Text = "X";
            IsNeedSeat = false;
            OnService = false;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return default(Type[]); }
        }

        public DateTime HrDate { get; set; }

        public override bool OnService
        {
            get { return false; }
        }

        public override WorkHourType Payment
        {
            get { return _payment; }
        }

        internal override bool Independent { get { return true; } }

        //public override int GetHashCode()
        //{
        //    return 10;
        //}

        internal override bool CanNotOverlapWithAbsent
        {
            get { return true; }
        }

        public virtual bool? BelongToPrv { get; set; }

        public bool IgnoreAdherence { get { return true; } }

        //public static void SetDefaultStyle(TermStyle value)
        //{
        //    _defaultStyle = value;
        //}

        //public override TermStyle Style
        //{
        //    get
        //    {
        //        if (_style == null) _style = _defaultStyle;
        //        return base.Style;
        //    }
        //    internal set
        //    {
        //        //if (value != null && value.Type == this.GetType())
        //            _style = value;
        //    }
        //}
    }
}