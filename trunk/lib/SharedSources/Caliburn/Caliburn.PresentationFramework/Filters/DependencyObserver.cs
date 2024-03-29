using Caliburn.Core.MemoryManagement;

namespace Caliburn.PresentationFramework.Filters
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core;
    using Core.Metadata;
    using Core.Invocation;

    /// <summary>
    /// Metadata which can be used to trigger availability changes in triggers based on <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class DependencyObserver : IMetadata
    {
        private readonly IRoutedMessageHandler _messageHandler;
        private readonly IMethodFactory _methodFactory;
        private readonly WeakReference<INotifyPropertyChanged> _weakNotifier;
        private readonly IDictionary<string, MonitoringInfo> _monitoringInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObserver"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="notifier">The notifier.</param>
        public DependencyObserver(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier)
        {
            _messageHandler = messageHandler;
            _methodFactory = methodFactory;
            _weakNotifier = new WeakReference<INotifyPropertyChanged>(notifier);
            _monitoringInfos = new Dictionary<string, MonitoringInfo>();
        }

        /// <summary>
        /// Makes the metadata aware of the relationship between an <see cref="IMessageTrigger"/> and its dependencies.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="dependencies">The dependencies.</param>
        public void MakeAwareOf(IMessageTrigger trigger, IEnumerable<string> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                var info = GetMonitoringInfos(dependency);
                info.RegisterTrigger(trigger);
            }
        }

        private MonitoringInfo GetMonitoringInfos(string propertyPath)
        {
            MonitoringInfo info;

            if (!_monitoringInfos.TryGetValue(propertyPath, out info))
            {
                info = new MonitoringInfo(_messageHandler, _methodFactory, _weakNotifier.Target, propertyPath);
                _monitoringInfos[propertyPath] = info;
            }

            return info;
        }

        private class MonitoringInfo
        {
            private readonly IRoutedMessageHandler _messageHandler;
            private readonly IList<IMessageTrigger> _triggersToNotify = new List<IMessageTrigger>();

            public MonitoringInfo(IRoutedMessageHandler messageHandler, IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath)
            {
                _messageHandler = messageHandler;
                new PropertyPathMonitor(methodFactory, notifier, propertyPath, OnPathChanged);
            }

            public void RegisterTrigger(IMessageTrigger trigger)
            {
                if (!_triggersToNotify.Contains(trigger))
                    _triggersToNotify.Add(trigger);
            }

            private void OnPathChanged()
            {
                _triggersToNotify.Apply(x => _messageHandler.UpdateAvailability(x));
            }
        }
    }
}