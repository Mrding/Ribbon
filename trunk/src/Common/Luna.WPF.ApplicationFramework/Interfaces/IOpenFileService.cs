using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// This interface defines a interface that will allow 
    /// a ViewModel to open a file
    /// </summary>
    public interface IOpenFileService
    {
        /// <summary>
        /// FileName
        /// </summary>
        String FileName { get; set; }

        /// <summary>
        /// Filter
        /// </summary>
        String Filter { get; set; }

        /// <summary>
        /// Gets or sets the default ext.
        /// </summary>
        /// <value>The default ext.</value>
        String DefaultExt { get; set; }

        /// <summary>
        /// Filter
        /// </summary>
        String InitialDirectory { get; set; }

        /// <summary>
        /// This method should show a window that allows a file to be selected
        /// </summary>
        /// <param name="model">The owner presenter of the dialog</param>
        /// <returns>A bool from the ShowDialog call</returns>
        bool? ShowDialog(object model);
    }

    
}
