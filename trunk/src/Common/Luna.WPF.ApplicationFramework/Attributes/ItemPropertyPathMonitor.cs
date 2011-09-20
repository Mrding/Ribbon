using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Caliburn.Core;
using Caliburn.Core.Invocation;
using Caliburn.Core.MemoryManagement;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    /// <summary>
    /// A class used to monitor changes in a property path.
    /// </summary>
    internal class ItemPropertyPathMonitor : IDisposable
    {
        private const string ALL_PROPERTIES = "*";

        private readonly IMethodFactory _methodFactory;
        private WeakReference<INotifyPropertyChanged> _notifier;
        private Action _notifyOfChange;

        private readonly string _propertyPath;
        private readonly string _observedPropertyName;
        private IMethod _propertyGetMethod;

        private readonly string _subPath;
        private List<ItemPropertyPathMonitor> _subPathMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPathMonitor"/> class.
        /// </summary>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="notifier">The notifier.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="onPathChanged">The on path changed.</param>
        public ItemPropertyPathMonitor(IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath, Action onPathChanged)
        {
            _methodFactory = methodFactory;
            _notifier = new WeakReference<INotifyPropertyChanged>(notifier);
            _notifyOfChange = onPathChanged;
            _propertyPath = propertyPath;
            _observedPropertyName = GetRootProperty(_propertyPath);
            _subPath = GetSubPath(_propertyPath);

            notifier.PropertyChanged += NotifierPropertyChanged;
            _subPathMonitor = new List<ItemPropertyPathMonitor>();
            HookSubpathMonitor();
        }

        private void HookSubpathMonitor()
        {
            if (_subPathMonitor.Count > 0)
            {
                foreach (IDisposable monitor in _subPathMonitor)
                    monitor.Dispose();
            }

            if (string.IsNullOrEmpty(_subPath))
                return;

            var getter = GetPropertyGetMethod();

            var subTarget = getter.Invoke(GetTargetOrFail());

            if (subTarget is IEnumerable)
            {
                if (_subPathMonitor.Count == 0)
                {
                    foreach (var item in (IEnumerable)subTarget)
                        if (item is INotifyPropertyChanged)
                            _subPathMonitor.Add(new ItemPropertyPathMonitor(_methodFactory, (INotifyPropertyChanged)item, _subPath, Notify));
                }
                else
                    Dispose();
            }
            else if (subTarget is INotifyPropertyChanged)
            {
                _subPathMonitor.Add(new ItemPropertyPathMonitor(_methodFactory, (INotifyPropertyChanged)subTarget, _subPath, Notify));
            }
        }

        private void NotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ShouldNotify(e.PropertyName))
                Notify();

            HookSubpathMonitor();
        }

        private object GetTargetOrFail()
        {
            if (!_notifier.IsAlive)
                throw new CaliburnException("Target is no longer available.");

            return _notifier.Target;
        }

        private bool ShouldNotify(string propertyName)
        {
            return _observedPropertyName.Equals(propertyName) || _observedPropertyName == ALL_PROPERTIES;
        }

        private void Notify()
        {
            if (_notifyOfChange != null)
                _notifyOfChange();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_notifier.IsAlive)
                _notifier.Target.PropertyChanged -= NotifierPropertyChanged;
            _notifyOfChange = null;
            _notifier = null;
            _propertyGetMethod = null;
            if (_subPathMonitor != null)
            {
                _subPathMonitor.Clear();
                _subPathMonitor = null;
            }

            if (_subPathMonitor != null)
                foreach (var monitor in _subPathMonitor)
                    monitor.Dispose();
        }

        private IMethod GetPropertyGetMethod()
        {
            if (_propertyGetMethod == null)
            {
                if (_observedPropertyName.Equals(ALL_PROPERTIES))
                    throw new CaliburnException(
                        string.Format(
                            "'{0}' marker in path {1} is invalid. '{0}' can only be used as leaf property in a path.", ALL_PROPERTIES, _propertyPath));

                var type = GetTargetOrFail().GetType();
                var propInfo = type.GetProperty(_observedPropertyName, BindingFlags.Instance | BindingFlags.Public);

                if (propInfo == null)
                {
                    throw new CaliburnException(
                        string.Format("Cannot find property {0} of path {1} in class {2}.",
                                      _observedPropertyName,
                                      _propertyPath,
                                      type.FullName
                            )
                        );
                }

                _propertyGetMethod = _methodFactory.CreateFrom(propInfo.GetGetMethod());
            }

            return _propertyGetMethod;
        }

        private static string GetRootProperty(string propertyPath)
        {
            var index = propertyPath.IndexOf(".");
            if (index < 0) index = propertyPath.Length;

            return propertyPath.Substring(0, index);
        }

        private static string GetSubPath(string propertyPath)
        {
            var index = propertyPath.IndexOf(".");
            if (index < 0 || index >= propertyPath.Length) return null;

            return propertyPath.Substring(index + 1);
        }
    }
}
