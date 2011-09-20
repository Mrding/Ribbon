using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using System.Diagnostics;
using Luna.Common.Extensions;
using Luna.Common.Domain;

namespace Luna.Shifts.Domain
{
    public partial class TimeBox
    {
        public virtual void ArrangeSubEvent(IAssignment newTerm, SubEventInsertRule rule, int randomOffset)
        {
            var startTime = newTerm.Start.AddMinutes(rule.TimeRange.StartValue + (randomOffset * rule.OccurScale));
            var newSubEvent = Term.New(startTime, rule.SubEvent, rule.SubEventLength);
            if (newSubEvent.IsInTheRange(newTerm.Start, newTerm.End))
                Create(newSubEvent, (t,success)=>
                                        {
                                            
                                        }, false);
        }

        public virtual void ArrangeSubEvent(IAssignment newTerm, IEnumerable<SubEventInsertRule> rules, Action<ITerm, bool> callback)
        {
            var anyError = false;
            foreach (var rule in rules)
            {
                DateTime startTime;
                // Whole SubEvent should be inside the range of StartValue~EndValue
                var amountOfAvailableOccurPoints = rule.GetAmountOfAvailableOccurPoints();
                if (amountOfAvailableOccurPoints == 1)
                { // SubEvent can only occur at the begin of range
                    startTime = newTerm.Start.AddMinutes(rule.TimeRange.StartValue);
                }
                else
                {
                    var randomOffset = TermExt.ArrangeSubEventRandom.Next(0, amountOfAvailableOccurPoints);
                    startTime = newTerm.Start.AddMinutes(rule.TimeRange.StartValue + (randomOffset * rule.OccurScale));
                }

                var newSubEvent = Term.New(startTime, rule.SubEvent, rule.SubEventLength);

                if (newSubEvent.IsInTheRange(newTerm.Start, newTerm.End))
                {
                    Create(newSubEvent, (t, success) =>
                    {
                        if (!success)
                            anyError = true;
                    }, false);
                }
                else
                    anyError = true;

                if (anyError) break;

            }
            if (callback != null)
                callback(newTerm, !anyError);
        }

        //public virtual void CreateOffWork(Term newTerm, Action<ITerm, bool> callback)
        //{
        //    var foundTerms = TermSet.Retrieve<Term>(newTerm.Start, newTerm.End, o => o.Is<IIndependenceTerm>());
        //    var filteredTerms = foundTerms.Where(t => t.IsCoverd(newTerm.Start, newTerm.End)).OrderBy(o=> o.Start);

        //    var success = CreateValidations[newTerm.GetType()].Invoke(this, newTerm, filteredTerms);
        //    if (success)
        //    {
        //        _termSet.Add(newTerm);
        //        newTerm.UpdateLevel();
        //        newTerm.SetEmployeeId(Id);
        //    }

        //    callback(newTerm, success);
        //}

        public virtual void Create(Term newTerm, Action<ITerm, bool> callback, bool withRplaceDayOff)
        {
            //var success = false;

            //var closestBottomTerm = GetOrderedBottoms(newTerm).FirstOrDefault();
            //if (closestBottomTerm == null || !closestBottomTerm.Locked)
            //{
            //TODO: reconsider OrderBy clause

            var foundTerms = TermSet.Retrieve<Term>(newTerm.Start, newTerm.End, o => o.IsNot<IImmutableTerm>());
            var filteredTerms = foundTerms.Where(t => t.IsCoverd(newTerm.Start, newTerm.End)).OrderByDescending(o => o.Level);

            var success = CreateValidations[newTerm.GetType()].Invoke(this, newTerm, filteredTerms);

            //set up belongToPrev
            //xnewTerm.RectifyAttribution(Boundary, newTerm.Start);

            //temporary remark with IsOutOfBoundary limit
            //if (success && newTerm.IsNot<IOffWork>())
            //{
            //    var lowestTerm = newTerm.GetLowestTerm();
            //    success = !lowestTerm.IsOutOfBoundary(lowestTerm.Start, this);
            //}

            if (!success && newTerm.Exception != null && withRplaceDayOff && newTerm.Exception.CauseTarget is DayOff)
                success = Delete(newTerm.Exception.CauseTarget, false);

            if (success)
            {
                _termSet.Add(newTerm);
                newTerm.UpdateLevel();
                newTerm.SetEmployeeId(Id);
                //xnewTerm.ForceAssignSeat(foundTerms, true);

                Reporting(newTerm);
                //xEmptySeatArrangment<AbsentEvent>(newTerm);
            }

            callback(newTerm, success);
        }

