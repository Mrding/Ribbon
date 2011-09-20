using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Core.Metadata;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.Statistic.Domain;
using Luna.Statistic.Domain.Model;
using Luna.Statistic.Domain.Service;
using Luna.Statistic.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Threads;
using Luna.WPF.ApplicationFramework.Extensions;


namespace Luna.Statistic.Presenters
{
    [PerRequest(typeof(IStaffingChartPresenter))]
    public partial class StaffingChartPresenter : DefaultPresenter, IStaffingChartPresenter
    {
        private readonly Schedule _schedule;
        private readonly IStaffingCalculatorModel _staffingCalculatorModel;
        private IServiceQueueContainer _serviceQueueContainer;
        private readonly IStaffingCalculatorService _staffingService;
        private readonly IEntityFactory _entityFactory;
        private DateRange _enquiryRange;
        private Thread _staffingCalculatorModelFetching;
        private StatisticRaw _combinedServiceQueues;
        private IEnumerable<IVisibleLinerData> _combinedServiceQueuesItems;
        private ICollectionView _staffingCalculatorData;
        private string _viewMode = "Analysis";  // Estimation, Analysis, Combine
        private ICollectionView _serviceQueuesView;
        private bool _analyisAborted;
        private bool _analyisComplete;

        public StaffingChartPresenter(IStaffingCalculatorModel staffingCalculatorModel,
            IEntityFactory entityFactory, Schedule schedule, IStaffingCalculatorService service)
        {
            _schedule = schedule;
            _staffingService = service;
            _staffingCalculatorModel = staffingCalculatorModel;
            _entityFactory = entityFactory;
        }

        protected override void OnActivate()
        {
            _staffingCalculatorData.Refresh();
            base.OnActivate();
        }

        protected override void OnInitialize()
        {
            Invoker.SaftyInvoke<DefaultPresenter>(p =>
                                                      {
                                                          //register ribbon buttons, 并非最佳作法, 导致 CommandBindings.Clear() 然后再重新加入  CommandBindings.AddRange();
                                                          p.CommandBindings.Add(new CommandBinding(WPF.ApplicationFramework.ApplicationCommands.ExportSvcLevelData, OnExportSvcLevelDataExecuted,
                                                              (d, e) => { e.CanExecute = AnalyisComplete; }));
                                                          p.NotifyOfPropertyChange(() => CommandBindings);
                                                          p.PropertyChanged += InvokerPropertyChanged;
                                                      });

            _enquiryRange = new DateRange(_schedule.Start.AddDays(Global.HeadDayAmount), _schedule.End.AddDays(Global.TailDayAmount));

            _serviceQueueContainer = _staffingCalculatorModel.Preparing(_schedule.Id, _enquiryRange, ConvertTo); // 读取预测数据 
            _serviceQueuesView = CollectionViewSource.GetDefaultView(_serviceQueueContainer.GetEntities());

            WhenReady.SaftyInvoke(d => d(_serviceQueueContainer, SwitchView));
            Pop();
            _combinedServiceQueues = new StatisticRaw(new CombinedServiceQueueStatistic(_serviceQueueContainer.Values, _serviceQueueContainer.CoverageDays), ConvertTo);
            _combinedServiceQueues.SetForceastValues();
            _combinedServiceQueues.Items3 = CollectionViewSource.GetDefaultView(_combinedServiceQueues.Items).Self(v =>
            {
                v.Filter = d => d.SaftyGetProperty<bool, IVisibleLinerData>(
                    line => line.Category == _selectedCategory);
            });

            base.OnInitialize();
            FullyFetch();

            #region 合併SQ做估算的code
            //new Thread(() =>
            //{
            //    IEnumerable[] lines = { new List<IStaffingStatistic>() };
            //    var action = new Action(() => StaffingStatistics = CollectionViewSource.GetDefaultView(lines[0]));
            //    _serviceQueueContainer.Output2(out lines[0], ref action);
            //    UIThread.Invoke(action);
            //}).Start();
            #endregion
        }

