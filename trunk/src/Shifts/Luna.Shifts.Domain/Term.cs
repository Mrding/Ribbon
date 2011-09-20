using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public abstract partial class Term : AbstractEntity<Int64> , IHierarchicalTerm, IStyledTerm, IComparable, IComparable<Term>, ISeatingTerm
        
    {
        protected Dictionary<string, object> _snapshot;
        protected int _level;
        //protected TermStyle _style;
        protected Guid _employeeId;

        protected Action _rollbackAction = () => { };


        internal Term() { }

        protected Term(DateTime start, TimeSpan length)
        {
            this.Start = start.ConvertToMultiplesOfFive();
            this.End = start.Add(length).ConvertToMultiplesOfFive();
        }

        #region Hibernate field

        //public virtual Int64 Id { get; set; }

        private DateTime _start;
        public virtual DateTime Start
        {
            get { return _start; }
            internal set
            {
                
                //var checkValue = value.TurnToMultiplesOf5();
                _start = value; //= checkValue == value ? value : checkValue;
                //CheckTime();
            }
        }

        private DateTime _end;
        public virtual DateTime End
        {
            get { return _end; }
            internal set
            {
                //var checkValue = value.TurnToMultiplesOf5();
                _end = value;//= checkValue == value ? value : checkValue;
                //CheckTime();
            }
        }

        //private void CheckTime()
        //{
        //    if (_start != default(DateTime) && _end != default(DateTime))
        //        if (_start > _end)
        //        {
        //            throw new InvalidOperationException("班表的開始時間大於結束時間");
        //        }
        //}


        protected bool _onService;

        public virtual bool OnService
        {
            //get
            //{
            //    if (_style == null)
            //        return true;
            //    return Style.OnService;
            //}
            get { return _onService; }
            set { _onService = value; }
        }

        public virtual bool Locked { get; set; }

        public virtual int Level
        {
            get { return _level; }
            set
            {
                if (_level == value) return;
                _level = value;
            }
        }

        private Term _bottom;
        public virtual Term Bottom
        {
            get { return _bottom; }
            set
            {
                if (_bottom == value) return;

                if (_bottom == null)
                    _bottom = value;
            }
        }

        #endregion

        public TermException Exception { get; set; }

        public abstract WorkHourType Payment { get; }

        //public virtual TermStyle Style
        //{
        //    get { return _style; }
        //    internal set { _style = value; }
        //}

        protected string _background;

        public virtual string Background
        {
            //get { return Style.Background; }
            get { return _background; }
            set { _background = value; }
        }

        public virtual bool CanAssignAbsent { get { return false; } }

        internal abstract Type[] CanBeOverlapTypes { get; }

        internal virtual bool Independent { get { return false; } }

        internal abstract bool CanNotOverlapWithAbsent { get; }

        public virtual bool Overwritable { get { return false; } }

        protected string _text;

        public virtual string Text
        {
            //get { return Style.Name; }
            get { return _text; }
            set { _text = value; }
        }

        public virtual string Remark { get; set; }

        protected bool _asARest;

        public virtual bool AsARest
        {
            //get { return Style.AsARest; }
            get { return _asARest; }
            set { _asARest = value; }
        }

        public virtual bool CanBeOverlap(Type type)
        {
            if (CanBeOverlapTypes == null) return false;
            if (CanBeOverlapTypes.Contains(type)) return true;
            return false;
        }

        public virtual bool NeedRelyOn(Term other)
        {
            return false; // most of type no need to rely with others
        }

        public virtual bool EnclosedWithTypeVerification(Term other)
        {
            return other.IsInTheRange(this.Start, this.End) && CanBeOverlap(other.GetType()); // && (other.GetType() == typeof(AbsentEvent) || this.CanOverlap(other.GetType()));
        }

        public virtual bool OverlapNotEnclosed(Term other)
        {
            if (EnclosedWithTypeVerification(other)) return false;
            return this.AnyOverlap(other);
        }

        public virtual bool Repellent(Term other)
        {
            return other.GetType() == this.GetType() || Independent == other.Independent;
        }

        public override string ToString()
        {
            return string.Format("{0:MM-dd HH:mm} - {1:MM-dd HH:mm} {2}", Start, End, this.GetType().Name);
        }

        public virtual bool SameTime(object obj)
        {
            var other = obj as Term;
            if (other == null) return false;
            if (this == obj) return true;

            return Independent == other.Independent && Start == other.Start && End == other.End;
        }

        /// <summary>
        /// For moving use
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsEditableChildTerm(Term other)
        {
            if (this is IAssignment)
                return !other.Locked && other.IsNot<AbsentEvent>() && this.EnclosedWithTypeVerification(other);
            //return !other.Locked && other.Bottom is IAssignment && other.GetType() != typeof(AbsentEvent) && this.EnclosedWithTypeVerification(other) && !other.Bottom.Locked;
            return !other.Locked && !other.Bottom.Locked && other.IsNot<AbsentEvent>() && this.EnclosedWithTypeVerification(other);

        }



        /// <summary>
        /// For delete use
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsDeletableChildTerm(Term other)
        {
            return !other.Locked && other.GetType() != typeof(AbsentEvent) && this.EnclosedWithTypeVerification(other) && !other.Bottom.Locked;
        }

        public virtual bool CanCauseMoved(DateTime start, DateTime end)
        {
            return Start != start && End != end && end.Subtract(start) == End.Subtract(Start);
        }

        public static Term FindBottomMost(Term bottom)
        {
            if (bottom.Bottom != null)
                return FindBottomMost(bottom.Bottom);
            return bottom;
        }

        public virtual void Snapshot()
        {
            if (_snapshot == null) _snapshot = new Dictionary<string, object>();
            _snapshot["Start"] = Start;
            _snapshot["End"] = End;
            _snapshot["Level"] = Level;
            _snapshot["Locked"] = Locked;
            _snapshot["EmployeeId"] = _employeeId;
            _snapshot["Tag"] = Tag;
        }

        public virtual void SwapSnapshotData()
        {
            if (_snapshot == null || _snapshot.Count == 0) return;

            object t;
            t = Start;
            Start = _snapshot["Start"].As<DateTime>();
            _snapshot["Start"] = t;

            t = End;
            End = _snapshot["End"].As<DateTime>();
            _snapshot["End"] = t;

            t = Level;
            Level = _snapshot["Level"].As<int>();
            _snapshot["Level"] = t;

            t = Locked;
            Locked = _snapshot["Locked"].As<bool>();
            _snapshot["Locked"] = t;

            t = _employeeId;
            _employeeId = _snapshot["EmployeeId"].As<Guid>();
            _snapshot["EmployeeId"] = t;

            t = Tag;
            Tag = _snapshot["Tag"].As<string>();
            _snapshot["Tag"] = t;

        }

        public virtual void EndEdit()
        {
            if (_snapshot == null) return;

            _snapshot.Clear();
            _snapshot = null;
        }

        public bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            var start = this.GetSnapshotValue<DateTime>("Start");
            var end = this.GetSnapshotValue<DateTime>("End");

            oldStart = start;
            oldEnd = end;

            if (compareWithDateOnly)
                return Start.Date != start.Date || End.Date != end.Date;

            return Start != start || End != end;
        }

        public void AddRollBackAction(Action action)
        {
            if (!_rollbackAction.GetInvocationList().Contains(action))
                _rollbackAction += action;
        }

        private Action _afterRollBackAction = () => { };
        public void AddAfterRollBackAction(Action action)
        {
            if (!_afterRollBackAction.GetInvocationList().Contains(action))
                _afterRollBackAction += action;
        }

        public T GetSnapshotValue<T>(string propertyName)
        {
            if (_snapshot == null)
                Snapshot();

            return (T)_snapshot[propertyName];
        }

        internal virtual void Reset()
        {
            if (_rollbackAction != null)
            {
                _rollbackAction.Invoke();
                _rollbackAction = () => { };
            }
            if (_snapshot == null || !_snapshot.ContainsKey("Start")) return;
            Start = (DateTime)_snapshot["Start"];
            End = (DateTime)_snapshot["End"];
            Level = (int)_snapshot["Level"];
            Locked = (bool)_snapshot["Locked"];
            Tag = (string)_snapshot["Tag"];

            EndEdit();

            if (_afterRollBackAction != null)
            {
                _afterRollBackAction.Invoke();
                _afterRollBackAction = () => { };
            }
        }

        internal void SetEmployeeId(Guid value)
        {
            _employeeId = value;
        }

        internal virtual void UpdateLevel()
        {
            if (this.Bottom != null) this.Level = this.Bottom.Level + 1;


        }

        public virtual double LaborMinutes
        {
            get;
            internal set;
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            var other = obj as Term;
            if (other == null) return -1;
            return CompareTo(other);
        }

        public int CompareTo(Term other)
        {
            var cLevel = other.Level.CompareTo(Level);
            if (cLevel == 0)
            {
                cLevel = other.Start.CompareTo(Start);
            }
            return -cLevel;
        }

        #endregion


        public ISeatingTerm ParentTerm
        {
            get { return Bottom; }
        }

        protected bool _isNeedSeat;

        public virtual bool IsNeedSeat
        {
            get
            {
                if (this is UnlaboredSubEvent || this is AbsentEvent) return false;
                return _isNeedSeat;
            }
            set { _isNeedSeat = value; }
        }

        public virtual bool GetIsNeedSeatField()
        {
            return _isNeedSeat;
        }

        public virtual string Seat { get; set; }

        public virtual string Tag { get; set; }

        public virtual Int64 Version { get; set; }

        internal virtual Guid TermStyleId { get; set; }
    }

}
