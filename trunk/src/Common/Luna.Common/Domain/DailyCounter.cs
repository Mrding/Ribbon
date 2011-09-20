using System;
using System.Linq;
using System.Text;

namespace Luna.Common.Domain
{
    public class CellTerm : DateTerm
    {
        private string _text;

        public CellTerm(DateTime date, string text):base(date, "")
        {
            _text = text;
        }

        public string ErrorInfo { get; set;}

        public override string Text
        {
            get { return _text; }
        }
    }

    public class DailyCounter : DateTerm
    {
     
        public DailyCounter(DateTime date, string format):base(date, format)
        {
        }
        public virtual double Value { get; set; }

        public override string Text
        {
            get { return Value.ToString(_format); }
        }

    }

    public class DailyCounter<T> : DateTerm, IDisposable
    {
        public DailyCounter() { }

        private T _kind;

        public DailyCounter(T kind, DateTime date, double max)
            : base(date, string.Empty)
        {
            _kind = kind;
            Max = max;
            Value = 0;
        }

        public virtual T Kind { get { return _kind; } set { _kind = value; } }

        public virtual double Max { get; set; }

        public virtual double Value { get; set; }

        public virtual double Remains { get { return Max - Value; } }

        

        public override string Text
        {
            get
            {
                return string.Format("{0:0}/{1:0}" ,Value, Max);
            }
        }
         
        public override string Background
        {
            get { return IsDirty ? "" : "Transparent"; }
        }

        public void Dispose()
        {
            _kind = default(T);
        }
    }
}
