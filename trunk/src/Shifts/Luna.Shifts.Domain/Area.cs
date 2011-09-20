using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;
using Luna.Common;
using Microsoft.Practices.ServiceLocation;

namespace Luna.Shifts.Domain
{
    public class OrganizationSeatingArea : SequentialEntity<Entity>, IArrangeSeatRule
    {
        public virtual ArrangeSeatMethodology Methodology { get; set; }

        private ISeat _targetSeat;
        public virtual ISeat TargetSeat
        {
            get { return _targetSeat; }
            set
            {


                _targetSeat = value;
            }
        }

        public virtual Entity Area { get; set; }



    }

    public class Area : Entity, IArea, IClozeContainer, IIndexable, IEquatable<Area>, IEqualityComparer<Area>
    {
        public virtual Entity Site { get; set; }

        public virtual Dimension Dimension { get; set; }

        private ISet _seatSet;
        public virtual ICollection<ISeat> Seats
        {
            get { return _seatSet as ICollection<ISeat>; }
            set { _seatSet = value as ISet; }
        }

        public override string GetUniqueKey()
        {
            return Name;
        }

        private Entity _campaign;
        public virtual Entity Campaign { get { return _campaign; } set { _campaign = value; } }

        //private IEnumerable<CampaignOrganization> _serviceOrganizations;
        //IEnumerable<CampaignOrganization> ISeatingArea.Organizations
        //{
        //    get
        //    {
        //        if (_serviceOrganizations == null)
        //            _serviceOrganizations = _campaign.GetOrganizations<CampaignOrganization>();
        //        return _serviceOrganizations;
        //    }
        //}

        public virtual int Capacity
        {
            get { return Dimension.Count; }
        }

        public virtual int Index { get; set; }

        public virtual IndexingMode Mode { get; set; }

        public virtual ICloze NewItem(int index)
        {
            var newItem = ServiceLocator.Current.GetInstance<IEntityFactory>().Create<Seat>(new Dictionary<string, object>
                                                                                                {
                                                                                                    {"Area",this},
                                                                                                    {"InUse", false},
                                                                                                    {"Mode", IndexingMode.Location},
                                                                                                    {"LocationIndex",index},
                                                                                                    {"IsActivated", false},
                                                                                                    {"UsingOrganozation", default(Entity)}
                                                                                                });
            return newItem;
        }

        IEnumerator IClozeContainer.GetEnumerator()
        {

            if (_seatSet == null) return null;

            return _seatSet.GetEnumerator();
        }

        public virtual bool Equals(Area other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (this.Id == Guid.Empty && other.Id == Guid.Empty) return false;
            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Area;
            if (other == null) return false;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return Id == Guid.Empty ? base.GetHashCode() : Id.GetHashCode();
        }

        public virtual bool Equals(Area x, Area y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public virtual int GetHashCode(Area obj)
        {
            return obj.GetHashCode();
        }
    }
}
