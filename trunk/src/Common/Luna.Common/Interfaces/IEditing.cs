namespace Luna.Common
{
    public interface IEditing
    {
        bool IsNew
        { get; set; }

        bool IsEditing
        { get; set; }

        bool IsEnablePropertyChanged
        { get; set; }
    }


}
