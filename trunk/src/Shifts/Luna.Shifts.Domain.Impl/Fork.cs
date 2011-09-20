using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain.Impl
{
    internal class Fork
    {
        private readonly ISeatingTerm _self;
        public readonly List<Fork> _childList;
        public Fork(ISeatingTerm seatingTerm)
        {
            _self = seatingTerm;
            _childList = new List<Fork>();
        }

        public ISeatingTerm Self { get { return _self; } }
        public List<Fork> ChildList { get { return _childList; } }


        public Fork Contain(ISeatingTerm seatingTerm)
        {
            Fork tempReturn;
            if (Self == seatingTerm && Self != null)
            {
                return this;
            }
            else if (ChildList.Count != 0)
            {
                for (int i = 0; i < ChildList.Count; i++)
                {
                    tempReturn = ChildList[i].Contain(seatingTerm);
                    if (tempReturn != null)
                        return tempReturn;
                }
            }
            //else
            //{
            //    return null;
            //}
            return null;
        }

        public List<Fork> GetAllSubFork()
        {
            var TermList = new List<Fork> { this };
            for (var i = 0; i < this.ChildList.Count; i++)
            {
                TermList.AddRange(ChildList[i].GetAllSubFork());
            }
            return TermList;
        }

        public bool[] CreateBinaryList(bool[] timeBinaryList, DateTime StartTime, int maxIndex)
        {
            int startIndex = (int)(Self.Start - StartTime).TotalMinutes;
            int endIndex = (int)(Self.End - StartTime).TotalMinutes;
            // startIndex = Math.Max(startIndex,0);
            // endIndex = Math.Min(endIndex, maxIndex);
            for (int i = startIndex; i < endIndex; i++)
            {
                timeBinaryList[i] = Self.IsNeedSeat;
            }
            if (ChildList.Count != 0)
            {
                foreach (Fork fork in ChildList)
                {
                    timeBinaryList = fork.CreateBinaryList(timeBinaryList, StartTime, maxIndex);
                }
            }
            return timeBinaryList;
        }

        public int GetTreeNodeNumber()
        {
            int count = 0;
            if (ChildList.Count == 0)
                return 1;
            else
                foreach (Fork fk in ChildList)
                    count += fk.GetTreeNodeNumber();
            return count + 1;
        }
    }
}