        private void SwitchView(string key)
        {
            ViewMode = key;
        }

        private IVisibleLinerData ConvertTo(double[] values, int category, string resourceKey, object queue)
        {
            var entity = _entityFactory.Create<IVisibleLinerData>();
            entity.Color = resourceKey + "LineColor";
            entity.Values = values;
            entity.Text = resourceKey;
            entity.Source = queue;
            entity.Category = category;
            entity.Format = category == 1 ? "{0:0.#%}" : "{0:0.#}";
            if (category == 1)
                entity.MaxValue = 1.1;

            entity.SaftyInvoke<ISelectable>(o => o.IsSelected = true);
            return entity;
        }

        private void InvokerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                var viewRange = _staffingService.GetViewRange();
                var start = Math.Max(0, (int)viewRange.Start.Subtract(_enquiryRange.Start).TotalMinutes / 15);
                var end = (int)(_enquiryRange.End < viewRange.End ? _enquiryRange.End : viewRange.End).Subtract(_enquiryRange.Start).TotalMinutes / 15;

                _staffingCalculatorModelFetching.SaftyInvoke(t =>
                {
                    if (!t.IsAlive) return;

                    t.Abort();
                    AnalyisAborted = true;

                });
                _staffingCalculatorModelFetching = new Thread(() =>
                {
                    AnalyisComplete = false;
                    // 多查询前期一天预测
                    _staffingCalculatorModel.Fetch(start, end, _serviceQueueContainer, _staffingService.GetAgents());
                    Pop();
                    AnalyisComplete = true;
                }).Self(t => { t.IsBackground = true; t.Start(); });
            }

