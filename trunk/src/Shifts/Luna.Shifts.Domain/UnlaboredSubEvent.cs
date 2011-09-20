using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    /// <summary>
    /// 不支付薪水的事件
    /// PS: Unlabored意思是: 轻易完成的,未耕耘的
    /// </summary>
    public class UnlaboredSubEvent : Term, IDependencyTerm
    {
        static readonly List<Type> _canBeOverlapTypes;
        static readonly List<Type> _typesOfRelyOn;
        static readonly WorkHourType _payment;

        static UnlaboredSubEvent()
        {
            //any allowed types can cover on subevent but must in the range 
            _canBeOverlapTypes = new List<Type>(new Type[] { typeof(RegularSubEvent), typeof(AbsentEvent), typeof(OvertimeSubEvent) });
            _typesOfRelyOn = new List<Type>(new Type[] { typeof(Assignment), typeof(OvertimeAssignment) });
            _payment = WorkHourType.Unpaid;
            //_defaultStyle = new TermStyle
            //{
            //    Background = "Green",
            //    Name = "UnlaboredSubEvent",
            //    OnService = false,
            //    Occupied = true
            //};
        }

        //public static void SetTypeParamaters(bool isOcuppied)
        //{
        //    _isOcuppied = isOcuppied;
        //}

        public UnlaboredSubEvent()
        {
            //_style = _defaultStyle;
        }

        public UnlaboredSubEvent(DateTime start, TimeSpan length): base(start, length)
        {
            _level = 1;
            //_style = _defaultStyle;
            _background = "Green";
            _text = "UnlaboredSubEvent";
            _onService = false;
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
        //    return 20;
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
