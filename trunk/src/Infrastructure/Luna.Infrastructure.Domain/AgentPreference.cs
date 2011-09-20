using System.Collections.Generic;
using System.Text;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class AgentPreference : Entity
    {
        public virtual Employee Agent { get; set; } 
        public virtual int LegalHoliday { get; set; }
        public virtual IList<int> WeekDay
        {
            get
            {
                return StringToList(WeekDayPerfect);
            }
        }
        public virtual IList<int> MonthDay
        {
            get
            {
                return StringToList(MonthDayPerfect);
            }
        }
        public virtual IList<int> TimeIntervel
        {
            get
            {
                return StringToList(TimeIntervelPerfect);
            }
        }

        public virtual string WeekDayPerfect { get; set; }
        public virtual string MonthDayPerfect { get; set; }
        public virtual string TimeIntervelPerfect { get; set; }


        public virtual IList<int> StringToList(string perfect)
        {
            var weekOrMonth = new List<int>();
            if (string.IsNullOrEmpty(perfect))
                return weekOrMonth;
            foreach (var item in perfect.Split(new[] { ',' }))
            {
                int flag;
                if (int.TryParse(item, out flag))
                    weekOrMonth.Add(flag);
            }

            return weekOrMonth;
        }

        public virtual string ToDayString(IList<int> lb)
        {
            var sb = new StringBuilder();
            foreach (var item in lb)
                sb.AppendFormat("{0},", item);
            return sb.ToString();
        }
    }
}
