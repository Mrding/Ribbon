using System.Collections.Generic;
using Luna.Common.Interfaces;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class ComparableOrganization : Common.Domain.CompareToSelectedEntity<Organization>,IHierarchical
    {
        public ComparableOrganization(Organization entity, System.Func<Organization, bool> compareWith, System.Action<Common.Domain.CompareToSelectedEntity<Organization>, bool?> whenSelected,
            IEntityFactory entityFactory)
            : base(entity, compareWith, whenSelected)
        {

            _children = new List<IHierarchical>( entity.Children !=null ? entity.Children.Count : 0);
            if(entity.Parent == null)
                BuildTree(this, entity.Children,entityFactory);
        }

        private void BuildTree(IHierarchical root, IEnumerable<IHierarchical> children,IEntityFactory entityFactory)
        {
            foreach (Organization child in children)
            {
                var @params = new Dictionary<string, object>
                {
                             {"entity", child},
                              {"compareWith", _compareWith},
                              {"whenSelected", _whenSelected},
                              {"Parent", root}
                };
                var obj = entityFactory.Create<ComparableOrganization>(@params);

                root.Children.Add(obj);

                if (obj.Children != null)
                    BuildTree(obj, obj.Entity.Children, entityFactory);
            }
        }

        public IHierarchical Parent
        {
            get; set;
        }

        private ICollection<IHierarchical> _children;
        public virtual ICollection<IHierarchical> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}