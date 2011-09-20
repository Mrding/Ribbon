using System.Collections;

namespace Luna.Common
{
    public interface IClozeContainer
    {
        int Capacity { get; }

        ICloze NewItem(int index);

        IEnumerator GetEnumerator();
    }
}
