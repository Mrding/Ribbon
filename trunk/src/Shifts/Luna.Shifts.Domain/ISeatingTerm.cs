using System;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public interface ISeatingTerm : IVisibleTerm
    {
        Int64 Id { get; }

        ISeatingTerm ParentTerm { get; } // nullable

        bool IsNeedSeat { get; } // 是否佔席
        //bool IsAbsent { get; } // 是否為請假事件

        string Seat { get; set; }
    }
}