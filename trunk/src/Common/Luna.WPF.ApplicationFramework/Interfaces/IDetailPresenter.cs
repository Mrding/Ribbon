using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Interfaces
{
    using Caliburn.PresentationFramework.ApplicationModel;

    public interface IDetailPresenter<TEntity> : IPresenter
    {
        //void ReAssignEntity(System.Action<TEntity> action);

        TEntity Entity { get; set; }

        IEditingObject EditingObject { get; }

        bool IsEntityProperty(string name);

        void OnEntityChanging();

        void OnEntityChanged();

        Action OnDirty { get; set; }

        // ???
        //bool CanSave();
    }
}
