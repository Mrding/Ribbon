using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// Provide the Initialize method for some component
    /// </summary>
    public interface IInitialize
    {
        void Initialize();
    }

    public abstract class ControllableFrameworkElement : FrameworkElement
    {
        internal abstract void Initialize();
    }

    //public interface IControlCycle : IInitialize, IDisposable
    //{
    //}
}
