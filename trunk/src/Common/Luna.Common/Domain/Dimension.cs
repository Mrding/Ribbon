namespace Luna.Common
{
    using System;
    using System.Collections.Generic;

    public struct Dimension : IEquatable<Dimension>, IEqualityComparer<Dimension>
    {
        #region Fields

        public static readonly Dimension Invalid = new Dimension();

        private int _columns;
        private int _rows;

        #endregion Fields

        #region Constructors

        public Dimension(int row, int columns)
        {
            _columns = columns;
            _rows = row;
        }

        #endregion Constructors

        #region Properties

        public int Columns
        {
            get
            {
                if (_columns <= 0) _columns = 1;
                return _columns;
            }
            set { _columns = value; }
        }

        public int Count
        {
            get { return _columns * _rows; }
        }

        public int Rows
        {
            get
            {
                if (_rows <= 0) _rows = 1;
                return _rows;
            }
            set { _rows = value; }
        }

        #endregion Properties

        #region Methods

        public static bool operator !=(Dimension x, Dimension y)
        {
            return !x.Equals(y);
        }

        public static bool operator ==(Dimension x, Dimension y)
        {
            return x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Dimension)
            {
                Dimension other = (Dimension)obj;
                return _rows.Equals(other._rows) && _columns.Equals(other._columns);
            }
            return false;
        }

        public bool Equals(Dimension other)
        {
            return _rows.Equals(other._rows) && _columns.Equals(other._columns);
        }

        public bool Equals(Dimension x, Dimension y)
        {
            return EqualityComparer<Dimension>.Default.Equals(x, y);
        }

        public override int GetHashCode()
        {
            return _rows.GetHashCode() ^ _columns.GetHashCode();
        }

        public int GetHashCode(Dimension obj)
        {
            return EqualityComparer<Dimension>.Default.GetHashCode(obj);
        }

        public bool IsOutofRange(int columnIdx, int rowIdx)
        {
            return columnIdx > Columns || columnIdx < 0 || rowIdx > Rows || rowIdx < 0;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", _rows, _columns);
        }

        #endregion Methods
    }
}