using System.Collections.Generic;
using Iesi.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class ServiceQueue : Entity
    {
        public ServiceQueue()
        {
            //AcdQueues = new HashedSet<AcdQueue>();
        }

        public virtual string Description{ get; set;}

        /// <summary>
        /// 放棄率（預測使用）
        /// </summary>
        public virtual int AbandonRate { get; set; }

        public virtual Campaign Campaign { get; set; } 

        public virtual Skill MappedSkill { get; set; }

        public virtual bool ForSingleSkillUse { get; set; }

        public virtual int MinimumOnlines { get; set; }

        //public virtual ICollection<AcdQueue> AcdQueues { get; set; }

        public override string GetUniqueKey()
        {
            return Name;
        }

        public override bool CustomEquals(object other)
        {
            var otherSkill = other as Skill;
            if (otherSkill == null) return false;

            return MappedSkill.Equals(otherSkill);
        }
    }
}
