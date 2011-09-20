using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Core;
using Luna.Infrastructure.Domain;


namespace Luna.Shifts.Domain
{
   
    public class AgentGroup : List<IAgent>
    {
        //for matching use
        private List<ServiceQueue> _validSQ;

        private List<ServiceQueue> _groupSQ;

        public AgentGroup(List<ServiceQueue> validSQ)
        {
            _validSQ = validSQ;
            _groupSQ = new List<ServiceQueue>();
            ParameterIndex = new Dictionary<ServiceQueue, int>();
        }

        //equals Group Key Property
        public List<ServiceQueue> GroupSQ { get { return _groupSQ; } }

        public Dictionary<ServiceQueue, int> ParameterIndex { get; set; }

        public int GetParameterIndexOf(ServiceQueue sq)
        {
            if (!_groupSQ.Contains(sq))
                return -1;
            if (ParameterIndex == null
                || !ParameterIndex.Keys.Contains(sq))
                return -1;
            return ParameterIndex[sq];
        }


        //5 min index
        public int GetOnServiceNumber(int index)
        {
            int result = 0;
            foreach (var ag in this) //_groupAgent
            {
                int onService =
                    GetOnServiceTimeFromAgent(index * 3, index * 3 + 3, ag);
                if (onService != 0)
                    result++;
            }
            return result;
        }


        


        public static int GetOnServiceTimeFromAgent(int index_start, int index_end, IAgent emp)
        {
            if (index_start > emp.Onlines.Length || index_end > emp.Onlines.Length)
            {
                return 0;
            }
            int online = 0;
            for (int i = index_start; i < index_end; i++)
            {
                if (emp.Onlines[i])
                {
                    online += 5;
                }
            }
            return online;
        }



        public void AddParameter(int index, ServiceQueue sq)
        {
            if (ParameterIndex.Keys.Contains(sq))
            {
                ParameterIndex[sq] = index;
            }
            else
            {
                ParameterIndex.Add(sq, index);
            }
        }


    }
}
