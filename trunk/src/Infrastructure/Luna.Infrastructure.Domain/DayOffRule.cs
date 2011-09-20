namespace Luna.Infrastructure.Domain
{

    public class DayOffRule
    {

        public DayOffRule()
        {
            Add1DayOffEachSaturdayInCS = true;
            Add1DayOffEachSundayInCS = true;
            Add1DayOffEachHolidayInCS = true;
            HolidayShiftRule = HolidayShiftRule.FreeToAssign;
        }

        public virtual bool Add1DayOffEachSaturdayInCS { get; set; }

        public virtual bool Add1DayOffEachSundayInCS { get; set; }

        public virtual bool Add1DayOffEachHolidayInCS { get; set; }

        public virtual HolidayShiftRule HolidayShiftRule { get; set; }

        private int? _maxFwTimes = 5;
        public virtual int MaxFWTimes
        {
            get { return _maxFwTimes ?? 0; }
            set
            {
                if (_minFwTimes.HasValue && value < _minFwTimes)
                {
                    if (_maxFwTimes < _minFwTimes)
                        _maxFwTimes = _minFwTimes;
                }
                else
                    _maxFwTimes = value;
            }
        }

        private int? _minFwTimes = 0;
        public virtual int MinFWTimes
        {
            get { return _minFwTimes ?? 0; }
            set
            {
                if (_maxFwTimes.HasValue && value > _maxFwTimes)
                {
                    if (_minFwTimes > _maxFwTimes)
                        _minFwTimes = _maxFwTimes;
                }
                else
                    _minFwTimes = value;
                Calculate();
            }
        }

        private int? _maxPwTimes = 5;
        public virtual int MaxPWTimes
        {
            get { return _maxPwTimes ?? 0; }
            set
            {
                if (_minPwTimes.HasValue && value < _minPwTimes)
                {
                    if (_maxPwTimes < _minPwTimes)
                        _maxPwTimes = _minPwTimes;
                    return;
                }
                _maxPwTimes = value;
            }
        }

        private int? _minPwTimes = 0;
        public virtual int MinPWTimes
        {
            get { return _minPwTimes ?? 0; }
            set
            {
                if (_maxPwTimes.HasValue && value > _maxPwTimes)
                {
                    if (_minPwTimes > _maxPwTimes)
                        _minPwTimes = _maxPwTimes;
                }
                else
                    _minPwTimes = value;
                Calculate();
            }
        }


        private bool _systemAccumulate = true;
        public virtual bool SystemAccumulate
        {
            get { return _systemAccumulate; }
            set { _systemAccumulate = value; }
        }

        private int _specifiedNumberOfDays = 8;

        public virtual int SpecifiedNumberOfDays
        {
            get { return _specifiedNumberOfDays; }
            set { _specifiedNumberOfDays = value; }
        }

        public virtual int MinimumTotalDayOff { get; set; }

        private void Calculate()
        {
            MinimumTotalDayOff = (MinFWTimes * 2) + MinPWTimes;
        }
    }
}