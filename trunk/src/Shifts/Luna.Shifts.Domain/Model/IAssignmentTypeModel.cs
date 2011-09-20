using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Model;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IAssignmentTypeModel : IModel<BasicAssignmentType>
    {
        IEnumerable<BasicAssignmentType> GetAllAssignmentTypes();

        IList<ServiceQueue> GetAllServiceQueues();

        BasicAssignmentType SaveAsNew(string newName, Type type, ref BasicAssignmentType source);

        bool DuplicationChecking(TermStyle entity, string name);

        void Release();
    }
}
