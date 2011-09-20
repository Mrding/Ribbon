using System.ComponentModel;

namespace Luna.Common
{
    public interface IEditingObject : IEditableObject, IEditing, INotifyPropertyChanged
    {
    }
}