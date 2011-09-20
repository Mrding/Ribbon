using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class PriorityEmployee : SequentialEntity<Employee>
    {
        public virtual ISeat Seat { get; set; }
    }



    public class Seat : AbstractEntity<Guid>, ISeat, IEquatable<Seat>, IEqualityComparer<Seat>
    {
        private IArea _area;
        private string _number = string.Empty;
        private string _extNo = string.Empty;
        private int _row;
        private int _column;
        private Dimension _areaDimension;

        public Seat() { }

        public Seat(int locationIndex, Dimension areaDimension)
            : this()
        {

            LocationIndex = locationIndex;
            InUse = false;
            _areaDimension = areaDimension;
        }

        public Seat(string number, string extNo, int row, int column)
        {
            Number = number;
            ExtNo = extNo;
            InUse = true;
            IsOpen = true;
            _row = row;
            _column = column;
        }

        public virtual void SetRowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public virtual void SetLocation(Dimension dimension)
        {
            if (_row == 0 && _column == 0)
                LocationIndex = YCord * dimension.Columns + XCord;
            else
                LocationIndex = _row * dimension.Columns + _column;
        }

        public override Guid Id { get; protected set; }

        public virtual string Number
        {
            get { return _number; }
            set { _number = value.Trim(); }
        }

        public virtual string ExtNo
        {
            get { return _extNo; }
            set { _extNo = value.Trim(); }
        }

        private int _locationIndex;
        public virtual int LocationIndex
        {
            get { return _locationIndex; }
            set
            {
                _locationIndex = value;
            }
        }

        public virtual int Rank { get; set; }

        public virtual bool InUse { get; set; }

        public virtual bool IsOpen { get; set; }

        private bool? _isActivated;
        public virtual bool IsActivated
        {
            get
            {
                if (_isActivated == null)
                    _isActivated = Id != default(Guid) && !string.IsNullOrEmpty(Number) && !string.IsNullOrEmpty(ExtNo);
                return _isActivated.Value;
            }
            set { _isActivated = value; }
        }

        public virtual void MarkAsInActive()
        {
            Number = string.Empty;
            ExtNo = string.Empty;
            InUse = false;
            IsActivated = false;
            Id = Guid.Empty;
        }

        public virtual int YCord
        {
            get { return LocationIndex / _areaDimension.Columns; }
        }

        public virtual int XCord
        {
            get { return LocationIndex % _areaDimension.Columns; }
        }

        public virtual int GetRow(Dimension dimension)
        {
            return LocationIndex / dimension.Columns;
        }

        public virtual int GetColumn(Dimension dimension)
        {
            return LocationIndex % dimension.Columns;
        }

        public virtual Entity Area
        {
            get { return _area as Entity; }
            set
            {
                _area = value as IArea;
                if (_area == null)
                    return;
                _areaDimension = _area.Dimension;
            }
        }

        /// <summary>
        /// Priority
        /// </summary>
        private int _index;
        public virtual int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private Entity _usingOrganozation;
        public virtual Entity UsingOrganozation
        {
            get { return _usingOrganozation; }
            set { _usingOrganozation = value; }
        }

        Entity ISeatingSeat.PriorityOrganization
        {
            get
            {
                if (_usingOrganozation == null)
                    return null;
                return _usingOrganozation;
            }
        }

        //private IDictionary<ISimpleEmployee, int> _priorityEmployees = new Dictionary<ISimpleEmployee, int>();

        //public virtual IDictionary<ISimpleEmployee, int> PriorityEmployees
        //{
        //    get { return _priorityEmployees; }
        //    set
        //    {
        //        if (value == null)
        //            return;
        //        _priorityEmployees = value;
        //    }
        //}

        //public virtual void NotifyPriorityEmployeesChanged()
        //{
        //    PriorityEmployees = null;
        //}


        public override int GetHashCode()
        {
            return _extNo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Seat;
            if (other == null) return false;
            return this.Equals(other);
        }

        public virtual bool Equals(Seat other)
        {
            if (ReferenceEquals(this, other)) return true;
            return _extNo == other.ExtNo && _locationIndex == other.LocationIndex && Area.Equals(other.Area);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Area.Name, Number);
        }

        public virtual bool Equals(Seat x, Seat y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public virtual int GetHashCode(Seat obj)
        {
            return obj.GetHashCode();
        }
    }
}
