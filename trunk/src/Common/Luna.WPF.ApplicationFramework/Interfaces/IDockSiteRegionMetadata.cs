using System;
using System.Windows;
using ActiproSoftware.Windows;
using ActiproSoftware.Windows.Controls.Docking;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// An interface that provides metadata to the IDockSitePresenter
    /// describing how a view should be presented (e.g. as a document window or a tool window),
    /// as well as the docking window's capabilities.
    /// </summary>
    public interface IDockingWindowMetadata
    {
        /// <summary>
        /// Gets the default dock location of the view.
        /// </summary>
        /// <value>The default dock location.</value>
        DockSiteDock DefaultDock { get; }

        /// <summary>
        /// Gets a value indicating whether the view should be presented as a tool window.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tool window; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>; by default, views will be presented as document windows.
        /// </remarks>
        bool IsToolWindow { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can close.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can close; otherwise, <c>false</c>.</value>
        bool CanClose { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the left.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can dock to the left; otherwise, <c>false</c>.</value>
        bool CanDockLeft { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the right.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can dock to the right; otherwise, <c>false</c>.</value>
        bool CanDockRight { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the top.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can dock to the top; otherwise, <c>false</c>.</value>
        bool CanDockTop { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the bottom.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can dock to the bottom; otherwise, <c>false</c>.</value>
        bool CanDockBottom { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can tear off as a rafting window.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can tear off as a rafting window; otherwise, <c>false</c>.</value>
        bool CanRaft { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can be dragged to a new location.
        /// </summary>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can be dragged to a new location; otherwise, <c>false</c>.</value>
        bool CanDrag { get; }

        /// <summary>
        /// Gets a value indicating whether a view's rafted <see cref="DockingWindow"/> can be docked.
        /// </summary>
        /// <value><c>true</c> if the rafted <see cref="DockingWindow"/> can be docked; otherwise, <c>false</c>.</value>
        bool CanAttach { get; }

        /// <summary>
        /// Gets a value indicating whether a view's docked <see cref="DockingWindow"/> can be auto-hidden.
        /// </summary>
        /// <value><c>true</c> if the docked <see cref="DockingWindow"/> can be auto-hidden; otherwise, <c>false</c>.</value>
        bool CanAutoHide { get; }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can be docked as a document.
        /// </summary>
        /// <value>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can be docked as a document; otherwise, <c>false</c>.</value>
        /// </value>
        bool CanBecomeDocument { get; }

        /// <summary>
        /// Gets the title to apply to the view's <see cref="DockingWindow"/>.
        /// </summary>
        /// <value>The title to apply to the view's <see cref="DockingWindow"/>.</value>
        string Title { get; }

        /// <summary>
        /// CreateNewDockingGroup
        /// </summary>
        bool CreateNewDockingGroup { get; }

        /// <summary>
        /// Undock
        /// </summary>
        bool Undock { get; }

        Size DefaultSize { get; }
      
 
    }
}