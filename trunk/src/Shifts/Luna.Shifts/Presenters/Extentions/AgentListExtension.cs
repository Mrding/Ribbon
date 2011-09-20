using System.Collections;
using System.Collections.Generic;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using System;

namespace Luna.Shifts.Presenters.Extentions
{

    /// <summary>
    /// For ShiftComposerPresenter & ShiftDispatcherPresenter use
    /// </summary>
    internal static class AgentListExtension
    {
        public static int ReAssignItemFromList<TItem>(this TItem item, IList<TItem> list, Func<TItem, TItem> process)
        {
            var index = list.IndexOf(item);
            if (index != -1)
            {
                list[index] = process(list[index]);
            }
            return index;
        }

        public static int ReAssignItemFromList<TItem>(this TItem item, IList list, Func<TItem, TItem> process)
        {
            var index = list.IndexOf(item);
            if (index != -1)
            {
                list[index] = process((TItem)list[index]);
            }
            return index;
        }

        public static IEnumerable<IAgent> ReAssignAgents(this IList agents, IList bindableAgents, IList<IAgent> allAgents, Func<IAgent, IAgent> process)
        {
            var reassignedAgents = new IAgent[agents.Count];

            var i = new[] { 0 };
            foreach (IAgent t in agents)
            {
                var index = t.ReAssignItemFromList(bindableAgents, process);
                if (index != -1)
                {
                    var reloadAgent = (IAgent)bindableAgents[index];

                    t.ReAssignItemFromList(allAgents, originalAgent =>
                    {
                        reassignedAgents[i[0]] = reloadAgent;
                        return reassignedAgents[i[0]];
                    });
                }
                i[0]++;
            }

            return reassignedAgents;
        }

        public static IEnumerable<IAgent> ReAssignAgents(this IList<IAgent> agents, IList bindableAgents, IList<IAgent> allAgents, Func<IAgent, IAgent> process)
        {
            var reassignedAgents = new IAgent[agents.Count];

            var i = new []{0};
            foreach (var t in agents)
            {
                var index = t.ReAssignItemFromList(bindableAgents, process);
                if (index != -1)
                {
                    var reloadAgent = (IAgent) bindableAgents[index];

                    t.ReAssignItemFromList(allAgents, originalAgent =>
                    {
                        reassignedAgents[i[0]] = reloadAgent;
                        return reassignedAgents[i[0]];
                    });
                }
                i[0]++;
            }
          
            return reassignedAgents;
        }
    }
}
