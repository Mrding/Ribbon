using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Core;
using System.Collections;
using Caliburn.PresentationFramework.ApplicationModel;
using Microsoft.Practices.ServiceLocation;
using Luna.Common;
using System.Windows.Data;
using System.ComponentModel;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class PresenterExt
    {
        public static void QuietlyReload<T>(this PropertyChangedBase presenter, ref  IEnumerable<T> list, string propertyName)
        {
            var temp = list;
            list = null;
            presenter.NotifyOfPropertyChange(propertyName);
            list = temp;
            presenter.NotifyOfPropertyChange(propertyName);
        }
        
        public static void QuietlyReload(this PropertyChangedBase presenter, ref  IList<IEnumerable> list, string propertyName)
        {
            var temp = list;
            list = new List<IEnumerable>();
            presenter.NotifyOfPropertyChange(propertyName);
            list = temp;
            presenter.NotifyOfPropertyChange(propertyName);
        }

        public static void QuietlyReload(this PropertyChangedBase presenter, ref  IList list, string propertyName)
        {
            
            var temp = list;
            list = new List<IEnumerable>();
            presenter.NotifyOfPropertyChange(propertyName);
            list = temp;
            presenter.NotifyOfPropertyChange(propertyName);
        }

        public static void QuietlyReload<T>(this PropertyChangedBase presenter, ref  IList<T> list, string propertyName)
        {

            var temp = list;
            list = new List<T>();
            presenter.NotifyOfPropertyChange(propertyName);
            list = temp;
            presenter.NotifyOfPropertyChange(propertyName);
        }

        public static void QuietlyReload<T>(this PropertyChangedBase presenter, ref  ICollectionView list, string propertyName) 
        {

            var temp = list;
            list = new ListCollectionView(new List<T>());
            presenter.NotifyOfPropertyChange(propertyName);
            list = temp;
            presenter.NotifyOfPropertyChange(propertyName);
        }

        public static T CreateDetailPresenter<T>(this PresenterBase presenter,object Item,bool IsInitialize)
        {
            var entityFactory = ServiceLocator.Current.GetInstance<IEntityFactory>();
            var currentPresenter = entityFactory.Create<T>(new Dictionary<string, object> { { "Entity", Item } });
            if (IsInitialize)
                (currentPresenter as PresenterBase).Initialize();
            return currentPresenter;
        }
    }
}