            if (e.PropertyName == "StatisticItems")
            {
                //刷新估算UI
                //xIEnumerable lines;
                //xvar action = new Action(() => { });
                //x_serviceQueueContainer.Output2(out lines, ref action);
                //xUIThread.Invoke(action);
            }
        }

        private void Pop()
        {
            IEnumerable dataList;

            _serviceQueueContainer.Output(o =>
            {
                if (o.Items3 != null)
                    return;
                o.Items3 = CollectionViewSource.GetDefaultView(o.Items)
                   .Self(v =>
                   {
                       v.Filter = d => d.SaftyGetProperty<bool, IVisibleLinerData>(line => line.Category == _selectedCategory);
                   });
            }, out dataList);

            if (StaffingCalculatorData == null)
                StaffingCalculatorData = CollectionViewSource.GetDefaultView(dataList);
            else
                UIThread.BeginInvoke(() => StaffingCalculatorData.Refresh());
        }


        public void FullyFetch()
        {
            AnalyisComplete = false;
            AnalyisAborted = false;
            _staffingCalculatorModelFetching = new Thread(() =>
            {
                // 多查询前期一天预测
                _staffingCalculatorModel.Fetch(0, 96 * (int)_enquiryRange.Duration.TotalDays, _serviceQueueContainer, _staffingService.GetAgents());//初次查询
                Pop();
                AnalyisComplete = true;
            }).Self(t => { t.IsBackground = true; t.Start(); });
        }

        public void Combine()
        {
            _combinedServiceQueues.Source.Output(); // sum
            _combinedServiceQueues.Output(); // line chart update

            WhenCurrentQueueSelected.SaftyInvoke(a =>
            {
                a(_combinedServiceQueues.Source);
                _combinedServiceQueues.Items3.SaftyInvoke<ICollectionView>(v => v.Refresh());
            });


            _combinedServiceQueuesItems = _combinedServiceQueues.Items;
            this.QuietlyReload(ref _combinedServiceQueuesItems, "CombinedServiceQueuesItems");
            NotifyOfPropertyChange(() => CombinedServiceQueuesItems3);
        }


        public ICollectionView ServiceQueues
        {
            get { return _serviceQueuesView; }
        }

        public IEnumerable<IVisibleLinerData> CombinedServiceQueuesItems { get { return _combinedServiceQueuesItems; } }

        public IEnumerable CombinedServiceQueuesItems3 { get { return _combinedServiceQueues.Items3; } }


        public ICollectionView StaffingCalculatorData
        {
            get { return _staffingCalculatorData; }
            set
            {
                _staffingCalculatorData = value;
                _staffingCalculatorData.CurrentChanged += StaffingCalculatorDataCurrentChanged;
                NotifyOfPropertyChange(() => StaffingCalculatorData);
            }
        }

        public Action<IServiceQueueStatistic> WhenCurrentQueueSelected { get; set; }

        public Action<IServiceQueueContainer, Action<string>> WhenReady { get; set; }

        private int _selectedCategory;
        public StatisticCategory SelectedCategory
        {
            get { return (StatisticCategory)_selectedCategory; }
            set
            {
                _selectedCategory = (int)value;

                var statisticRaw = default(StatisticRaw);

                if (_viewMode == "Analysis")
                    statisticRaw = _staffingCalculatorData.CurrentItem as StatisticRaw;
                else if (_viewMode == "CompositiveServiceQueue")
                    statisticRaw = _combinedServiceQueues;
                statisticRaw.SaftyInvoke(raw => raw.Items3.SaftyInvoke<ICollectionView>(v => v.Refresh()));
                NotifyOfPropertyChange(() => SelectedCategory);
            }
        }

        public bool AnalyisAborted
        {
            get { return _analyisAborted; }
            set { _analyisAborted = value; NotifyOfPropertyChange(() => AnalyisAborted); }
        }


        public bool AnalyisComplete
        {
            get { return _analyisComplete; }
            set
            {
                _analyisComplete = value; NotifyOfPropertyChange(() => AnalyisComplete);
                UIThread.BeginInvoke(CommandManager.InvalidateRequerySuggested);
            }
        }

        public string ViewMode
        {
            get { return _viewMode; }
            set
            {
                if (_viewMode != value)
                {
                    var statisticRaw = default(StatisticRaw);

                    if (value == "Analysis")
                        statisticRaw = _staffingCalculatorData.CurrentItem as StatisticRaw;
                    else if (value == "CompositiveServiceQueue")
                        statisticRaw = _combinedServiceQueues;
                    else
                    {
                        LoadEstimationNeeds();
                        statisticRaw = _staffingCalculatorData.CurrentItem as StatisticRaw;
                    }
                    statisticRaw.SaftyInvoke(o =>
                    {
                        WhenCurrentQueueSelected.SaftyInvoke(a => a(o.Source));
                        o.Items3.SaftyInvoke<ICollectionView>(v => v.Refresh());
                    });
                }

                _viewMode = value;
                NotifyOfPropertyChange(() => ViewMode);
            }
        }

        private void StaffingCalculatorDataCurrentChanged(object sender, EventArgs e)
        {
            _staffingCalculatorData.CurrentItem.SaftyInvoke<StatisticRaw>(q =>
            {
                q.Items3.SaftyInvoke<ICollectionView>(v => v.Refresh());
                WhenCurrentQueueSelected.SaftyInvoke(a => a(q.Source));
            });
        }

        protected override void OnShutdown()
        {
            _staffingCalculatorModelFetching.Abort();
            _serviceQueueContainer.Dispose();
            _serviceQueueContainer = null;
            _staffingCalculatorData.CurrentChanged -= StaffingCalculatorDataCurrentChanged;
            _staffingCalculatorData.CurrentChanged -= StaffingCalculatorDataCurrentChangedOnEstimationMode;
            _staffingCalculatorData = null;
            Invoker.SaftyInvoke<INotifyPropertyChanged>(p => p.PropertyChanged -= InvokerPropertyChanged);
            WhenCurrentQueueSelected = null;
            WhenReady = null;
            _staffingCalculatorModel.Release();
            base.OnShutdown();
        }
    }
}

