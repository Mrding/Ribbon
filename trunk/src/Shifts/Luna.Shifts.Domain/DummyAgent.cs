using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class DummyAgent : IEnumerable, IIndexable
    {
        private string _dummyNames;

        public DummyAgent(string name, int index, IEnumerable<int> duplications)
        {
            SetDummyName(name);
            Index = index;
            Duplications = duplications;
        }

        public object Profile { get; set; }

        public IEnumerable Duplications { get; set; }

        /// <summary>
        /// Dummy
        /// </summary>
        /// <param name="dummyNames"></param>
        /// /// <param name="index"></param>
        public DummyAgent(string[] dummyNames, int index)
        {
            SetDummyName(string.Join(",", dummyNames));
            Index = index;
        }

        private void SetDummyName(string name)
        {
            _dummyNames = name == "," ? string.Empty : name;
        }

        public int Index { get; set; }



        public override string ToString()
        {
            return _dummyNames;
        }

        public IEnumerator GetEnumerator()
        {
            return new List<ITerm>().GetEnumerator();
        }
    }
}