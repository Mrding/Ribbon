using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Common;
using Luna.ComponentBehaviors;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;
using uNhAddIns.ComponentBehaviors;
using uNhAddIns.ComponentBehaviors.Castle;
using uNhAddIns.ComponentBehaviors.Castle.Configuration;

namespace Luna.Basic.GuyWire.Configurators
{
    public class EntitiesConfigurator : AbstractConfigurator
    {
        protected BehaviorDictionary _config;

        protected void AddFacility(IWindsorContainer container)
        {
            //Add the facility
            container.AddFacility<LunaComponentBehaviorsFacility>();
            container.AddFacility<ComponentBehaviorsFacility>();
        }

        protected void AddBehavior(IWindsorContainer container)
        {
            //Register the Behavior
            _config = new BehaviorDictionary();
            //Adherence
            _config.For<AgentStatusType>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<AgentStatus>().Add<AgentStatusBehavior>();
            //Administration
            _config.For<Employee>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<LaborRule>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<DayOffRule>().Add<NotifyPropertyChangedBehavior>();
            _config.For<MaskOfDay>().Add<NotifyPropertyChangedBehavior>();
            _config.For<Organization>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<Skill>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            //Infrastructure
            //x_config.For<Acd>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            //x_config.For<AcdQueue>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<Campaign>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>().Add<SelectableBehavior>();
            _config.For<CalendarEvent>().Add<NotifyPropertyChangedBehavior>();
            
            //_config.For<NationalHoliday>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<Schedule>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditableBehavior>();
            _config.For<ServiceQueue>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            //Seating
            //x_config.For<Area>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<EditableBehavior>().Add<IndexableBehavior>();
            //x_config.For<OrganizationSeatingArea>().Add<NotifyPropertyChangedBehavior>().Add<EditableBehavior>();
            //x_config.For<Seat>().Add<NotifyPropertyChangedBehavior>().Add<EditableBehavior>().Add<SelectableBehavior>();
            //x_config.For<Site>().Add<NotifyPropertyChangedBehavior>().Add<EditableBehavior>();
            //x_config.For<SeatConsolidationRule>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>().Add<IndexableBehavior>();
            //x_config.For<SeatingEngineStatus>().Add<NotifyPropertyChangedBehavior>();
            //Shifts
            //x_config.For<SchedulingPayload>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>();
            _config.For<Attendance>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>();

            _config.For<Agent>().Add<NotifyPropertyChangedBehavior>();
            _config.For<PlanningAgent>().Add<NotifyPropertyChangedBehavior>().Add<IndexableBehavior>();
            _config.For<SubEventInsertRule>().Add<NotifyPropertyChangedBehavior>();
            _config.For<BasicAssignmentType>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<IndexableBehavior>().Add<EditingBehavior>();
            _config.For<AssignmentType>().Add<NotifyPropertyChangedBehavior>().Add<SelectableBehavior>().Add<IndexableBehavior>().Add<EditingBehavior>();

            _config.For<TermStyle>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();

            //x_config.For<ShiftGroup>().Add<NotifyPropertyChangedBehavior>().Add<EditingBehavior>().Add<EditableBehavior>();
            _config.For<Common.Domain.CompareToSelectedEntity<Employee>>().Add<NotifyPropertyChangedBehavior>();
            _config.For<ComparableOrganization>().Add<NotifyPropertyChangedBehavior>();

            container.Register(Component.For<IBehaviorStore>().Instance(_config));
        }

