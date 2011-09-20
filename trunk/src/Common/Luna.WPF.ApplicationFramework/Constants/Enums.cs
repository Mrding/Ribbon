using System;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// FXMessageBox choice
    /// </summary>
    [Flags]
    public enum MessageChoice
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 4,
        No = 8,
        Help = 16,
    }

    /// <summary>
    /// FXMessageBox Icons
    /// </summary>
    public enum MessageIcon
    {
        None,
        Warning,
        Error,
        Question,
        Information
    }
}