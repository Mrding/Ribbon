using System;
using Luna.Common.Interfaces;

namespace Luna.Infrastructure.Domain
{
    public static class OrganizationExtension
    {
        public static IHierarchical FindRootOrganization(this IHierarchical org)
        {
            if (org.Parent == null)
                return org;
            else
                return FindRootOrganization(org.Parent);
        }

        public static void VisitOrganizationTree<T>(this T root, Action<T> action) where T : IHierarchical
        {
            action(root);
            foreach (T child in root.Children)
            {
                child.VisitOrganizationTree(action);
            }
        }
    }
}
