using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Shifts.Data.Listeners;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit, ConversationId = "IBatchAlterModel")]
    public abstract class BatchAlterModelBase : IBatchAlterModel
    {
        protected readonly ITimeBoxRepository _timeBoxRepository;
        protected readonly ITermStyleRepository _termStyleRepository;

        protected IEnumerable<ICustomAction> _optionalActions;

        protected IEnumerable<ICustomFilter> _filters;
        protected DateTime _searchDate;
        protected string _title;


        private TimeSpan _termStartAlterOffSet;
        private TimeSpan _termEndAlterOffSet;

        private Dictionary<TimeBox, Func<Term,bool>> _lastSteps = new Dictionary<TimeBox, Func<Term,bool>>();

        protected Action<ITerm, string, bool> _setTimeCallBack;

        protected BatchAlterModelBase(DateTime watchPoint, ITimeBoxRepository timeBoxRepository, ITermStyleRepository termStyleRepository)
        {
            _timeBoxRepository = timeBoxRepository;
            _termStyleRepository = termStyleRepository;


            _optionalActions = new ICustomAction[]
                                   {
                                       new OptionalAction("SetTermStart", this, new ICustomAction[] { 
                                                           new CustomAction("ExplicitSetTermStart", this , SetTermStart),
                                                           new CustomAction("MoveTerm",this,MoveTerm), 
                                                           new CustomAction("ResizeTermStart",this,ResizeTermStart)
                                       } ),
                                       new OptionalAction("SetTermEnd", this,new ICustomAction[] {  
                                                           new CustomAction("ExplicitSetTermEnd",this, SetTermEnd ), 
                                                           new CustomAction("ResizeTermEnd",this,ResizeTermEnd)
                                       }),
                                       new CustomAction("LockTerm", this, LockTerm),
                                       new CustomAction("DeleteTerm", this, DeleteTerm)};
            _searchDate = watchPoint;
            _newStartTime = _searchDate;
            _newEndTime = _searchDate;
        }

        public abstract string Category { get; }

        public virtual string Title { get { return _title; } }

        public virtual DateTime SearchDate
        {
            get { return _searchDate; }
            set
            {
                _searchDate = DateTime.Parse(value.ToString("yyyy/MM/dd HH:mm"));
                NewStartTime = DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1:HH:mm}", _searchDate, _newStartTime));
            }
        }

        public virtual IEnumerable<ICustomFilter> Filters { get { return _filters; } }

        public virtual IEnumerable<ICustomAction> OptionalActions { get { return _optionalActions; } }

        public virtual TimeSpan StartAlterOffSet { get; set; }

        public virtual TimeSpan EndAlterOffSet { get; set; }

        public virtual TimeSpan MoveOffSet { get; set; }

        private DateTime _newStartTime;
        public virtual DateTime NewStartTime
        {
            get { return DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1:HH:mm}", _searchDate, _newStartTime)); }
            set { _newStartTime = value; }
        }

        private DateTime _newEndTime;
        public virtual DateTime NewEndTime
        {
            get { return DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1:HH:mm}", _searchDate, _newEndTime)); }
            set { _newEndTime = value; }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public abstract void Initial();

        public void OnDispatching()
        {
            _lastSteps.Clear();
        }

        public void TryDispatch(IAgent agent, Func<Term, bool> filter, Action<Term, TimeBox> action, Action<ITerm, string, bool> callback 
            )
        {
            var foundTerms = FindTargetTerms(agent, filter).OrderByStartAndLevel(MoveOffSet);
            _setTimeCallBack = callback;

            foreach (var term in foundTerms)
            {
                //term.Tag = comments;
                //term.Snapshot();

                _termEndAlterOffSet = default(TimeSpan);
                _termStartAlterOffSet = default(TimeSpan);

                action(term, agent.Schedule);


                if (_termStartAlterOffSet != default(TimeSpan) || _termEndAlterOffSet != default(TimeSpan))
                {
                    agent.Schedule.SetTime(term, 
                        term.Start.Add(_termStartAlterOffSet), 
                        term.End.Add(_termEndAlterOffSet), (t, success) =>
                                                                 {
                                                                     _setTimeCallBack(term, "", success);
                                                                 }, false);
                 
                }
            }
        }

       
        public void Comment(string content)
        {
            TermAlteringListener.Batching(content);
        }

        public void TearDown()
        {
            foreach (var filter in Filters)
            {
                filter.Dispose();
            }
            foreach (var optionalAction in OptionalActions)
            {
                optionalAction.Dispose();
            }
            this.SaftyInvoke<IDisposable>(d => d.Dispose());
        }

        public abstract IEnumerable<Term> FindTargetTerms(IAgent agent, Func<Term, bool> pred);

        protected void SetTermStart(Term term, TimeBox timeBox)
        {
            _termStartAlterOffSet = NewStartTime.Subtract(term.Start);
            //term.Start = NewStartTime;
            //timeBox.SetTime(term, NewStartTime, term.End, (t, success) =>
            //                                             {
            //                                                 _setTimeCallBack(term, "", success);
            //                                             } ,false);
        }

        protected void SetTermEnd(Term term, TimeBox timeBox)
        {
            
            _termEndAlterOffSet = NewEndTime.Subtract(term.End);
            //timeBox.SetTime(term, term.Start, NewEndTime, (t, success) =>
            //                                             {
            //                                                 _setTimeCallBack(term, "", success);
            //                                             }, false);
        }

        protected void MoveTerm(Term term, TimeBox timeBox)
        {
            _termStartAlterOffSet += MoveOffSet;
            _termEndAlterOffSet += MoveOffSet;
            //timeBox.SetTime(term, start, end, (t, success) =>
            //                                            {
            //                                                _setTimeCallBack(term, "", success);
            //                                            }, false);
        }

        protected void ResizeTermStart(Term term, TimeBox timeBox)
        {
            _termStartAlterOffSet += StartAlterOffSet;

            //timeBox.SetTime(term, start, term.End, (t, success) =>
            //                                            {
            //                                                _setTimeCallBack(term, "", success);
            //                                            }, false);

        }

        protected void ResizeTermEnd(Term term, TimeBox timeBox)
        {
            _termEndAlterOffSet += EndAlterOffSet;
            
            //timeBox.SetTime(term, term.Start, end, (t, success) =>
            //                                                {
            //                                                    _setTimeCallBack(term, "", success);
            //                                                }, false);
        }

        protected void DeleteTerm(Term term, TimeBox timeBox)
        {
            _termStartAlterOffSet = default(TimeSpan);
            _termEndAlterOffSet = default(TimeSpan);
            var success = timeBox.Delete(term, false);
            _setTimeCallBack(term, "", success);

            var delete = new Func<Term,bool>( t => timeBox.Delete(t, false));

            if (!_lastSteps.ContainsKey(timeBox))
                _lastSteps.Add(timeBox, delete);
        }

        protected void LockTerm(Term term, TimeBox timeBox)
        {
            _termStartAlterOffSet = default(TimeSpan);
            _termEndAlterOffSet = default(TimeSpan);
            term.Snapshot();
            term.Locked = true;
            _setTimeCallBack(term, "", true);   
        }
    }
}
