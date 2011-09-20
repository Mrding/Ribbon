using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using Luna.WPF.ApplicationFramework;
using Caliburn.Core.Metadata;
using Luna.Shifts.Presenters.Interfaces;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IRTAAPrefixPresenter))]
    public partial class RTAAPrefixPresenter : DockingPresenter, IRTAAPrefixPresenter
    {
        private readonly IRealTimeAdherenceModel _model;

        private IList<AdherenceEvent> _newAddedSet;
        private IList<AdherenceEvent> _removedSet;

        public RTAAPrefixPresenter(IRealTimeAdherenceModel model)
        {
            _model = model;
            _newAddedSet = new List<AdherenceEvent>(50);
            _removedSet = new List<AdherenceEvent>(50);
            WhenAdding = o =>
                             {
                                 IsDirty = true;
                                 var e = new AdherenceEvent
                                             {
                                                 Text = o.Text,
                                                 Start = o.Start.RemoveSeconds(),
                                                 End = o.End.Second == 0 ? o.End : o.End.AddSeconds(60 - o.End.Second),
                                                 Remark = "added",
                                                 Reason = SelectedAbsence
                                             };

                                 _newAddedSet.Add(e);
                                 return e;
                             };
            WhenChanged = o =>
                              {
                                  IsDirty = true;
                              };
            WhenRemoving = o =>
                               {
                                   IsDirty = true;
                                   _newAddedSet.Remove(o);
                                   _removedSet.Add(o);
                               };
        }

        protected override void OnInitialize()
        {
            CloseWithInvoker(true);
            Reload(_model.GetAgentAdherenceEvents);
            AbsenceTypes = _model.GetAbsenceTypes();
            Invoker.SaftyInvoke<INotifyPropertyChanged>(p => p.PropertyChanged += InvokerPropertyChanged);

            base.OnInitialize();
        }

        private void InvokerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var invoker = new Luna.Core.Reflector(sender);

            if (e.PropertyName == "AgentAdherences" && invoker.Property<IList<IEnumerable>>("AgentAdherences").Count > 0)
            {
                //if (invoker.Property<bool>("RtaaIsAutoRunning")) return;
                Refresh();
            }

            if (e.PropertyName == "EnableRtaa" && sender.SaftyGetProperty<bool, IShiftDispatcherPresenter>(p => p.EnableRtaa == false))
            {
                this.Close();
                sender.SaftyInvoke<INotifyPropertyChanged>(p => p.PropertyChanged -= InvokerPropertyChanged);
            }
        }

        public string SelectedAbsence { get; set; }

        public IList<string> AbsenceTypes { get; set; }

        public DateTime MonitoringPoint { get; set; }

        public Action<Func<DateTime, DateTime, IDictionary<Guid, IEnumerable>>> Reload { get; set; }

        public Func<IVisibleTerm, AdherenceEvent> WhenAdding { get; private set; }

        public Action<AdherenceEvent> WhenRemoving { get; private set; }

        public Action<IRTAAPrefixPresenter, Exception> WhenClosed { get; set; }

        public Action<AdherenceEvent> WhenChanged { get; private set; }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty == value) return;
                _isDirty = value; NotifyOfPropertyChange(() => IsDirty);
            }
        }

        public void Refresh()
        {
            Apply();
            Reload(_model.GetAgentAdherenceEvents);
        }

        public void Apply()
        {
            if (IsDirty)
            {
                IsDirty = false;
                if (_newAddedSet != null && _newAddedSet.Count > 0)
                {
                    _model.AddAdherenceEvents(_newAddedSet);
                    _newAddedSet.Clear();
                    return;
                }

                if (_removedSet != null && _removedSet.Count > 0)
                {
                    _model.RemoveAdherenceEvents(_removedSet);
                    _removedSet.Clear();
                    return;
                }
                _model.AlterAdherenceEvents();
            }
        }

        protected override void OnShutdown()
        {
            Apply();
            _removedSet.Clear();
            _newAddedSet.Clear();
            WhenClosed(this, null);
            Invoker.SaftyInvoke<INotifyPropertyChanged>(p => p.PropertyChanged -= InvokerPropertyChanged);
            base.OnShutdown();
        }



    }
}
