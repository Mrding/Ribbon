using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Core.Extensions;
using System.Linq;

namespace Luna.Shifts.Domain
{
    public abstract class AssignmentBase : Term, IAssignment
    {
        protected AssignmentBase() { }

        protected AssignmentBase(DateTime start, TimeSpan length)
            : base(start, length)
        {
        }

        internal override bool Independent { get { return true; } }

        public virtual TimeSpan ShrinkageTotals { get; set; }
        public virtual TimeSpan OvertimeTotals { get; set; }
        private TimeSpan _workingTotals;
        public virtual TimeSpan WorkingTotals
        {
            get { return _workingTotals; }
            set { _workingTotals = value; }
        }

        protected bool _asAWork;

        public virtual bool AsAWork
        {
            //get { return ((AssignmentType)Style).AsAWork; }
            get { return _asAWork; }
            set { _asAWork = value; }
        }

        public virtual bool GapGuaranteed
        {
            //get { return ((AssignmentType)Style).GapGuaranteed; }
            get;
            set;
        }

        public virtual bool IgnoreAdherence { get; set; }

        public virtual bool? BelongToPrv { get; set; }

        public override DateTime Start
        {
            get
            {
                return base.Start;
            }
            internal set
            {
                base.Start = value;
                From = value;
            }
        }

        public override DateTime End
        {
            get
            {
                return base.End;
            }
            internal set
            {
                base.End = value;
                Finish = value;
            }
        }

        private DateTime _from;
        public virtual DateTime From
        {
            get
            {
                if (_from == default(DateTime))
                    _from = Start;
                return _from;
            }
            internal set { _from = value; }
        }

        private DateTime _finish;
        public virtual DateTime Finish
        {
            get
            {
                if (_finish == default(DateTime))
                    _finish = End;
                return _finish;
            }
            internal set { _finish = value; }
        }

        public virtual DateTime HrDate { get; set; }

        private string _nativeName;
        public virtual string NativeName
        {
            get { return string.IsNullOrEmpty(_nativeName) ? Text : _nativeName; }
            set { _nativeName = value; }
        }

        //public IEnumerable<SubEventInsertRule> GetSubEventInsertRules()
        //{
        //    return ((AssignmentType)Style).GetSubEventInsertRules();
        //}


        public virtual void AdjustFromFinish(IEnumerable<Term> terms)
        {
            // aditional filter: item.Is<UnlaboredSubEvent>() || item.Is<Shrink>()
            var cells = terms.ConvertToCell(new DateRange(Start, End), 5, item => item.AsARest, null);
            cells.Loop(i => !cells[i], i =>
                                           {
                                               From = Start.AddMinutes((i - 0) * 5);
                                           });
            cells.LoopFromLast(i => !cells[i], i => Finish = Start.AddMinutes((i + 1) * 5));
        }

        public override void Snapshot()
        {
            base.Snapshot();
            _snapshot["IsNeedSeat"] = base.IsNeedSeat;
            _snapshot["BelongToPrv"] = BelongToPrv;
        }

        internal override void Reset()
        {
            //if (_snapshot != null && _snapshot.ContainsKey("OccupyStatus"))
            //    

            //if (_snapshot != null && _snapshot.Count == 1)
            //    EndEdit();

            if (_snapshot == null || !_snapshot.ContainsKey("Start")) return;
            Start = (DateTime)_snapshot["Start"];
            End = (DateTime)_snapshot["End"];
            Level = (int)_snapshot["Level"];
            Locked = (bool)_snapshot["Locked"];
            IsNeedSeat = (bool)_snapshot["IsNeedSeat"];
            BelongToPrv = (bool)_snapshot["BelongToPrv"];
            EndEdit();
        }

        //public string GetMD5Code()
        //{

        //    return string.Empty;
        //}

        //private string GetSummary(Term term)
        //{
        //    var summary = ;
        //    return summary;
        //}




        //public bool WatingForAssignSeat()
        //{
        //    return 
        //}

        //public override void Snapshot()
        //{
        //    base.Snapshot();
        //    _snapshot["From"] = From;
        //    _snapshot["Finish"] = Finish;
        //}

        //public override void Reset()
        //{
        //    if (_snapshot == null || !_snapshot.ContainsKey("Start")) return;

        //    Start = _snapshot["Start"];
        //    End = _snapshot["End"];
        //    From = _snapshot["From"];
        //    Finish = _snapshot["Finish"];
        //    EndEdit();
        //}
    }
}