        private bool Delete(Term target, bool trying, Action<IEnumerable<Term>> callback)
        {
            if (target.Is<IImmutableTerm>() || (target.Locked && target.IsNot<AbsentEvent>())) return false;

            Term[] coverdTerms;


            if (target.IsNew()) // unsaved term deleting
                coverdTerms = GetCoveredTermsWithAbsent(target).Where(o => o.Id == 0).ToArray();
            else
                coverdTerms = GetCoveredTermsWithAbsent(target).ToArray();

            if (coverdTerms.Any(t => !(t is AbsentEvent) && t.Locked)) return false; // 有任何请假不准删除

            if (coverdTerms.Length != GetDeletableCoveredTerms(target).Count()) return false;

            if (trying) return true;

            bool result;
            List<Term> deletedTerms;
            if (coverdTerms.Length == 0)
            {
                result = _termSet.Remove(target);
                deletedTerms = new List<Term>(new[] { target });
            }
            else
            {
                result = _termSet.RemoveAll(coverdTerms) && _termSet.Remove(target);
                deletedTerms = new List<Term>(coverdTerms) { target };
            }

            if (result)
            {
                if (target.IsNot<IAssignment>())
                    Reporting(target);
                EmptySeatArrangment<AbsentEvent>(target);
                if (callback != null)
                    callback(deletedTerms);
            }

            return result;
        }

        public virtual bool Delete(Term target, Action<IEnumerable<Term>> callback)
        {
            return Delete(target, false, callback);
        }

        public virtual bool Delete(Term target, bool trying)
        {
            return Delete(target, false, null);
        }

        public virtual bool CanDelete(Term target)
        {
            return Delete(target, true);
        }

