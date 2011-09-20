using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// redefine the public method for the element inherit from frameworkelement
    /// </summary>
    public interface ILogicalParent
    {
        /// <summary>
        /// Adds the provided object to the logical tree of this element
        /// </summary>
        /// <param name="child"></param>
        void AddLogicalChild(object child);

        /// <summary>
        /// Removes the specified element from the logical tree for this element
        /// </summary>
        /// <param name="child"></param>
        void RemoveLogicalChild(object child);

        /// <summary>
        /// Replaces the specified element from the logical for this element
        /// </summary>
        /// <param name="oldchild"></param>
        /// <param name="newChild"></param>
        void ReplaceLogicalChild(object oldchild, object newChild);
    }
}
