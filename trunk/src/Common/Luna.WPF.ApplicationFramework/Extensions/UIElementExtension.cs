using System;
using System.Collections;
using System.Linq;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class UIElementExtension
    {
        //public static void RegisterRefreshDelegate(this IPresenter presenter, object source, string method, ref Action<IEnumerable> @delegate)
        //{
        //    var delegateMethod = source as Action<IEnumerable>;

        //    if (source is UIElement)
        //    {
        //        delegateMethod = new Action<IEnumerable>(list => ((UIElement)source).InvalidateVisual());
        //        var refreshLayerMethodInfo = source.GetType().GetMethod(method);

        //        if (refreshLayerMethodInfo != null)
        //            new Action<IEnumerable>((list) => refreshLayerMethodInfo.Invoke(source, null)).AppendTo(ref @delegate);
        //    }
        //    delegateMethod.AppendTo(ref @delegate);
        //}

        public static void AppendTo(this Action<IEnumerable> @delegate , ref Action<IEnumerable> action)
        {
            if(@delegate == default(Action<IEnumerable>)) return;
            
                if (action == null)
                    action = @delegate;
                else if (!action.GetInvocationList().Contains(@delegate))
                    action += @delegate;
        }
    }
}
