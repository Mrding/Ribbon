using System;

namespace Luna.Shifts.Domain
{
    /// <summary>
    /// Having holiday with paid
    /// </summary>
    public class TimeOff : Term, IOffWork, IIndependenceTerm
    {
        static readonly WorkHourType _payment;

        static TimeOff()
        {
            _payment = WorkHourType.Paid;
            //_defaultStyle = new TermStyle
            //                    {
            //                        Background = "LightGray",
            //                        Name = "TimeOff",
            //                        OnService = false,
            //                        Occupied = false
            //                    };
        }

        public TimeOff()
        {
            //_style = _defaultStyle;
            _background = "LightGray";
            _text = "TimeOff";
            _onService = false;
            _isNeedSeat = false;
        }

        public TimeOff(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 0;
            //_style = _defaultStyle;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return default(Type[]); }
        }

       public  DateTime HrDate { get; set; }

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

        public override bool OnService
        {
            get
            {
                return false;
            }
            set { _onService = false; }
        }

        public virtual bool? BelongToPrv { get; set; }

        public bool IgnoreAdherence { get { return true; } }

        //static TermStyle _defaultStyle;

        //public static void SetDefaultStyle(TermStyle value)
        //{
        //    _defaultStyle = value;
        //}

        //public override TermStyle Style
        //{
        //    get
        //    {
        //        if (_style == null)
        //            _style = _defaultStyle;
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
