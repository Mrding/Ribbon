using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Core.Metadata;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.Shifts.Presenters.Interfaces;
using Luna.WPF.ApplicationFramework.Controls.Brick;
using Luna.WPF.ApplicationFramework.Presenters;

namespace Luna.Shifts.Presenters
{
    [PerRequest(typeof(IAssignmentTypeDetailPresenter))]
    public class AssignmentTypeDetailPresenter : DetailPresenter<BasicAssignmentType>, IAssignmentTypeDetailPresenter
    {
        private readonly IEntityFactory _entityFactory;
        private IList _singleList;
        private readonly string[] _causeBlockRebuildProperties = new[] { "Background", "Name", "SubEventInsertRules" };

        public AssignmentTypeDetailPresenter(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        protected override void OnInitialize()
        {
            //_showupPoint = Entity.Start.Subtract(TimeSpan.FromHours(1.5));
            base.OnInitialize();
        }

        public override void OnEntityChanging()
        {
            Entity.SubEventInsertRules.ForEach<INotifyPropertyChanged>(r => r.PropertyChanged -= OnEntityPropertyChanged);
            Entity.SaftyInvoke<AssignmentType>(a => a.WorkingDayMask.SaftyInvoke<INotifyPropertyChanged>(w=> w.PropertyChanged -= OnEntityPropertyChanged));
        }

        public override void OnEntityChanged()
        {
            Entity.SubEventInsertRules.ForEach<INotifyPropertyChanged>(r => r.PropertyChanged += OnEntityPropertyChanged); // 使subeventInsertRule属性发生变化时能通知
            Entity.SaftyInvoke<AssignmentType>(a => a.WorkingDayMask.SaftyInvoke<INotifyPropertyChanged>(w => w.PropertyChanged += OnEntityPropertyChanged));
            SubEventRulesView = CollectionViewSource.GetDefaultView(Entity).Self(o =>
            {
                o.Filter = t => t.Is<SubEventInsertRule>();
                o.SortDescriptions.Add(new SortDescription("Start", ListSortDirection.Ascending));
            });
            SelectedBlock = null;
            if(!IsInDesignMode)
                SubEventRulesView.Refresh();
            NotifyOfPropertyChange(() => SingleEntityList);
        }

        protected override void OnActivate()
        {
            ShowupPoint = Entity.Start.Subtract(TimeSpan.FromHours(1.5));
            base.OnActivate();
        }

        public virtual bool IsOvertime { get { return Entity.Type == typeof(OvertimeAssignment); } }

        public IList SingleEntityList { get { return _singleList ?? (_singleList = new List<IEnumerable>(new[] { Entity })); } }

        public ICollectionView SubEventRulesView { get; private set; }

        private bool _isInDesignMode;
        public bool IsInDesignMode
        {
            get { return _isInDesignMode; }
            set
            {
                _isInDesignMode = value;
                SelectedBlock = _isInDesignMode ? Entity : null;
                if (!_isInDesignMode)
                    SubEventRulesView.Refresh();
                NotifyOfPropertyChange(()=> IsInDesignMode);
            }
        }

        private DateTime _showupPoint;
        public DateTime ShowupPoint
        {
            get { return _showupPoint; }
            set
            {
                _showupPoint = value;
                NotifyOfPropertyChange(() => ShowupPoint);
            }
        }

        private object _selectedBlock;
        private object _selectedSubEventInsertRule;
        public object SelectedBlock
        {
            get { return _selectedBlock; }
            set
            {
                if (value.Is<SubEventInsertRule>() || value == null)
                {
                    _selectedSubEventInsertRule = value;
                    _selectedBlock = value.SaftyGetProperty<TermStyle, SubEventInsertRule>(r => r.SubEvent);
                    NotifyOfPropertyChange(() => SelectedBlock);
                }
            }
        }

        protected override void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnEntityPropertyChanged(sender, e);

            if (!IsEntityProperty(e.PropertyName) || !EditingObject.IsEditing) return;

            if (_causeBlockRebuildProperties.Contains(e.PropertyName))
                NotifyOfPropertyChange(() => SingleEntityList);
        }

        public void SetNewTime(RoutedEventArgs e)
        {
            var source = new Core.Reflector(e.OriginalSource);
            var newPlacement = source.Property<ITerm>("DropedPlacement");

            if(newPlacement==null) return;

            newPlacement = new TimeRange(newPlacement.Start.RemoveSeconds(),newPlacement.End.RemoveSeconds());

            var target = source.Property<ICanConvertToValueTerm>("PointOutBlock");

            var newStart = newPlacement.Start;
            var newEnd = newPlacement.End;

            if ((target.Start == newStart && target.End == newEnd))
                return;

            var targetIsSubeventInsertRule = !ReferenceEquals(Entity, target); //判断是assignment 或 subeventInsertRule
            if (targetIsSubeventInsertRule) 
            {
                // 以下为subeventInsertRule检查
                if (!newPlacement.IsInTheRange(Entity.Start, Entity.End)) // 有没有超出 assignment
                    return;

                if (Entity.GetSubEventInsertRules().Any(o => !ReferenceEquals(o, target) && o.IsCoverd(newPlacement))) // 和其他 subeventInsertRule 相交
                    return;
            }

            target.SetNewTime(newStart, newEnd);
        }

        public void CanAlter(RoutedEventArgs e)
        {
            e.Handled = !IsInDesignMode || e.OriginalSource.SaftyGetProperty<bool, ICanConvertToValueTerm>(o => o.Level > 1);
        }

        public void DraggingNewSubEventInsertRule(DragingRoutedEventArgs e)
        {
            var draggingSubEventType = e.DrageItemData.As<TermStyle>();
            e.Target = new VisibleSubEventTerm(draggingSubEventType, e.PointTime);
        }

        public void AddInsertRule(DragEventArgs e)
        {
            var start = e.Data.GetData("DropedPlacement").SaftyGetProperty<DateTime, ITerm>(o => o.Start);
            var subevent = e.Data.GetData("PointOutBlock").SaftyGetProperty<TermStyle, VisibleSubEventTerm>(o => o.Source);

            if (subevent == null || subevent.Type == typeof(Shrink))
                return;

            var startValue = (int)start.Subtract(Entity.Start).TotalMinutes;
            var endValue = startValue + (subevent.TimeRange.Length * 1);
            var rule = _entityFactory.Create<SubEventInsertRule>();
            //var timeRange = new TimeValueRange(startValue, endValue);
            //if (timeRange.IsValid) return null;
            rule.MAssign(new
            {
                TimeRange = new TimeValueRange(startValue, endValue),
                SubEvent = subevent,
                SubEventLength = subevent.TimeRange.Length
            });
            if (Entity.AddSubEventInsertRule(rule))
                SelectedBlock = rule;
        }

        public void RemoveInsertRule()
        {
            _selectedSubEventInsertRule.SaftyInvoke<SubEventInsertRule>(r =>
            {
                r.As<INotifyPropertyChanged>().PropertyChanged -= OnEntityPropertyChanged;
                Entity.RemoveSubEventInsertRule(r);
                SelectedBlock = null;
            });
        }
    }
}
