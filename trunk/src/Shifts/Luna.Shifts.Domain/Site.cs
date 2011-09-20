using Luna.Common;

namespace Luna.Shifts.Domain
{
    /// <summary>
    /// Site of sests
    /// </summary>
    public class Site : Entity
    {
        public virtual string Description { get; set; }

        public override string GetUniqueKey()
        {
            return Name;
        }
    }
}
