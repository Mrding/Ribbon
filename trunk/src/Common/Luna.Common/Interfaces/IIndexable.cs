using System;
using System.Collections;
using Luna.Core.Extensions;

namespace Luna.Common
{
    public interface IIndexable
    {
        /// <summary>
        /// Priority
        /// </summary>
        int Index { get; set; }

        //IndexingMode Mode { get; set; }
    }

    public static class IndexManager
    {
        public static void Down(IList list, IIndexable item)
        {
            Move(list, item.Index, () => item.Index + 1);
        }

        public static void Up(IList list, IIndexable item)
        {
            Move(list, item.Index, () => item.Index - 1);
        }

        public static void Top(IList list, IIndexable item)
        {
            Move(list, item.Index, () => 0);
        }

        public static void Bottom(IList list, IIndexable item)
        {
            Move(list, item.Index, () => list.Count - 1);
        }

        public static void Move(IList list, int oldIndex, Func<int> computeNewIndex)
        {
            Move(list, oldIndex, computeNewIndex());
        }

        public static void Move(IList list, int oldStartingIndex, int newStartingIndex)
        {
            if (oldStartingIndex < newStartingIndex)
            {
                for (int i = oldStartingIndex + 1; i < newStartingIndex; i++)
                {
                    list[i].As<IIndexable>().Index--;
                }
            }
            else
            {
                for (int i = oldStartingIndex - 1; i > newStartingIndex; i--)
                {
                    list[i].As<IIndexable>().Index++;
                }
            }

            var oldIndexItem = list[oldStartingIndex] as IIndexable;
            var newIndexItem = list[newStartingIndex] as IIndexable;
            oldIndexItem.Index = oldStartingIndex;
            newIndexItem.Index = newStartingIndex;
        }

        public static void Insert(IList list, int index, IIndexable item)
        {
            item.Index = index;
            for (int i = index; i < list.Count - 1; i++)
            {
                list[i].As<IIndexable>().Index++;
            }
            list.Insert(index, item);
        }

        public static void Add(IList list)
        {
            list[list.Count - 1].As<IIndexable>().Index = list.Count - 1;
        }

        public static void Remove(IList list, int removeIndex)
        {
            for (int i = removeIndex; i < list.Count; i++)
            {
                list[i].As<IIndexable>().Index--;
            }
        }
    }

    public interface ICloze
    {
        int LocationIndex { get; set; }
    }

    public enum IndexingMode
    {
        Sequentail,
        Location,
        Default
    }
}