        public virtual void SetTime(ITerm term, DateTime unCheckedstart, DateTime uncheckedEnd, Action<ITerm, bool> callback, bool withRplaceDayOff)
        {
            var start = unCheckedstart.TurnToMultiplesOf5();
            var end = uncheckedEnd.TurnToMultiplesOf5();

            var result = false;
            var source = (Term)term;

            var value = new DateRange(start, end);

            if (start >= end)
            {
                source.Exception = new TermException(source, "InvalidTimeValue");
                callback(source, false);
                return;
            }


            bool? childIsInsideOfParent = null;
            source.Bottom.SaftyInvoke(o => childIsInsideOfParent = value.IsInTheRange(o));

            //var assignmentIsOutOfBoundary = source.GetLowestTerm().IsOutOfBoundary(start, this);
            //if(assignmentIsOutOfBoundary)
            //    source.Exception = new TermException(source, "IsOutOfBoundary");
            if (childIsInsideOfParent == false)
                source.Exception = new TermException(source, "BreakAwayFromParent");

            if ((!source.Locked) && childIsInsideOfParent != false) // ((!source.Locked) && !assignmentIsOutOfBoundary && childIsInsideOfParent != false)
            {
                var coverage = source.MeasureCoverage(value.Start, value.End);
                var coverdTerms = TermSet.Retrieve<Term>(coverage.Start, coverage.End, o => !ReferenceEquals(o, source) && o.IsNot<IImmutableTerm>()).ToList();//range filter
                var overlappedTerm = coverdTerms.FirstOrDefault(o => o.Level == source.Level && o.IsCoverd(value));//same level have any covered term
                var relatedTerms = coverdTerms.WhereFamilyTerms(source).ToList(); //relation filter, the real child in the parent

                if (overlappedTerm == null || (withRplaceDayOff && overlappedTerm is DayOff))
                {
                    if (source.IsNot<AbsentEvent>())
                    {
                        overlappedTerm.SaftyInvoke<DayOff>(o =>
                        {
                            _termSet.Remove(o);
                            overlappedTerm = null;
                        });

                        var notMoving = !source.CanCauseMoved(start, end);
                        var offset = new { L = value.Start.Subtract(source.Start), R = value.End.Subtract(source.End) };

                        var verifiedTerms = relatedTerms.Where(o => o.IsInTheRange(source.Start, source.End)).OrderByDescending(o => o.Level);
                        var lockedTerms = relatedTerms.Where(o => o.Locked && o.IsInTheRange(source.Start, source.End));
                        Action applyDelegate = () => { };

                        foreach (var o in verifiedTerms)
                        {
                            var newTime = new DateRange(o.Is<AbsentEvent>() || o.Locked || notMoving ? o.Start : o.Start.Add(offset.L), o.Is<AbsentEvent>() || o.Locked || notMoving ? o.End : o.End.Add(offset.R));
                            if (o.Level < source.Level ? !value.IsInTheRange(newTime) : !newTime.IsInTheRange(value) && o.IsNot<AbsentEvent>())
                            {
                                overlappedTerm = o;
                                break;
                            }

                            if (o.Is<AbsentEvent>() && !o.IsInTheRange(o.ParentTerm == term ? start : o.ParentTerm.Start, o.ParentTerm == term ? end : o.ParentTerm.End))
                            {
                                overlappedTerm = o;
                                break;
                            }

                            if (notMoving || o.Is<AbsentEvent>()) continue;
                            var current = o;
                            var verifyLockedTermIsNotInTheRange = new Func<Term, bool>(lockedTerm =>
                            {
                                if (lockedTerm.Bottom != current) return false;
                                if (!lockedTerm.IsInTheRange(newTime.Start, newTime.End))
                                    overlappedTerm = lockedTerm;
                                return true;
                            });

                            if (lockedTerms.Any(verifyLockedTermIsNotInTheRange)) continue;

                            overlappedTerm = lockedTerms.FirstOrDefault(locked => !ReferenceEquals(current, locked) && ((locked.Level == current.Level && locked.IsCoverd(newTime))));
                            if (overlappedTerm == null && !o.Locked && !o.BottomIsLocked())
                            {
                                applyDelegate += () =>
                                {
                                    current.Snapshot();
                                    current.Start = newTime.Start;
                                    current.End = newTime.End;
                                };
                                current.ForceAssignSeat(coverdTerms, false);
                            }
                            if (overlappedTerm != null) break;
                        }
                        if (overlappedTerm == null) applyDelegate();
                    }

                    if (overlappedTerm == null)
                    {
                        source.Snapshot();
                        source.RectifyAttribution(Boundary, value.Start);
                        source.Start = value.Start;
                        source.End = value.End;
                        result = true;
                        TermSet.Rebuild<Term>();
                        Reporting(source);
                        EmptySeatArrangment<AbsentEvent>(source);
                        source.ForceAssignSeat(coverdTerms, false);
                        source.TryReplaceSeat();
                    }
                }
                if (overlappedTerm != null)
                    source.Exception = new TermException(overlappedTerm, "Covered");
            }

            callback(source, result);
        }

        public virtual void Reporting(ITerm source)
        {
            if (source.Is<IOffWork>()) return;

            var lowestTerm = source.GetLowestTerm() as AssignmentBase;
            if (lowestTerm == null) return;

            BalanceLabourHour(lowestTerm);
            AdjustBoundary(lowestTerm);
        }
    }
}
