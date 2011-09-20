using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ActiproSoftware.Windows.Controls.Wizard;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Metadata;
using Luna.Common;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;

using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Attributes;
using System.Windows.Controls;
using Luna.Core.Extensions;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IBatchDispatcherPresenter))]
    public class BatchDispatcherPresenter : DefaultPresenter, IBatchDispatcherPresenter
    {
        private string _comments;
        private Action<IBatchAlterModel, ObservableCollection<object>> _dispatchDelegate;

        public BatchDispatcherPresenter()
        {
            var args = new Dictionary<string, object> { { "watchPoint", base.GetWatchPoint() } };

            AssignmentAlterModel = Container.Resolve<IAssignmentBatchAlterModel>(args);
            EventAlterModel = Container.Resolve<IEventBatchAlterModel>(args);
            AddingEventModel = Container.Resolve<IBatchAddingEventModel>(args);
            AddingShiftModel = Container.Resolve<IBatchAddingShiftModel>(args);

            Patterns = new IBatchAlterModel[] { AssignmentAlterModel, 
                                                EventAlterModel,
                                                AddingEventModel,
                                                AddingShiftModel
                                              };
        }

        public IEnumerable<IBatchAlterModel> Patterns { get; private set; }

        public IAssignmentBatchAlterModel AssignmentAlterModel { get; private set; }

        public IEventBatchAlterModel EventAlterModel { get; private set; }

        public IBatchAddingShiftModel AddingShiftModel { get; private set; }

        public IBatchAddingEventModel AddingEventModel { get; private set; }

        

        public Func<bool?, IEnumerable> GetSelectedAgents { get; set; }

        public Action<IEnumerable, bool> RefreshDelegate { get; set; }

        protected override void OnInitialize()
        {
            foreach (var model in Patterns)
                model.Initial();

            base.OnInitialize();
        }
        public override void AddMetadata(IMetadata metadata)
        {
            if (metadata is ViewMetadata) return;
            base.AddMetadata(metadata);
        }


        public void CheckActionSetting(ObservableCollection<object> optinalActions, WizardSelectedPageChangeEventArgs e)
        {
            if ((e.SelectionFlags & WizardPageSelectionFlags.BackwardProgress) == WizardPageSelectionFlags.BackwardProgress)
                return;
            e.Cancel = optinalActions.Count == 0;
        }


        public void DispatchSubEvent(FrameworkElement page)
        {
            var filterList = (ListBox)page.FindName("EventFilterList");
            var optionalFiltersList = (ListBox)page.FindName("OptionalEventFilterList");

            _dispatchDelegate = (model, optinalActions) =>
                                    {
                                        var filter = filterList.SelectedValue as Func<Term, bool>;
                                        var optionalFilters = optionalFiltersList.SelectedItems;
                                        Func<Term, bool> where = t =>
                                                           {
                                                               if (!filter(t)) return false;
                                                               foreach (var func in optionalFilters.Cast<ICustomFilter>().Select(o => o.WhereClause).OfType<Func<Term, bool>>())
                                                                   if (!func(t)) return false;
                                                               return true;
                                                           };

                                        var action = default(Action<Term, TimeBox>);
                                        foreach (var act in optinalActions.Cast<ICustomAction>().Select(o => o.Action).OfType<Action<Term, TimeBox>>())
                                            action += act;
                                        TryDispatch(model, where, action);
                                    };
        }


        public void DispatchAssignment(FrameworkElement page)
        {
            var filterList = (ListBox)page.FindName("AssignmentFilterList");
            _dispatchDelegate = (model, optinalActions) =>
                                    {
                                        var filter = filterList.SelectedValue as Func<Term, bool>;
                                        var action = default(Action<Term, TimeBox>);
                                        foreach (var act in optinalActions.Cast<ICustomAction>().Select(o => o.Action).OfType<Action<Term, TimeBox>>())
                                            action += act;
                                        TryDispatch(model, filter, action);
                                    };
        }


        public void InsertAssignment(IBatchAlterModel model)
        {
            TryDispatch(model, t => true, (t, tb) => { });
        }


        public void InsertSubEvent(IBatchAddingEventModel model, TimeSpan timeSpan)
        {
            var targetAssignmentTypes = model.AssignmentTypes
                                             .OfType<TermStyle>().Where(o => o.SaftyGetProperty<bool?, ISelectable>(x => x.IsSelected) == true)
                                             .ToArray();
            var noAssignmentTypesSelected = targetAssignmentTypes.Count() == 0;

            TryDispatch(model, t => noAssignmentTypesSelected ||
                                    targetAssignmentTypes.Any(o => o.Text == t.Text), model.GetDefaultAction(timeSpan));
        }

        private void TryDispatch(IBatchAlterModel model, Func<Term, bool> filter, Action<Term, TimeBox> action)
        {
            var applied = false;
            model.OnDispatching();

            var agents = GetSelectedAgents(true);

            foreach (IAgent item in agents)
            {
                var agent = item;

                model.TryDispatch(agent, filter, action, (t, text, success) =>
                                                         {
                                                             if (success)
                                                             {
                                                                 t.SaftyInvoke<Term>(x => x.Tag = _comments);
                                                                 applied = true;
                                                             }
                                                             agent.OperationFail = !success;
                                                         });
            }
            if (RefreshDelegate != null)
                RefreshDelegate.Invoke(agents, applied);
        }

        public void Apply(IBatchAlterModel model, ObservableCollection<object> optinalActions)
        {
            if (_dispatchDelegate != null)
                _dispatchDelegate(model, optinalActions);
        }

        public void Comment(string text)
        {
            _comments = text;
        }

        public void CancelEdit()
        {
        }

        protected override void OnShutdown()
        {
            Invoker = null;
            foreach (var batchAlterModel in Patterns)
                batchAlterModel.TearDown();

            base.OnShutdown();
        }
    }
}
