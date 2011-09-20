using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Shifts.Domain
{
    public class UnknowAssignment: AssignmentBase
    {
        static readonly List<Type> _canBeOverlapTypes;
        static readonly WorkHourType _payment;
    

        static UnknowAssignment()
        {
            _canBeOverlapTypes = new List<Type>();
            _payment = WorkHourType.Unpaid;
        }

        public UnknowAssignment()
        {
        }

        public UnknowAssignment(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 0;
            _background = "Silver";
            _text = "DO";
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

        public override DateTime HrDate
        {
            get { return Start; }
            set
            {
                
            }
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
