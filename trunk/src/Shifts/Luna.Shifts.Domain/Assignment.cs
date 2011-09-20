using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{

    public partial class Assignment : AssignmentBase
    {
        static readonly List<Type> _canBeOverlapTypes;
        static readonly WorkHourType _payment;
        //static TermStyle _defaultStyle;

        static Assignment()
        {
            //any allowed types can cover on assignment but must in the range 
            _canBeOverlapTypes = new List<Type>(new[] { typeof(RegularSubEvent), typeof(UnlaboredSubEvent), typeof(OvertimeSubEvent), typeof(AbsentEvent), typeof(Shrink) });
            _payment = WorkHourType.Paid;
            
            
            //_defaultStyle = new TermStyle
            //                    {
            //                        Background = "Blue",
            //                        Name = "RegularAssignment",
            //                        OnService = true,
            //                        Occupied = true
            //                    };
        }

        public Assignment()
        {
            //_style = _defaultStyle;
        }

        public Assignment(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _level = 0;
            //_style = _defaultStyle;
            Background = "Blue";
            Text = "RegularAssignment";
            OnService = true;
            IsNeedSeat = true;
        }

        public override bool CanAssignAbsent
        {
            get
            {
                return true;
            }
        }


        public override WorkHourType Payment
        {
            get { return _payment; }
        }


        internal override Type[] CanBeOverlapTypes
        {
            get { return _canBeOverlapTypes.ToArray(); }
        }


        //public override int GetHashCode()
        //{
        //    return 10;
        //}

        internal override bool CanNotOverlapWithAbsent
        {
            get { return true; }
        }

        //public static void SetDefaultStyle(TermStyle value)
        //{
        //    _defaultStyle = value;
        //}

        public override bool OnService
        {
            get { return true; }
            set { _onService = true; }
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
        //        _style = value;
        //    }
        //}
    }
}
