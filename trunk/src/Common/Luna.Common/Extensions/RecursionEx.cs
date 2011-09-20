using System.Linq;
using Luna.Common.Interfaces;
using System;

namespace Luna.Common.Extensions
{
    public static class RecursionEx
    {
        //树形状结构节点单击时
        public static void RecursionLooping<T>(this T current, bool includeSelf, Action<T> action) where T : IHierarchical
        {
            if (Equals(current,default(T))) return;
            FooChild(current, includeSelf, action);
        }

        //递归子节点跟随其全选或全不选
        private static void FooChild<T>(T node,bool includeSelf, Action<T> action) where T : IHierarchical
        {
            if (includeSelf)
                action(node);
            foreach (T nd in node.Children)
                FooChild(nd,true,action);
        }

        //递归父节点跟随其全选或全不选
        private static void FooParent(IHierarchical root)
        {
            if (root.Parent == null) return;
            //遍历该节点的兄弟节点
            var brotherNodeCheckedCount = root.Parent.Children.Count(node => ((ISelectable)node).IsSelected == true);
            //兄弟节点全没选
            if (brotherNodeCheckedCount == 0)
            {
                var parentNode = root.Parent;
                ((ISelectable)parentNode).IsSelected = false;
                //其父节点也不选
                FooParent(parentNode);
            }
            //兄弟节点中只要有一个被选
            if (brotherNodeCheckedCount == 1)
            {
                var parentNode = root.Parent;
                ((ISelectable)parentNode).IsSelected = true;
                //其父节点也被选
                FooParent(parentNode);
            }
        }
    }
}
