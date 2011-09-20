using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public class Shrink : Term ,IDependencyTerm{

        static readonly List<Type> _typesOfRelyOn;
        static readonly WorkHourType _payment;

        static Shrink()
        {
            _typesOfRelyOn = new List<Type>(new Type[] { typeof(Assignment) });
            _payment = WorkHourType.Shrink;
            //_defaultStyle = new TermStyle
            //{
            //    Background = "LightGray",
            //    Name = "Shrink",
            //    Occupied = false,
            //    OnService = false
            //};
        }

        public Shrink()
        {
            //_style = _defaultStyle;
        }

        public Shrink(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 1;
            //_style = _defaultStyle;
            _background = "LightGray";
            _text = "Shrink";
            _isNeedSeat = false;
            _onService = false;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return default(Type[]); }
        }

        public override bool OnService
        {
            get { return false; }
            set { _onService = false; }
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
            get { return true; }
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
