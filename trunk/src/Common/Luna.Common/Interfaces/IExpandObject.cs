namespace Luna.Common.Interfaces
{
    public interface IExpandObject
    {
        object ExpandData { get; set; }
    }


    public class ExpandObject : IExpandObject
    {
        public virtual object ExpandData { get; set; }
    }
}