        protected void RegisterToContainer(IWindsorContainer container)
        {
            container.Register(Component.For<IEntityFactory>().ImplementedBy<EntityFactory>().LifeStyle.Singleton);
            //Adherence
            //xcontainer.Register(Component.For<AgentStatus>().LifeStyle.Transient);
            //xcontainer.Register(Component.For<AgentStatusType>().LifeStyle.Transient);
            //Administration
            container.Register(Component.For<Employee>().LifeStyle.Transient);
            container.Register(Component.For<DayOffRule>().LifeStyle.Transient);
            container.Register(Component.For<MaskOfDay>().LifeStyle.Transient);
           
            container.Register(Component.For<Organization>().LifeStyle.Transient);
            container.Register(Component.For<Skill>().LifeStyle.Transient);
            //Infrastructure
            //xcontainer.Register(Component.For<Acd>().LifeStyle.Transient);
            //xcontainer.Register(Component.For<AcdQueue>().LifeStyle.Transient);
            container.Register(Component.For<Campaign>().LifeStyle.Transient);
            container.Register(Component.For<CalendarEvent>().LifeStyle.Transient);
            container.Register(Component.For<Holiday>().LifeStyle.Transient);
            container.Register(Component.For<DaylightSavingTime>().LifeStyle.Transient);
            //xcontainer.Register(Component.For<NationalHoliday>().LifeStyle.Transient);
            container.Register(Component.For<Schedule>()
                                   .DependsOn(Property.ForKey("Campaign").Eq(new Campaign()))
                                   .LifeStyle.Transient);
            container.Register(Component.For<ServiceQueue>().LifeStyle.Transient);
            //Seating
            /*container.Register(Component.For<Area>().Proxy.AdditionalInterfaces(typeof(IArea)).LifeStyle.Transient);

            container.Register(Component.For<OrganizationSeatingArea>()
                                   .DependsOn(Property.ForKey("Area").Eq(new Area()))
                                   .DependsOn(Property.ForKey("Entity").Eq(new Organization()))
                                   .DependsOn(Property.ForKey("TargetSeat").Eq(new Seat()))
                                   .LifeStyle.Transient);
            
            container.Register(Component.For<Seat>()
                                   .DependsOn(Property.ForKey("Area").Eq(new Area()))
                                   .LifeStyle.Transient);

            container.Register(Component.For<SeatConsolidationRule>().LifeStyle.Transient);
            container.Register(Component.For<Site>().LifeStyle.Transient);
            container.Register(Component.For<SeatingEngineStatus>().LifeStyle.Transient);*/
            //Shifts
            container.Register(Component.For<SchedulingPayload>().LifeStyle.Transient);
            container.Register(Component.For<Attendance>().LifeStyle.Transient);
            
            container.Register(Component.For<Agent>().LifeStyle.Transient);
            container.Register(Component.For<PlanningAgent>().LifeStyle.Transient);
            /*container.Register(Component.For<ShiftGroup>()
                //.DependsOn(Property.ForKey("WorkingDayMask").Eq(container.Resolve<MaskOfDay>()))
                .LifeStyle.Transient);*/
            container.Register(Component.For<TermStyle>().LifeStyle.Transient);
            container.Register(Component.For<SubEventInsertRule>().LifeStyle.Transient);
            container.Register(Component.For<AssignmentType>().LifeStyle.Transient);
            container.Register(Component.For<BasicAssignmentType>().LifeStyle.Transient);
            container.Register(Component.For<Common.Domain.CompareToSelectedEntity<Employee>>().LifeStyle.Transient);
            container.Register(Component.For<ComparableOrganization>().LifeStyle.Transient);
        
         

            //Scheduling
            //container.Register(Component.For<Scheduling.Domain.SchedulingStatus>().LifeStyle.Transient);)))
        }

        protected override void Customize(IWindsorContainer container)
        {
            container.Register(Component.For<LaborRule>()
                //.DependsOn(Property.ForKey("DayOffRule").Eq(container.Resolve<DayOffRule>()))
                //.DependsOn(Property.ForKey("DayOffMask").Eq(container.Resolve<MaskOfDay>()))
                //.DependsOn(Property.ForKey("ShiftGroups").Eq(new uNhAddIns.WPF.Collections.ObservableSet<ShiftGroup>()))
           .LifeStyle.Transient);

            //container.Register(Component.For<LaborRule>()
            // .DependsOn(Property.ForKey("DayOffRule").Eq(container.Resolve<DayOffRule>()))
            // .DependsOn(Property.ForKey("DayOffMask").Eq(container.Resolve<MaskOfDay>()))
            // //.DependsOn(Property.ForKey("ShiftGroups").Eq(new uNhAddIns.WPF.Collections.Types.ObservableSetType<ShiftGroup>()))
            // .LifeStyle.Transient);
        }

        public override void Configure(IWindsorContainer container)
        {
            AddFacility(container);
            AddBehavior(container);
            RegisterToContainer(container);
            Customize(container);
          
            base.Configure(container);
        }
    }
}
