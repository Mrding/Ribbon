using Luna.Common;


namespace Luna.Infrastructure.Domain
{
    public class AgentStatusAlertTime
    {
        protected AgentStatusAlertTime()
        {
            
        }
        public AgentStatusAlertTime(Entity area, AgentStatusType type)
        {
            Area = area;
            Type = type;
            AlertTimeOutSecond = type.AlertTimeOutSecond;
            AlertTimeOutSecond2 = type.AlertTimeOutSecond2;
        }


        public virtual int Id { get; set; }

        public virtual Entity Area { get; set; }

        public virtual AgentStatusType Type { get; set; }


        private int _alertTimeOutSecond = 0;
        public virtual int AlertTimeOutSecond
        {
            get { return _alertTimeOutSecond; }
            set
            {
                
                _alertTimeOutSecond = value;
            }
        }

        private int _alertTimeOutSecond2 = 0;
        public virtual int AlertTimeOutSecond2
        {
            get { return _alertTimeOutSecond2; }
            set
            {
                _alertTimeOutSecond2 = value;
            }
        }

    }
}
