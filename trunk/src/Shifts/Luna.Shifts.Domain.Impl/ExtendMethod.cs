using System.Collections.Generic;

namespace Luna.Shifts.Domain.Impl
{
    internal static class ExtendMethod
    {
        internal static List<Fork> GetFork(this Term[] filteredTerms)
        {
            #region Create Tree
            var root = new Fork(null);
            Fork currentFork;
            Fork newFork;
            Fork tempFork;

            var termNumber = filteredTerms.Length;

            for (int i = 0; i < termNumber; i++)
            {
                if (root.Contain(filteredTerms[i]) != null)
                    continue;
                if (filteredTerms[i].ParentTerm == null)
                {
                    root.ChildList.Add(new Fork(filteredTerms[i]));
                }
                else
                {
                    currentFork = new Fork(filteredTerms[i]);
                    while (currentFork.Self.ParentTerm != null)
                    {
                        // check if create new fork
                        tempFork = root.Contain(currentFork.Self.ParentTerm);
                        if (tempFork == null)
                        {
                            // creat new fork and add Self in to new fork
                            newFork = new Fork(currentFork.Self.ParentTerm);
                            newFork.ChildList.Add(currentFork);
                            currentFork = newFork;
                        }
                        else
                        {
                            tempFork.ChildList.Add(currentFork);
                            currentFork = tempFork;
                            //break;
                        }
                    }
                    if (!root.ChildList.Contains(currentFork))
                        root.ChildList.Add(currentFork);
                }
            }
            #endregion

            return root.ChildList;
           
        }
    }
}
