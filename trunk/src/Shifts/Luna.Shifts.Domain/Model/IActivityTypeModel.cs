using System.Collections.Generic;
using Luna.Infrastructure.Domain.Model;


namespace Luna.Shifts.Domain.Model
{
    using Luna.Common.Model;
    using Luna.Common;

    public interface IActivityTypeModel : IModel<TermStyle>
    {
        IEnumerable<TermStyle> GetSubEventTypes();

        IEnumerable<TermStyle> GetAbsentEventTypes();

        bool DuplicationChecking(Entity entity, string name);

        void Release();
    }
}
