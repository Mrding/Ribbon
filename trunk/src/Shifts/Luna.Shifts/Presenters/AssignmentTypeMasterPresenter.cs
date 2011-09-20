using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Interfaces;
using System.Collections;

namespace Luna.Shifts.Presenters
{
    [PerRequest(FunctionKeys.ManageAssignmentType, typeof(IPresenterManager))]
    public class AssignmentTypeMasterPresenter : MasterPresenter<IAssignmentTypeDetailPresenter, BasicAssignmentType>,
        IAssignmentTypeMasterPresenter
    {
        private readonly IAssignmentTypeModel _assignmentTypeModel;

        private readonly IBlockConverter _termStyleBlockConverter = new TermStyleBlockConverter();
        private readonly List<Type> _avaliableEntityTypes;
        private readonly List<Type> _implementationTypes;
        private List<BasicAssignmentType> _basicAssignmentTypeList;

        public AssignmentTypeMasterPresenter(IEntityFactory entityFactory, IAssignmentTypeModel model)
            : base(entityFactory, model)
        {
            _assignmentTypeModel = model;
            _avaliableEntityTypes = new List<Type> { typeof(BasicAssignmentType), typeof(AssignmentType) };
            _implementationTypes = new List<Type> { typeof(Assignment), typeof(OvertimeAssignment) };
            _basicAssignmentTypeList = new List<BasicAssignmentType>(50) { new BasicAssignmentType() }; // 为了能让此列表有个NULL选项,特意加上了空的对象

            SettingCategories = new List<string> { "General", "Rules", "Region", "Arrangemnt" };
        }

        protected override void OnInitialize()
        {
            InitialDetailPresenters(_assignmentTypeModel.GetAllAssignmentTypes(), null, t => t);
            ServiceQueuesSource = CollectionViewSource.GetDefaultView(_assignmentTypeModel.GetAllServiceQueues()
                                        .Self(queues => queues.Add(new ServiceQueue())));// 为了能让此列表有个NULL选项,特意加上了空的对象

            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Entity.Purpose"));
            base.OnInitialize();
        }

        protected override void LoadPresenters(IAssignmentTypeDetailPresenter presenter)
        {
            if (presenter.Entity.IsNot<AssignmentType>())
                _basicAssignmentTypeList.Add(presenter.Entity);
            
            base.LoadPresenters(presenter);
        }

        /// <summary>
        /// 智能感知配對基礎班型
        /// </summary>
        public IList BasicAssignmentTypes1 { get { return _basicAssignmentTypeList; } }

        public ICollectionView AssignmentTypeCategories { get { return CollectionViewSource.GetDefaultView(_avaliableEntityTypes); } }

        public ICollectionView ImplementationTypes { get { return CollectionViewSource.GetDefaultView(_implementationTypes); } }


        public IList<string> SettingCategories { get; private set; }


        public bool CanMakeChangeForCurrentPresenter()
        {
            return CurrentPresenter != null;
        }

        [Preview("CanMakeChangeForCurrentPresenter")]
        [Dependencies("CurrentPresenter")]
        [InputText(ConfirmDelegate = "[Event Unchecked] = [Action Rename($dataContext)]")]
        public IDialogBoxPresenter OpenRename(IDialogBoxPresenter dialog)
        {
            dialog.BackgroundTitle = "Rename";
            dialog.CanConfirmDelegate = text => CurrentEntity.Text != text;
            dialog.Text = CurrentEntity.Name;
            dialog.Message = string.Format("Enter a new name for '{0}'.", CurrentEntity.Name);
            return dialog;
        }

        public void Rename(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;

            if (_assignmentTypeModel.DuplicationChecking(CurrentEntity, result.Text))
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

        [InputText(PartView = "Shifts.NewAssignmentTypeDialog", ConfirmDelegate = "[Event Unchecked] = [Action CreateNew($dataContext)]")]
        public IDialogBoxPresenter OpenNew(IDialogBoxPresenter dialog)
        {
            dialog.BackgroundTitle = "New";
            dialog.Message = string.Format("Select the category and enter a new for the new type");
            return dialog;
        }

        public void CreateNew(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;

            var newEntity = _entityFactory.Create<TermStyle>((Type)AssignmentTypeCategories.CurrentItem);

            if (_assignmentTypeModel.DuplicationChecking(newEntity, result.Text))
            {
                result.ConfrimCallback(new Exception("duplication of name"));
            }
            else
            {
                newEntity.Name = result.Text;
                newEntity.Type = (Type)ImplementationTypes.CurrentItem;
                newEntity.SaftyInvoke<AssignmentType>(o =>
                                                          {
                                                              o.TimeZone = TimeZoneInfo.Local;
                                                              o.Country = Country.Local;
                                                          });
                var newPresenter = _entityFactory.Create<IAssignmentTypeDetailPresenter>(new Dictionary<string, object> { { "Entity", newEntity } });

                AddToPresenters(newPresenter);
                Save();
                result.Close();
            }
        }

        [Preview("CanMakeChangeForCurrentPresenter")]
        [Dependencies("CurrentPresenter")]
        [InputText(PartView = "Shifts.NewAssignmentTypeDialog", ConfirmDelegate = "[Event Unchecked] = [Action SaveAsNew($dataContext)]")]
        public IDialogBoxPresenter OpenSaveAsNew(IDialogBoxPresenter dialog)
        {
            dialog.BackgroundTitle = "SaveAs";
            dialog.Message = string.Format("Enter a name for the duplicate of '{0}'", CurrentEntity.Name);
            dialog.Text = string.Format("{0}1", CurrentEntity.Name);
            return dialog;
        }

        public void SaveAsNew(IDialogBoxPresenter result)
        {
            if (result.IsCancel == true) return;

            var selectedType = AssignmentTypeCategories.CurrentItem.As<Type>();
            var source = CurrentEntity;

            if (_assignmentTypeModel.DuplicationChecking(source, result.Text))
            {
                result.ConfrimCallback(new Exception("duplication of name"));
            }
            else
            {
                var newEntity = _assignmentTypeModel.SaveAsNew(result.Text, selectedType, ref source);
                var newPresenter = _entityFactory.Create<IAssignmentTypeDetailPresenter>(new Dictionary<string, object> { { "Entity", newEntity } });
                Cancel();
                AddToPresenters(newPresenter);
                Save();

                result.Close();
            }
        }

        protected override void OnSave(BasicAssignmentType entity)
        {
            base.OnSave(entity);

            if (entity.IsNot<AssignmentType>() && !_basicAssignmentTypeList.Contains(entity))
            {
                _basicAssignmentTypeList.Add(entity);
                _basicAssignmentTypeList = new List<BasicAssignmentType>(_basicAssignmentTypeList);
                NotifyOfPropertyChange(() => BasicAssignmentTypes1);
            }
        }

        protected override void OnDelete(BasicAssignmentType entity)
        {
            base.OnDelete(entity);
            if (entity.IsNot<AssignmentType>())
            {
                _basicAssignmentTypeList.Remove(entity);
                _basicAssignmentTypeList = new List<BasicAssignmentType>(_basicAssignmentTypeList);
                NotifyOfPropertyChange(() => BasicAssignmentTypes1);
            }
        }

        protected override void OnCancelEdit(IAssignmentTypeDetailPresenter presenter)
        {
            var entityRef = presenter.Entity;
            _assignmentTypeModel.Reload(ref entityRef);
            presenter.Entity = entityRef;
        }


        public IBlockConverter BlockConverter { get { return _termStyleBlockConverter; } }

        public ICollectionView ServiceQueuesSource { get; private set; }

        [SuperRescue("HandleDeleteException", "Activate fail", "ApplicationFramework_MenuPresenter_ExDialogTitle")]
        [Preview("CanMakeChangeForCurrentPresenter")]
        [Dependencies("CurrentPresenter")]
        public void ChangeInUseStatus()
        {
            if (!CurrentEntity.InUse && _assignmentTypeModel.DuplicationChecking(CurrentEntity, CurrentEntity.Name))
            {
                throw new Exception(string.Format("Unable activate type '{0}' which name is already been used.", CurrentEntity.Name));
            }
            CurrentEntity.InUse = !CurrentEntity.InUse;
            Save();
        }

        protected override void OnShutdown()
        {
            _termStyleBlockConverter.Dispose();
            _assignmentTypeModel.Release();
            _avaliableEntityTypes.Clear();
            _implementationTypes.Clear();
            _basicAssignmentTypeList.Clear();
            ServiceQueuesSource = null;
            SettingCategories = null;


            base.OnShutdown();
        }

    }
}
