using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.Common
{
    public enum EditStatus
    {
        Inserting,
        Inserted,
        Saving,
        Saved,
        Deleting,
        Delete
    }

    public class CanEditStatus
    {
        public bool CanInsert { get; set; }

        public bool CanSave { get; set; }

        public bool CanDelete { get; set; }
    }
}
