using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ActiproSoftware.Windows.Controls.Ribbon.Controls;
using ActiproSoftware.Windows.DocumentManagement;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Metadata;
using Luna.Common.Interfaces;
using Luna.Core;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Domain.Model;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Interfaces;
using Luna.WPF.ApplicationFramework.Threads;
using ApplicationCommands = System.Windows.Input.ApplicationCommands;

namespace Luna.Shifts.Presenters
{
    [Singleton("RecentSchedule", typeof(IBackstageTabPresenter))]
    public class RecentScheduleManagerPresenter : DefaultPresenter, IBackstageTabPresenter
    {
        private readonly IList<Tuple<int, string>> _rangeFilters;
        private readonly IDockSitePresenter _dockSitePresenter;
        private IScheduleManagerModel _scheduleManagerModel;
        private readonly RecentDocumentManager _recentScheduleManager;
        private IDictionary<IDocumentReference, IMetadataContainer> _openedDocs;

        private IList<Schedule> _schedules;
        //private Thread _thread;

        public RecentScheduleManagerPresenter(INotifyNhBuildComplete nhLoader, IDockSitePresenter dockSitePresenter)
        {
            _dockSitePresenter = dockSitePresenter;

            _recentScheduleManager = new RecentDocumentManager { MaxFilteredDocumentCount = 20 };
            _rangeFilters = new List<Tuple<int, string>>()
                                {
                                    new Tuple<int, string>(3,"3 month"),
                                    new Tuple<int, string>(6,"6 month"),
                                    new Tuple<int, string>(12,"1 year"),
                                };

            nhLoader.AfterBuildComplete(PersistenceLayerBuildCompleted);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnExecuted));
        }

        private void PersistenceLayerBuildCompleted()
        {
            _scheduleManagerModel = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IScheduleManagerModel>();

            new Thread(() =>
            {
                var misc = _scheduleManagerModel.LoadMisc();
                var l = new List<Campaign> {Campaign.AllOptopn};

                l.AddRange(misc.Item1);
                _campaigns = l;


                SearchRecentSchedule();
                BuildDocuments();


                RangeFilters.CurrentChanged += delegate
                {
                    SearchRecentSchedule();
                    BuildDocuments();
                };

                _campaignsView.CurrentChanged += delegate
                {
                    BuildDocuments();
                };

                NotifyOfPropertyChange(() => Campaigns);

            }).Self(t => { t.IsBackground = true; t.Start(); });


        }

        private void SearchRecentSchedule()
        {
            var pastMonths = RangeFilters.CurrentItem.SaftyGetProperty<int, Tuple<int, string>>(o => o.Item1, () => 3);
            _schedules = _scheduleManagerModel.GetAll(pastMonths);
        }

        private void BuildDocuments()
        {
            if (Campaigns == null || Campaigns.CurrentItem == null || _schedules == null) return;
            UIThread.BeginInvoke(() =>
            {
                _openedDocs = new Dictionary<IDocumentReference, IMetadataContainer>(_schedules.Count);
                RecentScheduleManager.Documents.BeginUpdate();
                RecentScheduleManager.Documents.Clear();
                foreach (var o in _schedules)
                {
                    if (o.Campaign.Equals(Campaigns.CurrentItem) || ReferenceEquals(Campaigns.CurrentItem, Campaign.AllOptopn))
                        _recentScheduleManager.Documents.Add(new DocumentReference
                        {
                            Name = string.Format("{0:yyyy/MM/dd} - {1:MM/dd}", o.Start, o.End.AddDays(-1)),
                            Description = o.Campaign.Name,
                            Tag = o,
                            LastOpenedDateTime = o.Start,
                            ImageSourceLarge = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Images/Calendar32.png", UriKind.Relative))
                        });
                }
                _recentScheduleManager.Documents.EndUpdate();
            });
        }

        public RecentDocumentManager RecentScheduleManager { get { return _recentScheduleManager; } }


        private ICollectionView _rangeFiltersView;
        public ICollectionView RangeFilters
        {
            get { return _rangeFiltersView ?? (_rangeFiltersView = CollectionViewSource.GetDefaultView(_rangeFilters)); }
        }

        private IEnumerable _campaigns;
        public ICollectionView _campaignsView;
        public ICollectionView Campaigns
        {
            get
            {
                if (_campaignsView == null && _campaigns != null)
                    _campaignsView = CollectionViewSource.GetDefaultView(_campaigns);

                return _campaignsView;
            }
        }

        private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Parameter.SaftyInvoke<IDocumentReference>(d =>
                {
                    if (_openedDocs.ContainsKey(d))
                    {
                        _dockSitePresenter.Activate(_openedDocs[d]);
                        return;
                    }

                    Show<Interfaces.IShiftComposerPresenter>(new Dictionary<string, object> { { "Schedule", d.Tag } });

                    _openedDocs[d] = _dockSitePresenter.ActivePresenter as IMetadataContainer;
                    _dockSitePresenter.ActivePresenter.SaftyInvoke<ILifecycleNotifier>(w =>
                    {
                        w.WasShutdown += delegate
                        {
                            _openedDocs.Remove(d);
                        };
                    });

                });
        }
    }
}
