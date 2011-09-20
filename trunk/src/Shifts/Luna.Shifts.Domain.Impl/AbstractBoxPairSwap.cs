using System;
using System.Collections.Generic;
using System.Text;
using Luna.Globalization;

namespace Luna.Shifts.Domain.Impl
{
    public abstract class AbstractBoxPairSwap : Pair<BoxSwapModel>
    {
        private bool TermGapHasMatched
        {
            get
            {
                if (Applier.Term != null && Replier.Term != null)
                    return Applier.Term.End - Applier.Term.Start == Replier.Term.End - Replier.Term.Start;
                return false;
            }
        }

        protected Dictionary<Guid, Dictionary<string, bool>> SetValidates()
        {
            var results = new Dictionary<Guid, Dictionary<string, bool>>
                              {
                                  {Applier.TimeBox.Id, Applier.SetValidate()},
                                  {Replier.TimeBox.Id, Replier.SetValidate()}
                              };
            return results;
        }
        protected string WithMessage()
        {
            var stringBuilder = new StringBuilder(100);
            if (Applier.Message.Length > 0)
            {
                //.Append(LanguageReader.GetValue("Shifts_BoxPairSwap_Applier")).Append(LanguageReader.GetValue("Shifts_BoxPairSwap_Message"))
                stringBuilder.Append(Applier.Message);
            }
            if (Replier.Message.Length > 0)
            {
                stringBuilder.Append(Replier.Message);
            }
            return stringBuilder.ToString();
        }
        protected void VaildateSwapMessage()
        {
            if (Applier.FailedTerm.Count > 0)
            {
                Applier.HasExchanged = true;
                Applier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_CreateTermFail"));
                //foreach (var term in Applier.FailedTerm)
                //{
                //    Applier.Message.Append(term).Append(",");
                //}
            }
            if (Replier.FailedTerm.Count > 0)
            {
                Replier.HasExchanged = true;
                Replier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_CreateTermFail"));
                //foreach (var term in Replier.FailedTerm)
                //{
                //    Replier.Message.Append(term).Append(",");
                //}
            }
        }

        protected void ValidateTermGapHasMatched()
        {
            if (!TermGapHasMatched)
            {
                Applier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_TermGapHasMatched"));
            }
        }

        protected void ValidateHasTimeOff()
        {
            if (Applier.HasTimeOff)
            {
                Applier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasTimeOff"));
            }
            if (Replier.HasTimeOff)
            {
                Replier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasTimeOff"));
            }
        }

        protected void ValidateHasAbsentEvent()
        {
            if (Applier.HasAbsentEvent)
            {
                Applier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasAbsentEvent"));
            }
            if (Replier.HasAbsentEvent)
            {
                Replier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasAbsentEvent"));
            }
        }

        protected void ValidateHasLocked()
        {
            if (Applier.Lockeds.Count != 0)
            {
                Applier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasLocked"));
            }
            if (Replier.Lockeds.Count != 0)
            {
                Replier.Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_HasLocked"));
            }
        }
        
        protected void SetTimeOff()
        {
            Applier.SetTimeOff(Applier.TimeOff, Applier.Term);
            Replier.SetTimeOff(Replier.TimeOff, Replier.Term);
        }

        /// <summary>
        /// 指定班互换
        /// </summary>
        protected void SwapSpecificTerm()
        {
            SwapTerms(new Pair<Term>(Applier.Term, Replier.Term));
        }

        /// <summary>
        /// 指定班互换
        /// </summary>
        protected void SwapSpecificTermIsNeedSeat()
        {
            //在申请方上新增回复方的班
            Applier.CreateTermWithSeat(Replier.Term, Applier.Term.Seat);
            //在回复方上新增申请方的班
            Replier.CreateTermWithSeat(Applier.Term, Replier.Term.Seat);
        }


        protected void SwapTerms(Pair<Term> term)
        {
            if (term.Replier != null)
            {
                //在申请方上新增回复方的班
                Applier.CreateTerm(term.Replier);
            }
            if (term.Applier != null)
            {
                //在回复方上新增申请方的班
                Replier.CreateTerm(term.Applier);
            }
        }

        protected void SwapTerms(Pair<IList<Term>> terms)
        {
            foreach (var item in terms.Replier)
            {
                Applier.CreateTerm(item);
            }
            foreach (var item in terms.Applier)
            {
                Replier.CreateTerm(item);
            }
        }

        protected void CreateTerms(Pair<IList<Term>> terms)
        {
            foreach (var item in terms.Applier)
            {
                Applier.CreateTerm(item);
            }
            foreach (var item in terms.Replier)
            {
                Replier.CreateTerm(item);
            }
        }

        protected void SwapSpecificSubEvents()
        {
            if (Replier.Term != null)
            {
                //在回复方上面有事件的情况下，也要同时新增。
                foreach (var item in Replier.SubEvents)
                {
                    Applier.CreateTerm(item);
                }
            }

            if (Applier.Term != null)
            {
                //在申请方上面有事件的情况下，也要同时新增。
                foreach (var item in Applier.SubEvents)
                {
                    Replier.CreateTerm(item);
                }
            }
        }

        protected void DeleteTerm()
        {
            Applier.DeleteTerm(Applier.Term);
            Replier.DeleteTerm(Replier.Term);
        }

        protected void DeleteTerms(Pair<IList<Term>> terms)
        {
            foreach (var assignment in terms.Applier)
            {
                Applier.DeleteTerm(assignment);
            }
            foreach (var assignment in terms.Replier)
            {
                Replier.DeleteTerm(assignment);
            }
        }
    }
}
