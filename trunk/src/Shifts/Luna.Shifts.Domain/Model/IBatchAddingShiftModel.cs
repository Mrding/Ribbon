using System.Collections.Generic;

namespace Luna.Shifts.Domain.Model
{
    public interface IBatchAddingShiftModel : IBatchAlterModel
    {
        IEnumerable<AssignmentType> Types { get; set; }

        AssignmentType SelectedType { get; set; }
    }
}
