using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public class OvertimeAssignment : AssignmentBase
    {
        static readonly List<Type> _canBeOverlapTypes;
        static readonly WorkHourType _payment;
    

        static OvertimeAssignment()
        {
            _canBeOverlapTypes = new List<Type>(new [] { typeof(RegularSubEvent), typeof(OvertimeSubEvent), typeof(UnlaboredSubEvent) });
            _payment = WorkHourType.ExtraPaid;
        }

        public OvertimeAssignment()
        {
        }

        public OvertimeAssignment(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 0;
            _background = "LightBlue";
            _text = "OvertimeAssignment";
            _onService = true;
            _isNeedSeat = true;
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return _canBeOverlapTypes.ToArray(); }
        }

        public override WorkHourType Payment
        {
            get { return _payment; }
        }

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
            get { return true; }
        }

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
