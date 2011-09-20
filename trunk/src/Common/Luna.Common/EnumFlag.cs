using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    public class EnumFlag<T>
    {
        private readonly IList<string> _list = new List<string>();

        public void FlagDelete(T flag)
        {
            var str = flag.ToString();
            if (_list.Contains(str))
            {
                _list.Remove(str);
            }
        }

        public void FlagAdd(T flag)
        {
            var str = flag.ToString();
            if (_list.Contains(str)) return;
            _list.Add(str);
        }

        public bool FlagExist(T flag)
        {
            var str = flag.ToString();
            return _list.Contains(str);
        }
    }
}
