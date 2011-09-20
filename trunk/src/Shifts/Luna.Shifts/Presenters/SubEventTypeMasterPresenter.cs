using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.Filters;
using Luna.Globalization;
using Luna.WPF.ApplicationFramework;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Caliburn.Core.Metadata;
using Luna.Shifts.Presenters.Interfaces;
using Luna.Common;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(ISubEventTypeMasterPresenter))]
    public class SubEventTypeMasterPresenter : MasterPresenter<ISubEventTypeDetailPresenter, TermStyle>, ISubEventTypeMasterPresenter
    {
        private readonly IActivityTypeModel _model;

        public SubEventTypeMasterPresenter(IEntityFactory entityFactory, IActivityTypeModel model)
            : base(entityFactory, model)
        {
            _model = model;
        }

        protected override void OnInitialize()
        {
            InitialDetailPresenters(_model.GetSubEventTypes(), null, t => t);
            base.OnInitialize();
        }

        protected override void OnCancelEdit(ISubEventTypeDetailPresenter presenter)
        {
            if (!presenter.EditingObject.IsEditing) return;

            var entityRef = presenter.Entity;
            _model.Reload(ref entityRef);
            presenter.Entity = entityRef;
        }

        public bool CanMakeChangeForCurrentPresenter()
        {
            return CurrentPresenter != null;
        }

        [Preview("CanMakeChangeForCurrentPresenter")]
        [Dependencies("CurrentPresenter")]
        [InputText(PartView = "Shifts.SubEventTypeDetailDialog", ConfirmDelegate = "[Event Unchecked] = [Action SaveOrUpdate($dataContext)] ; [Event Indeterminate] = [Action Cancel]")]
        public IDialogBoxPresenter OpenEdit(IDialogBoxPresenter dialog)
        {
            dialog.Text = CurrentEntity.Text;
            return dialog;
        }

        protected override ISubEventTypeDetailPresenter CreateDetailPresenter()
        {
            var newPresenter = base.CreateDetailPresenter();
            //newPresenter.Entity.Name = LanguageReader.GetValue<string>("Shifts_SubEventTypeDetail_NewName");
            newPresenter.Entity.TimeRange = new TimeValueRange(0, 60);
            newPresenter.Entity.Type = typeof(RegularSubEvent);
            newPresenter.Entity.Background = "LightGreen";
            newPresenter.Entity.OnService = false;
            newPresenter.Entity.Occupied = true;
            return newPresenter;
        }

        [InputText(PartView = "Shifts.SubEventTypeDetailDialog", ConfirmDelegate = "[Event Unchecked] = [Action SaveOrUpdate($dataContext)] ; [Event Indeterminate] = [Action Delete]")]
        public IDialogBoxPresenter OpenNew(IDialogBoxPresenter dialog)
        {
            New();
            return dialog;
        }

        public void SaveOrUpdate(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;

            if (_model.DuplicationChecking(CurrentEntity, result.Text))
            {
                result.ConfrimCallback(new Exception("duplication of name"));
            }
            else
            {
                CurrentEntity.Name = result.Text;
                Save();
                result.Close();
            }
        }

        public override void ViewUnloaded(object view, object context)
        {
            Shutdown();
        }

        protected override void OnShutdown()
        {
            _model.Release();
            base.OnShutdown();
        }

        //[Preview("CanMakeChangeForCurrentPresenter")]
        //[Dependencies("CurrentPresenter")]
        //[InputText(ConfirmDelegate = "[Event Unchecked] = [Action Rename($dataContext)]")]
        //public IDialogBoxPresenter OpenRename(IDialogBoxPresenter inputTextBoxPresenter)
        //{
        //    inputTextBoxPresenter.CanConfirmDelegate = text => CurrentEntity.Text != text;
        //    inputTextBoxPresenter.Text = CurrentEntity.Name;
        //    inputTextBoxPresenter.Message = string.Format("Enter a new name for '{0}'.", CurrentEntity.Name);
        //    return inputTextBoxPresenter;
        //}

        //public void Rename(IDialogBoxPresenter result)
        //{
        //    if (result.IsCancel == true) return;

        //    if (_model.DuplicationChecking(CurrentEntity, result.Text))
        //    {
        //        result.ConfrimCallback(new Exception("duplication of name"));
        //    }
        //    else
        //    {
        //        CurrentEntity.Name = result.Text;
        //        Save();
        //        result.Close();
        //    }
        //}
    }

}
