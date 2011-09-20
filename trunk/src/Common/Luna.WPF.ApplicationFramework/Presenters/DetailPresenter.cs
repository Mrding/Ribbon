using System;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using System.ComponentModel;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Interfaces;

using System.Linq;

namespace Luna.WPF.ApplicationFramework.Presenters
{

    public class DetailPresenter<TEntity> : Presenter, IDetailPresenter<TEntity>
    {
        protected static string[] exceptPropertyNames = new[] { "IsEditing", "IsNew", "IsEnablePropertyChanged", "IsSelected" };
        protected TEntity _entity;


        public DetailPresenter()
        {
        }

        public void ReAssignEntity(System.Action<TEntity> action)
        {
            action(_entity);
        }

        public IEditingObject EditingObject
        {
            get { return _entity as IEditingObject; }
        }

        public bool IsEntityProperty(string name)
        {
            return !exceptPropertyNames.Contains(name);
        }

        public virtual TEntity Entity
        {
            get { return _entity; }
            set
            {
                if (_entity.IsNotNull())
                {
                    EditingObject.IsEnablePropertyChanged = false;
                    EditingObject.PropertyChanged -= OnEntityPropertyChanged;
                    OnEntityChanging();
                }

                _entity = value;
                if (_entity.IsNot<IEditingObject>())
                    throw new Exception(string.Format("{0} can not support IEditingObject. Must add EditingBehavior from Guywire.", _entity.GetType()));
                this.NotifyOfPropertyChange("Entity");

                EditingObject.IsEnablePropertyChanged = false;
                EditingObject.PropertyChanged += OnEntityPropertyChanged;
                OnEntityChanged();

                EditingObject.IsEnablePropertyChanged = true;
            }
        }

        public virtual void OnEntityChanging() { }

        public virtual void OnEntityChanged() { }

        public Action OnDirty { get; set; }


        protected virtual void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var obj = (IEditing)Entity;
            if (obj.IsEnablePropertyChanged && IsEntityProperty(e.PropertyName))
            {
                obj.IsEditing = true;
                OnDirty.SaftyInvoke(o => o.Invoke());
            }
        }

        protected override void OnShutdown()
        {
            OnDirty = null;
            EditingObject.PropertyChanged -= OnEntityPropertyChanged;
            base.OnShutdown();
        }
    }
}
