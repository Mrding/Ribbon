using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public class RegularSubEvent : Term, IDependencyTerm
    {
        static readonly List<Type> _canBeOverlapTypes;
        static readonly List<Type> _typesOfRelyOn;
        static readonly WorkHourType _payment;
        //static bool _isOcuppied;

        static RegularSubEvent()
        {
            //any allowed types can cover on subevent but must in the range 
            _canBeOverlapTypes = new List<Type>(new Type[] { typeof(AbsentEvent) });//, typeof(UnlaboredSubEvent)
            _typesOfRelyOn = new List<Type>(new Type[] { typeof(Assignment), typeof(OvertimeAssignment), typeof(UnlaboredSubEvent) });
            _payment = WorkHourType.Paid;

            //_defaultStyle = new TermStyle
            //                    {

            //                        Background = "Green",
            //                        Name = "SubEvent",
            //                        OnService = true,
            //                        Occupied = true
            //                    };
        }

        //public static void SetTypeParamaters(bool isOcuppied)
        //{
        //    _isOcuppied = isOcuppied;
        //}

        public RegularSubEvent()
        {
            //_style = _defaultStyle;

        }

        public RegularSubEvent(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 1;
            //_style = _defaultStyle;
            _background = "Green";
            _text = "SubEvent";
            _onService = true;
            _isNeedSeat = true;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return _canBeOverlapTypes.ToArray(); }
        }

        public override bool NeedRelyOn(Term other)
        {
            return _typesOfRelyOn.Contains(other.GetType());
        }

        public override WorkHourType Payment
        {
            get { return _payment; }
        }

        //public override int GetHashCode()
        //{
        //    return 30;
        //}

        internal override bool CanNotOverlapWithAbsent
        {
            get { return false; }
        }

        //static TermStyle _defaultStyle;
        //public static void SetDefaultStyle(TermStyle value)
        //{
        //    _defaultStyle = value;
        //}

        //public override TermStyle Style
        //{
        //    get
        //    {
        //        if (base._style == null) _style = _defaultStyle;
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
