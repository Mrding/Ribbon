using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using System.ComponentModel;

namespace Luna.Common
{
    public interface IEditingObjectAdapter : IEditableObject
    {
        void BeginNewEdit();

        void ReBeginEdit();

        IEditingObject EditObj { get; set; }
    }

    public class EditingObjectAdapter : IEditingObjectAdapter
    {
        public IEditingObject EditObj { get; set; }

        

        public EditingObjectAdapter(IEditingObject editObj)
        {
            EditObj = editObj;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IEditing obj = (IEditing)sender;
            if (obj.IsEnablePropertyChanged && e.PropertyName != "IsEditing" &&
                e.PropertyName != "IsNew" && e.PropertyName != "IsEnablePropertyChanged")
                obj.IsEditing = true;
        }

        public void BeginNewEdit()
        {
            EditObj.EndEdit();
            EditObj.IsNew = true;
            EditObj.BeginEdit();
            EditObj.IsEditing = false;
            EditObj.PropertyChanged += OnPropertyChanged;
        }

        public void BeginEdit()
        {
            EditObj.IsNew = false;
            EditObj.BeginEdit();
            EditObj.IsEditing = false;
            EditObj.PropertyChanged += OnPropertyChanged;
        }

        public void ReBeginEdit()
        {
            EditObj.EndEdit();
            EditObj.BeginEdit();
            EditObj.IsEditing = false;
            EditObj.IsNew = false;
        }

        public void EndEdit()
        {
            EditObj.IsNew = false;
            EditObj.EndEdit();
            EditObj.IsEditing = false;
            EditObj.PropertyChanged -= OnPropertyChanged;
        }

        public void CancelEdit()
        {
            EditObj.CancelEdit();
            EditObj.IsEditing = false;
        }
    }
}
