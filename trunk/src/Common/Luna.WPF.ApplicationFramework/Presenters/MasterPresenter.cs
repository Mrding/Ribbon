using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Model;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Attributes;
using Luna.WPF.ApplicationFramework.Interfaces;
using Luna.WPF.ApplicationFramework.Threads;

namespace Luna.WPF.ApplicationFramework
{
    public abstract class MasterPresenter<TIDetailPresenter, TEntity> : MultiPresenterManager, IMasterPresenter<TIDetailPresenter, TEntity>
        where TIDetailPresenter : IDetailPresenter<TEntity>
        where TEntity : Entity
    {
        protected readonly IEntityFactory _entityFactory;
        private readonly IModel<TEntity> _model;

        protected MasterPresenter(IEntityFactory entityFactory, IModel<TEntity> model)
        {
            _entityFactory = entityFactory;
            _model = model;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitialize()
        {
            CollectionView.CurrentChanging += OnCurrentChanging;

            UIThread.BeginInvoke(() =>
            {
                if (CollectionView.CurrentItem == null)
                    CollectionView.MoveCurrentToFirst();
            });

            base.OnInitialize();
        }

        protected void InitialDetailPresenters(IEnumerable<TEntity> exists, Dictionary<string, object> args, Func<TEntity, TEntity> looping)
        {
            var @params = args ?? new Dictionary<string, object>();

            Presenters.SupendOnCollectionChangedEvent(true);
            foreach (var item in exists)
            {
                @params["Entity"] = looping(item);
                LoadPresenters(_entityFactory.Create<TIDetailPresenter>(@params).Self(p => p.Initialize()));
            }
            Presenters.SupendOnCollectionChangedEvent(false);
            @params.Remove("Entity");
        }

        /// <summary>
        /// 加载Presenter，只在初始化OnInitialize()时候调用
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        protected virtual void LoadPresenters(TIDetailPresenter presenter)
        {
            Presenters.Add(presenter);
        }

        private ICollectionView _defaultPresenterView;
        public ICollectionView CollectionView
        {
            get
            {
                if (_defaultPresenterView == null)
                    _defaultPresenterView = CollectionViewSource.GetDefaultView(Presenters);
                return _defaultPresenterView;
            }
        }

        public IEditingObject CurrentEditingObject
        {
            get { return ReferenceEquals(CurrentEntity, default(TEntity)) ? default(IEditingObject) : (IEditingObject)CurrentEntity; }
        }

        /// <summary>
        /// 得到Detail的数据模板。界面必须使用Binding Mode=OneTime，这样性能最高
        /// </summary>
        /// <value>The template.</value>
        public DataTemplate Template
        {
            get
            {
                var detailPresenterType = typeof(TIDetailPresenter);

                var viewName = detailPresenterType.Name.Substring(1, typeof(TIDetailPresenter).Name.IndexOf("Presenter") - 1) + "View";
                var modelName = detailPresenterType.Namespace.Split('.')[1];
                var fullName = String.Format("Luna.GUI.Views.{0}.{1}, {2}", modelName, viewName, Assembly.GetEntryAssembly().FullName);
                var view = Type.GetType(fullName);
                var element = new FrameworkElementFactory(view);
                return new DataTemplate { VisualTree = element };
            }
        }

        /// <summary>
        /// 当前选中的DetailPresenter
        /// </summary>
        /// <value>The current detail presenter.</value>
        public TIDetailPresenter CurrentDetailPresenter
        {
            get { return CollectionView.CurrentItem.As<TIDetailPresenter>(); }
        }

        /// <summary>
        /// 当前操作的实体
        /// </summary>
        /// <value>The current entity.</value>
        public TEntity CurrentEntity
        {
            get { return ReferenceEquals(CurrentDetailPresenter, default(TEntity)) ? default(TEntity) : CurrentDetailPresenter.Entity; }
        }

        /// <summary>
        /// 取消时调用
        /// </summary>
        [Preview("CanCancel")]
        [Dependencies("CurrentPresenter")]
        public void Cancel()
        {
            //CurrentEditingObject不处于编辑状态
            CurrentEditingObject.IsEditing = false;
            CurrentEditingObject.IsEnablePropertyChanged = false;


            //取消编辑当前DetailPresenter
            OnCancelEdit(CurrentDetailPresenter);

            //CurrentEditingObject开始编辑
            //CurrentEditingObject.BeginEdit();

            NotifyOfPropertyChange(() => CurrentPresenter);
        }

        protected virtual void OnCancelEdit(TIDetailPresenter presenter)
        {
        }

        public object BeforeDelete(IQuestionPresenter question)
        {
            if (CurrentEditingObject.IsNew) return true; // 因为是new object 所以无需做删除提示

            question.DisplayName = this.GetType().Name;
            question.SavingConfirmModeOn = false;
            question.Text = string.Format("Do you want to delete {0}", CurrentEntity);

            return question;
        }

        /// <summary>
        /// 删除时调用
        /// </summary>
        [Preview("CanDelete")]
        [Dependencies("CurrentPresenter")]
        [Question("BeforeDelete")]
        [SuperRescue("HandleDeleteException", "ApplicationFramework_MasterPresenter_DeleteException",
            "ApplicationFramework_MenuPresenter_ExDialogTitle")]
        public virtual void Delete()
        {
            //保存当前detail presenter 的引用
            var presenter = CurrentDetailPresenter;

            //如果要删除的presenter的实体不是新增的，那么就下delete命令，删除它
            if (!CurrentEditingObject.IsNew)
            {
                OnDelete(presenter.Entity);
            }

            //找到要删除的index
            var index = Presenters.IndexOf(presenter);
            //找到要激活的（置为当前页）的presenter
            var shouldActionPresenter = DetermineNextPresenterToActivate(index);
            //切换到要激活的presenter
            CollectionView.MoveCurrentTo(shouldActionPresenter);
            //关闭删除的presenter
            this.Shutdown(presenter);
        }

        /// <summary>
        /// 新增时调用。1：创建DetailPresenter。2：增加到Presenters集合中。3：设置当前Presenter为新增的。
        /// </summary>
        public void New()
        {
            //1：创建DetailPresenter。
            var newPresenter = CreateDetailPresenter();
            //OnCreated(newPresenter);
            //2：增加到Presenters集合中。
            AddToPresenters(newPresenter);
            //3：设置当前Presenter为新增的。
            CurrentPresenter = newPresenter;
        }

        /// <summary>
        /// 保存时调用
        /// </summary>
        //[UIValidation]
        [Preview("CanSave")]
        [Dependencies("CurrentPresenter")]
        [SuperRescue("HandelSaveException", "ApplicationFramework_MasterPresenter_SaveException", "ApplicationFramework_MenuPresenter_ExDialogTitle")]
        public virtual void Save()
        {
            Save(CurrentDetailPresenter);
        }

        public bool CanCancel()
        {
            return CurrentEditingObject != null && CurrentEditingObject.IsEditing;
        }

        public bool CanDelete()
        {
            return CurrentPresenter != null; //&& CanDelete(CurrentEntity);
        }

        public bool CanSave()
        {
            return CanSave(CurrentDetailPresenter);
        }

        public virtual bool DeleteCallback(IQuestionPresenter p)
        {
            return p.Answer == Answer.Yes;
        }

        public virtual void HandelSaveException(Exception ex)
        {
        }

        public virtual void HandleDeleteException(Exception ex)
        {
        }

        /// <summary>
        /// 另存为
        /// </summary>
        public void SaveAs()
        {
            var newPresenter = OnSaveAs();
            AddToPresenters(newPresenter);
        }

        /// <summary>
        /// 增加到Presenters集合，新增时调用
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        protected void AddToPresenters(TIDetailPresenter presenter)
        {
            if (presenter.Entity == null)
                presenter.Entity = _entityFactory.Create<TEntity>();

            //新增时，设置IsNew为true
            presenter.EditingObject.IsNew = true;
            //开始编辑presenter
            //xBeginEdit(presenter);
            //附加PropertyChanged事件，以便及时刷新UI
            //xpresenter.EditingObject.PropertyChanged += delegate { NotifyOfPropertyChange(() => CurrentPresenter); };
            //打开当前presenter
            this.Open(presenter);

            //将当前视图指向新增的presenter
            //CollectionView.MoveCurrentTo(presenter);
            //presenter.Entity.SaftyInvoke<ISelectable>(o=> o.IsSelected = true);
        }

        /*
        protected void BeginEdit(TIDetailPresenter presenter)
        {
            //OnBeginEdit(presenter);
            //presenter.EditingObject.BeginEdit();
            //presenter.EditingObject.IsEditing = false;
        }*/

        /*/// <summary>
        /// 初始化presenter,但并不添加到项中,供取消用
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        protected void BeginEditPresenter(TIDetailPresenter presenter)
        {
            if (presenter.Entity == null)
                presenter.Entity = _entityFactory.Create<TEntity>();

            //附加属性通知事件，及时刷新UI
            presenter.EditingObject.PropertyChanged += (s, e) => NotifyOfPropertyChange(() => CurrentPresenter);
            //开始编辑此presenter
            BeginEdit(presenter);
        }*/

        protected virtual bool CanDelete(TEntity entity)
        {
            return entity != null;
        }

        protected virtual bool CanSave(TIDetailPresenter detailPresenter)
        {
            if (ReferenceEquals(detailPresenter, default(TIDetailPresenter))) return false;

            var entity = detailPresenter.Entity as Entity;
            if (entity == null) return true;

            if (String.IsNullOrEmpty(entity.Name) || entity.Name.Trim().Length == 0)
                return false;

            return CurrentEditingObject.IsEditing;
        }

        /// <summary>
        /// 创建DetailPresenter，默认使用EntityFactory创建，可以重写来创建自己的DetailPresenter
        /// </summary>
        /// <returns></returns>
        protected virtual TIDetailPresenter CreateDetailPresenter()
        {
            return _entityFactory.Create<TIDetailPresenter>();
        }

        protected void EndEdit(TIDetailPresenter presenter)
        {
            //OnEndEdit(presenter);
            //presenter.EditingObject.EndEdit();
        }

        protected virtual void OnDelete(TEntity entity)
        {
            //if (entity.SaftyGetProperty<bool, IEditingObject>(o => o.IsNew))
            //    _model.Reload(ref entity);
            //else
            _model.Delete(entity);
        }

        private bool LeavSavingConfirm()
        {
            if (CurrentDetailPresenter.IsNull() || !CurrentEditingObject.IsEditing || CurrentEditingObject.IsNew) return true;

            CurrentPresenter = CurrentDetailPresenter; //move back (effect to listbox)

            var q = _entityFactory.Create<IQuestionPresenter>();

            q.ClosingConfirmModeOn = true;
            q.Text = string.Format("Do you want to save changes to {0}", CurrentEntity);
            q.DisplayName = GetType().Name;
            q.Invoker = this;
            q.ConfirmDelegate = "[Event Closing] = [Action CompleteConfirm($dataContext)]";

            _entityFactory.Create<IWindowManager>().ShowDialog(q);

            return q.Answer != Answer.Cancel;
        }

        public void OnCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            var movedPresenter = CurrentPresenter; // temp

            if (LeavSavingConfirm())
            {
                CurrentPresenter = movedPresenter;
                AttachOnEditNotification(CurrentPresenter ?? CurrentDetailPresenter);
                return;
            }

            if (e.IsCancelable)
                e.Cancel = true;
        }

        protected void AttachOnEditNotification(IPresenter presenter)
        {
            presenter.SaftyInvoke<TIDetailPresenter>(p =>
            {
                if (p.OnDirty == null)
                    p.OnDirty = () => NotifyOfPropertyChange(() => CurrentPresenter);
            });
        }

        public virtual void CompleteConfirm(IQuestionPresenter q)
        {
            if (q.Answer == Answer.Yes)
                Save();
            if (q.Answer == Answer.No)
                Cancel();
        }

        protected virtual void OnSave(TEntity entity)
        {
            _model.Save(entity);
        }

        protected virtual TIDetailPresenter OnSaveAs()
        {
            return default(TIDetailPresenter);
        }

        /// <summary>
        /// 保存指定的DetailPresenter
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        protected void Save(TIDetailPresenter presenter)
        {
            var editingObj = presenter.EditingObject;
            //关闭属性通知，以避免IsEditing = true
            editingObj.IsEnablePropertyChanged = false;
            //结束编辑
            EndEdit(presenter);
            try
            {
                //保存
                OnSave(presenter.Entity);
                //重置编辑和新增状态，此时IsEnablePropertyChanged为false，可以安全改变而没有事件通知
                editingObj.IsEditing = false;
                editingObj.IsNew = false;
            }
            catch
            {
                //editingObj.IsEditing = true;
#if DEBUG
                throw;
#endif
            }
            finally
            {
                //重新开始编辑
                //BeginEdit(presenter);
                //开启属性通知
                editingObj.IsEnablePropertyChanged = true;
                //刷新UI
                NotifyOfPropertyChange(() => CurrentPresenter);
            }
        }

        public override bool CanShutdown()
        {
            return LeavSavingConfirm();
        }

        protected override void OnShutdown()
        {
            var disposableModel = _model as IDisposable;
            if (disposableModel != null)
                disposableModel.Dispose();

            CollectionView.CurrentChanging -= OnCurrentChanging;

      
            
            
            base.OnShutdown();


        }
    }
}