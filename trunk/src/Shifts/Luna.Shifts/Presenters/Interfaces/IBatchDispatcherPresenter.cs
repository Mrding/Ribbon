using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IBatchDispatcherPresenter : IPresenter
    {
        IEnumerable<IBatchAlterModel> Patterns { get; }

        IAssignmentBatchAlterModel AssignmentAlterModel { get; }

        IEventBatchAlterModel EventAlterModel { get; }

        IBatchAddingShiftModel AddingShiftModel { get; }

        IBatchAddingEventModel AddingEventModel { get; }
    }
}
