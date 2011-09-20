using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public class OvertimeSubEvent : Term, IDependencyTerm
    {
        //static readonly List<Type> _canBeOverlapTypes;
        static readonly List<Type> _typesOfRelyOn;
        static readonly WorkHourType _payment;
        //static bool _isOcuppied;

        static OvertimeSubEvent()
        {
            _typesOfRelyOn = new List<Type>(new Type[] { typeof(Assignment), typeof(OvertimeAssignment), typeof(UnlaboredSubEvent) });
            _payment = WorkHourType.ExtraPaid;
        
        }

        //public static void SetTypeParamaters(bool isOcuppied)
        //{
        //    _isOcuppied = isOcuppied;
        //}

        public OvertimeSubEvent()
        {
            //_style = _defaultStyle;
        }

        public OvertimeSubEvent(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 1;
            //_style = _defaultStyle;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return default(Type[]); }
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
        //        if (base._style == null) _style = _defaultStyle;
        //        return base.Style;
        //    }
        //    internal set
        //    {
        //        if(value != null && value.Type == this.GetType())
        //            _style = value;
        //    }
        //}
    }
}
