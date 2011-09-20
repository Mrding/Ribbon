namespace Luna.Infrastructure.Domain
{
    public struct AcdSkill
    {
        public AcdSkill(Skill skill, int agentAcdid,string color) : this()
        {
            Skill = skill;
            AgentAcdid = agentAcdid;
            _color = color;
        }

        public Skill Skill { get; set; }

        public int AgentAcdid { get; set; }

        private string _color;
        public string Color
        {
            get
            {
                return string.IsNullOrEmpty(_color) ? "DarkGray" : _color;
            }
            set { _color = value; }
        }
    }
}