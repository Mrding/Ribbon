using Luna.Common;
using Nh = Iesi.Collections.Generic;


namespace Luna.Infrastructure.Domain
{
    public class Campaign : Entity, ICampaign
    {
        private Nh.ISet<Entity> _organizations = new Nh.HashedSet<Entity>();

        public static Campaign AllOptopn = new Campaign { Name = "All" };

        public virtual Nh.ISet<Entity> Organizations
        {
            get { return _organizations; }
            set { _organizations = value; }
        }

        public override string GetUniqueKey()
        {
            return Name;
        }
    }
}
