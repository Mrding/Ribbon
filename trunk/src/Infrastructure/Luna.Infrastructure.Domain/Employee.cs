using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class Employee : Entity, ISimpleEmployee
    {
        private double _productivity = 1;
        private IDictionary<Skill, double> _skills;

        public virtual string Name2 { get; set; }
        public virtual DateTime EnrollmentDate { get; set; }
        public virtual DateTime? LeavingDate { get; set; }
        // 正职\委外
        public virtual bool EmployeeTag1 { get; set; }
        // 黑名单
        public virtual bool EmployeeTag2 { get; set; }
        public virtual string AgentId { get; set; }
        public virtual bool IsAgent { get; set; }
        public virtual int Rank { get; set; }
        public virtual string EmployeeType1 { get; set; }
        public virtual string EmployeeType2 { get; set; }
        public virtual string EmployeeType3 { get; set; }
        public virtual string Email { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Password { get; set; }

        public virtual double Productivity
        {
            get { return _productivity; }
            set { _productivity = value; }
        }

        public virtual Entity CustomLaborRule { get; set; }

        LaborRule ISimpleEmployee.LaborRule
        {
            get
            {
                //todo 当CustomLaborRule为null时候，会查询Organization
                return CustomLaborRule as LaborRule ?? ((Organization)Organization).LaborRule;
            }
            set { CustomLaborRule = value; }
        }

        public virtual IDictionary<Skill, double> Skills
        {
            get
            {
                //todo 需要修改
                if (_skills == null)
                {
                    var distinctSkills = _skillMap.Select(o => o.Key.Skill).Distinct();
                    _skills = _skillMap == null ? new Dictionary<Skill, double>() : distinctSkills.ToDictionary(o => o, o => 1d);
                }

                return _skills;
            }
        }

        private IDictionary<AcdSkill, double> _skillMap = new Dictionary<AcdSkill, double>();
        protected virtual IDictionary<AcdSkill, double> SkillMap
        {
            get { return _skillMap; }
            set { _skillMap = value; }
        }



        public virtual bool AddSkill(Skill skill, int agentAcdid, string color, double productivity)
        {
            var colorValue = color;
            var productivityValue = productivity;
            if (_skillMap.Keys.Any(o => o.AgentAcdid == agentAcdid && o.Skill.Equals(skill)))
            {
                var exist = _skillMap.First(o => o.Key.AgentAcdid == agentAcdid && o.Key.Skill.Equals(skill));
                colorValue = exist.Key.Color;
                productivityValue = exist.Value;
                _skillMap.Remove(exist.Key);
            }

            _skillMap.Add(new AcdSkill(skill, agentAcdid, colorValue), productivityValue);
            _skills.Clear();
            return true;
        }


        private string[] _agentAcdids;
        public virtual string[] AgentAcdids
        {
            get
            {
                if (_agentAcdids == null)
                {
                    _agentAcdids = _skillMap.Keys.Select(o => o.AgentAcdid.ToString()).Distinct().ToArray();
                }
                return _agentAcdids;
            }
        }

        public virtual Entity Organization { get; set; }

        //語言專長Language,通訊地址Address1,戶籍地址Address2

        public virtual string Language { get; set; }

        public virtual string Address1 { get; set; }

        public virtual string Address2 { get; set; }

        public override string GetUniqueKey()
        {
            return AgentId;
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string GetColorByAcdid(string acdid)
        {
            var agentAcdid = Int32.Parse(acdid);
            return _skillMap.Keys.First(o => o.AgentAcdid == agentAcdid).Color;
        }
    }
}
