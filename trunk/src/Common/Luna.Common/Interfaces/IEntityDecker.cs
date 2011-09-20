namespace Luna.Common.Interfaces
{
    public interface IEntityDecker
    {
        T GetEntity<T>() where T : Entity;
    }
}