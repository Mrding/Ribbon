using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Metadata;
using Luna.Core.Extensions;
using Luna.Common;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.Infrastructure.Presenters.Interfaces;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;

namespace Luna.Infrastructure.Presenters
{
    [PerRequest(typeof(IAgentFinderPresenter))]
    public class AgentFinderPresenter : DefaultPresenter, IAgentFinderPresenter
    {
        private IAgentFinderModel _model;
        private IEnumerable<ICustomFilter> _filters;
        private IEnumerable _agents;
        private ICanSupportAgentFinder _canSupportAgentFinderPresenter;
        private Func<object, Employee> _cast;

        public AgentFinderPresenter(IAgentFinderModel model)
        {
            _model = model;
        }

        protected override void OnInitialize()
        {
            Invoker.SaftyInvoke<ICanSupportAgentFinder>(p =>
            {
                _canSupportAgentFinderPresenter = p;
                _cast = p.Transform<Employee>;
                _agents = p.Agents;
                _model.CreateTimeBoxFilter(_agents, p.GetWatchPoint().Date);
            });
            _filters = _model.GetFilters();

            base.OnInitialize();
        }

        public override void AddMetadata(IMetadata metadata)
        {
            if (metadata is ViewMetadata) return;
            base.AddMetadata(metadata);
        }

        public IEnumerable<ICustomFilter> Filters
        {
            get { return _filters; }
        }

        private IList<Employee> _queryResults = new List<Employee>();
        public IList<Employee> QueryResults
        {
            get { return _queryResults; }
            set
            {
                _queryResults = value;
                this.NotifyOfPropertyChange("QueryResults");
            }
        }

        public void Query(ObservableCollection<object> optionalFilters)
        {
            QueryResults = new List<Employee>();

            var filters = new List<Func<Employee, bool>>();
            var filters2 = new List<Func<IAgent, DateTime, bool>>();

            foreach (ICustomFilter filter in optionalFilters)
            {
                filter.BeforeQuery();
                if (filter.WhereClause is Func<Employee, bool>)
                    filters.Add(filter.WhereClause as Func<Employee, bool>);
                else if (filter.WhereClause is Func<IAgent, DateTime, bool>)
                    filters2.Add(filter.WhereClause as Func<IAgent, DateTime, bool>);
            }

            var targetDate = _canSupportAgentFinderPresenter.GetWatchPoint().Date;

            QueryResults = _agents.Cast<object>().Where(o =>
              {
                  var notMatched = filters.Any(f => f(_cast(o)) == false) || filters2.Any(f => f(o as IAgent, targetDate) == false);
                  return !notMatched && _canSupportAgentFinderPresenter.CanAddTo(o);

              }).Select(o => _cast(o)).ToList();
        }

        public bool SelectedAgentNotEmpty(IList selectedItems)
        {
            return selectedItems != null && selectedItems.Count != 0;
        }

        [Preview("SelectedAgentNotEmpty")]
        public void AddQueryResult(IList selectedItems)
        {
            _searchMode = AgentSearchMode.Add;
            var selected = _agents.OfType<object>().Where(o => selectedItems.Contains(_cast(o)));
            var existed = _canSupportAgentFinderPresenter.BindableAgents;
            foreach (var item in selected)
            {
                if (existed.Contains(item))
                {
                    item.SaftyInvoke<ISelectable>(o => o.IsSelected = false);
                    continue;
                }
                if (_canSupportAgentFinderPresenter.CanAddTo(item))
                    existed.Add(item);
            }

            _canSupportAgentFinderPresenter.FullyRefresh = () => true;
            _canSupportAgentFinderPresenter.Sort();

            UpdateQueryResult();
        }

        [Preview("SelectedAgentNotEmpty")]
        public void ReplaceWithResult(IList selectedItems)
        {
            _searchMode = AgentSearchMode.Replace;
            var results = _agents.OfType<object>().Where(o => selectedItems.Contains(_cast(o)) && _canSupportAgentFinderPresenter.CanAddTo(o));
            
            _canSupportAgentFinderPresenter.Agents = results; // castedAgents;
            _canSupportAgentFinderPresenter.Sort();
            UpdateQueryResult();
        }

        private void UpdateQueryResult()
        {
            var newResults = new List<Employee>(QueryResults.Count);
            QueryResults.ForEach(o =>
                                     {
                                         var isNotSelected = o.SaftyGetProperty<bool, ISelectable>(e => e.IsSelected != true);
                                         if (isNotSelected)
                                             newResults.Add(o);
                                         else
                                         {
                                             if (_canSupportAgentFinderPresenter.UnselectedAfterUpateQueryResult)
                                                 o.SaftyInvoke<ISelectable>(s => s.IsSelected = false);
                                         }

                                     });
            if (newResults.Count != QueryResults.Count)
                QueryResults = newResults;
        }

        protected override void OnShutdown()
        {
            Invoker = null;
            _cast = null;
            _filters = null;
            _agents = null;
            _queryResults = null;
            _model.TearDown();
            _model.SaftyInvoke<IDisposable>(d => d.Dispose());
            _model = null;
            base.OnShutdown();
        }

        #region IAgentFinderPresenter Members

        private AgentSearchMode _searchMode;
        public AgentSearchMode SearchMode
        {
            get { return _searchMode; }
        }

        #endregion
    }
}
