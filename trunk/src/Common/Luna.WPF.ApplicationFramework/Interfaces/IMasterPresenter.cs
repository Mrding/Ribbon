namespace Luna.WPF.ApplicationFramework.Interfaces
{
    using Caliburn.PresentationFramework.ApplicationModel;

    using Luna.Common.Interfaces;

    public interface IMasterPresenter<TIDetailPresenter, TEntity> : IPresenterManager
        where TIDetailPresenter : IDetailPresenter<TEntity>
        where TEntity : class
    {

        // ???
        //IModel<TEntity> Model { get; }

        TIDetailPresenter CurrentDetailPresenter { get; }

        //  T GetEntityInterface<T>();

        TEntity CurrentEntity { get; }

        //void OnBeginEdit();

        //void OnCancelEdit();

        //void OnEndEdit();

        // ???
        void Save();
        void Delete();
        void New();
        void Cancel();
    }

    // ???
    //public interface IPresenterModel
    //{
    //    void Save();
    //    void Delete();
    //    void New();
    //    void Cancel();
    //}

    public interface INotifyMetadata
    {
        void Notify();
    }
}
