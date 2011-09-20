using System;
namespace Luna.WPF.ApplicationFramework
{
    public interface ICommandSetPresenter
    {
        void Fire(string key, object p);

        void Register(string key, MulticastDelegate delegateMethod);
    }

    public static class CommandSetPresenterExt
    {
        public static void Show(this ICommandSetPresenter presenter)
        {
            var dockablePresenter = presenter as IDockablePresenter;
            if(dockablePresenter != null)
                dockablePresenter.Show();
        }
    }
}