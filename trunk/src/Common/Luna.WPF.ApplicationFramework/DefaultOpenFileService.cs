using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
using Microsoft.Win32;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// This class implements the IOpenFileService for WPF purposes.
    /// </summary>
    public class DefaultOpenFileService : IOpenFileService
    {
        #region Data

        /// <summary>
        /// Embedded OpenFileDialog to pass back correctly selected
        /// values to ViewModel
        /// </summary>
        private OpenFileDialog ofd = new OpenFileDialog();

        #endregion

        #region IOpenFileService Members
        /// <summary>
        /// This method should show a window that allows a file to be selected
        /// </summary>
        /// <param name="model">The owner presenter of the dialog</param>
        /// <returns>A bool from the ShowDialog call</returns>
        public bool? ShowDialog(object model)
        {
            //Set embedded OpenFileDialog.Filter
            if (!String.IsNullOrEmpty(this.Filter))
                ofd.Filter = this.Filter;

            //Set embedded OpenFileDialog.InitialDirectory
            if (!String.IsNullOrEmpty(this.InitialDirectory))
                ofd.InitialDirectory = this.InitialDirectory;

            //return results
            
            return ofd.ShowDialog();
        }

        /// <summary>
        /// FileName : Simply use embedded OpenFileDialog.FileName
        /// But DO NOT allow a Set as it will ONLY come from user
        /// picking a file
        /// </summary>
        public string FileName
        {
            get { return ofd.FileName; }
            set
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Filter : Simply use embedded OpenFileDialog.Filter
        /// </summary>
        public string Filter
        {
            get { return ofd.Filter; }
            set { ofd.Filter = value; }
        }

        /// <summary>
        /// Gets or sets the default ext.
        /// </summary>
        /// <value>The default ext.</value>
        public string DefaultExt
        {
            get { return ofd.DefaultExt; }
            set { ofd.DefaultExt = value; }
        }

        /// <summary>
        /// Filter : Simply use embedded OpenFileDialog.InitialDirectory
        /// </summary>
        public string InitialDirectory
        {
            get { return ofd.InitialDirectory; }
            set { ofd.InitialDirectory = value; }
        }

        #endregion
    }
}
