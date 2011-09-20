namespace Luna.Shifts.Domain
{
    public interface IPair
    {
    }
    public class Pair<T> : IPair
    {
        public Pair(){}
        public Pair(T applier,T replier)
        {
            Applier = applier;
            Replier = replier;
        }
        public T Applier { get; set; }
        public T Replier { get; set; }
    }
}


