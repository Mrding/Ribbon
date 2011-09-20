using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public class ExternalActivity : AbsentEvent, IAdherenceTerm, IImmutableTerm
    {

        public ExternalActivity() { }

        public ExternalActivity(DateTime start, TimeSpan length, string payment, Guid employeeId)
            : base(start, length)
        {
            _employeeId = employeeId;
            _customPayment = payment;
        }

        public override bool Locked
        {
            get { return true; }
            set { }
        }

        public override bool OnService
        {
            get { return _onService; }
            set
            {
                _onService = value;
            }
        }

        public virtual bool IgnoreAdherence { get; set; }
        private string _customPayment;
    }


    public class AbsentEvent : Term, IDependencyTerm
    {
        static readonly List<Type> _typesOfRelyOn;
        protected WorkHourType _payment;
        //static TermStyle _defaultStyle;


        static AbsentEvent()
        {
            //_isOcuppied = false;
            _typesOfRelyOn = new List<Type>(new[] { typeof(Assignment), typeof(RegularSubEvent), typeof(UnlaboredSubEvent) });

            //_defaultStyle = new TermStyle
            //                    {
            //                        Background = "LightCoral",
            //                        Name = "AbsentEvent",
            //                        Occupied = false,
            //                        OnService = false
            //                    };
        }

        public AbsentEvent()
        {
            // _style = _defaultStyle;

        }

        public AbsentEvent(DateTime start, TimeSpan length)
            : base(start, length)
        {
            _payment = WorkHourType.Unpaid;
            _level = 3;
            _background = "LightCoral";
            _text = "AbsentEvent";
            _isNeedSeat = false;
            _onService = false;
        }


        public AbsentEvent(DateTime start, TimeSpan length, TermStyle style)
            : base(start, length)
        {
            _level = 3;
            this.MAssign(new { style.Background, Text = style.Name, IsNeedSeat = style.Occupied });
        }

        internal override Type[] CanBeOverlapTypes
        {
            get { return default(Type[]); }
        }

        public override WorkHourType Payment
        {
            get { return _payment; }
        }

        public override bool NeedRelyOn(Term other)
        {
            return _typesOfRelyOn.Contains(other.GetType());
        }

        public override bool Repellent(Term other)
        {
            return base.Repellent(other) || !_typesOfRelyOn.Contains(other.GetType());
        }

        public override bool IsNeedSeat
        {
            get { return false; }
            set { _isNeedSeat = false; }
        }

        public override bool OnService
        {
            get { return false; }
            set
            {
                if (value) return;
                _onService = false;
            }
        }

        public override int Level
        {
            get { return 10; }
        }
        //internal override void UpdateLevel()
        //{
        //    base.UpdateLevel();
        //    Bottom = Bottom.GetLowestTerm() as Term;
        //}


        public override bool Overwritable { get { return true; } }

        internal override bool CanNotOverlapWithAbsent
        {
            get { return true; }
        }
    }
}
