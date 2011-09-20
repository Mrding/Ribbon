using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common.Interfaces
{
    public interface ISubmitablePresenter
    {
        void SubmitChanges(bool abort);

        bool IsDirty { get; }
    }
}
