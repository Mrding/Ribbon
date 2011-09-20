using System;
using System.Windows;
using ActiproSoftware.Windows;
using ActiproSoftware.Windows.Controls.Docking;

namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// The standard <see cref="IDockingWindowMetadata"/> implementation.
    /// </summary>
    /// <remarks>
    /// Usable in Xaml, this class provides metadata to the IDockSitePresenter
    /// describing how a view should be presented (e.g. as a document window or a tool window),
    /// as well as the docking window's capabilities.
    /// </remarks>
    public class DockingWindowMetadata : Freezable, IDockingWindowMetadata
    {
        public static readonly DependencyProperty CanCloseProperty;
        public static readonly DependencyProperty CanDockLeftProperty;
        public static readonly DependencyProperty CanDockRightProperty;
        public static readonly DependencyProperty CanDockTopProperty;
        public static readonly DependencyProperty CanDockBottomProperty;
        public static readonly DependencyProperty CanDragProperty;
        public static readonly DependencyProperty CanAutoHideProperty;
        public static readonly DependencyProperty CanRaftProperty;
        public static readonly DependencyProperty CanAttachProperty;
        public static readonly DependencyProperty CanBecomeDocumentProperty;
        public static readonly DependencyProperty DefaultDockProperty;
        public static readonly DependencyProperty IsToolWindowProperty;
        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyProperty CreateNewDockingGroupProperty;
        public static readonly DependencyProperty UndockProperty;
        public static readonly DependencyProperty DefaultSizeProperty;

        public static readonly DependencyProperty InstanceProperty;


        static DockingWindowMetadata()
        {
            CanDragProperty = DependencyProperty.Register(
                "CanDrag",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanAttachProperty = DependencyProperty.Register(
                "CanAttach",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanCloseProperty = DependencyProperty.Register(
                "CanClose",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            IsToolWindowProperty = DependencyProperty.Register(
                "IsToolWindow",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(
                    false,
                    IsToolWindowChanged));

            CanDockLeftProperty = DependencyProperty.Register(
                "CanDockLeft",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanDockRightProperty = DependencyProperty.Register(
                "CanDockRight",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanDockTopProperty = DependencyProperty.Register(
                "CanDockTop",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanDockBottomProperty = DependencyProperty.Register(
                "CanDockBottom",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(true));

            CanAutoHideProperty = DependencyProperty.Register(
                "CanAutoHide",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(
                    true,
                    null,
                    CoerceToolWindowSpecificBoolProperty));

            CanRaftProperty = DependencyProperty.Register(
                "CanRaft",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(
                    true,
                    null,
                    CoerceToolWindowSpecificBoolProperty));

            CanBecomeDocumentProperty = DependencyProperty.Register(
                "CanBecomeDocument",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(
                    false,
                    null,
                    CoerceToolWindowSpecificBoolProperty));

            DefaultDockProperty = DependencyProperty.Register(
                "DefaultDock",
                typeof(DockSiteDock),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(
                    DockSiteDock.Left,
                    null,
                    CoerceDefaultDock));

            TitleProperty = DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata("untitled"));

            CreateNewDockingGroupProperty = DependencyProperty.Register(
                "CreateNewDockingGroup",
                typeof(bool),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(false));

            UndockProperty = DependencyProperty.Register(
                 "Undock",
                 typeof(bool),
                 typeof(DockingWindowMetadata),
                 new FrameworkPropertyMetadata(false));


            DefaultSizeProperty = DependencyProperty.Register(
                 "DefaultSize",
                 typeof(Size),
                 typeof(DockingWindowMetadata),
                 new FrameworkPropertyMetadata(Size.Empty));


            InstanceProperty = DependencyProperty.RegisterAttached(
                "Instance",
                typeof(DockingWindowMetadata),
                typeof(DockingWindowMetadata),
                new FrameworkPropertyMetadata(null));
        }

        private static object CoerceDefaultDock(DependencyObject d, object baseValue)
        {
            if (Equals(d.GetValue(IsToolWindowProperty), true))
            {
                if (Equals(baseValue, DockSiteDock.Content))
                    return DockSiteDock.Left;
                return baseValue;
            }
            if (Equals(baseValue, DependencyProperty.UnsetValue))
                return DockSiteDock.Content;
            return baseValue;
        }

        private static object CoerceToolWindowSpecificBoolProperty(DependencyObject d, object baseValue)
        {
            if (Equals(d.GetValue(IsToolWindowProperty), false))
                return false;
            return baseValue;
        }


        private static void IsToolWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(CanRaftProperty);
            d.CoerceValue(CanBecomeDocumentProperty);
            d.CoerceValue(DefaultDockProperty);
        }

        /// <summary>
        /// Gets the <see cref="DockingWindowMetadata"/> for a given view.
        /// </summary>
        /// <param name="d">The view.</param>
        /// <returns>The <see cref="DockingWindowMetadata"/> for the view.</returns>
        public static DockingWindowMetadata GetInstance(DependencyObject d)
        {
            return d.GetValue(InstanceProperty) as DockingWindowMetadata;
        }

        /// <summary>
        /// Sets the <see cref="DockingWindowMetadata"/> for a given view.
        /// </summary>
        /// <param name="d">The view.</param>
        /// <param name="value">The <see cref="DockingWindowMetadata"/> to apply to the view.</param>
        public static void SetInstance(DependencyObject d, DockingWindowMetadata value)
        {
            d.SetValue(InstanceProperty, value);
        }

        #region IDockingWindowMetadata Implementation
        /// <summary>
        /// Gets a value indicating whether a view's rafted <see cref="DockingWindow"/> can be docked.
        /// </summary>
        /// <value>
        /// <c>true</c> if the rafted <see cref="DockingWindow"/> can be docked; otherwise, <c>false</c>.
        /// </value>
        public bool CanAttach
        {
            get { return (bool)GetValue(CanAttachProperty); }
            set { SetValue(CanAttachProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's docked <see cref="DockingWindow"/> can be auto-hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if the docked <see cref="DockingWindow"/> can be auto-hidden; otherwise, <c>false</c>.
        /// </value>
        public bool CanAutoHide
        {
            get { return (bool)GetValue(CanAutoHideProperty); }
            set { SetValue(CanAutoHideProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can be dragged to a new location.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can be dragged to a new location; otherwise, <c>false</c>.
        /// </value>
        public bool CanDrag
        {
            get { return (bool)GetValue(CanDragProperty); }
            set { SetValue(CanDragProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can tear off as a rafting window.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can tear off as a rafting window; otherwise, <c>false</c>.
        /// </value>
        public bool CanRaft
        {
            get { return (bool)GetValue(CanRaftProperty); }
            set { SetValue(CanRaftProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can close.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can close; otherwise, <c>false</c>.
        /// </value>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the left.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can dock to the left; otherwise, <c>false</c>.
        /// </value>
        public bool CanDockLeft
        {
            get { return (bool)GetValue(CanDockLeftProperty); }
            set { SetValue(CanDockLeftProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the right.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can dock to the right; otherwise, <c>false</c>.
        /// </value>
        public bool CanDockRight
        {
            get { return (bool)GetValue(CanDockRightProperty); }
            set { SetValue(CanDockRightProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the top.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can dock to the top; otherwise, <c>false</c>.
        /// </value>
        public bool CanDockTop
        {
            get { return (bool)GetValue(CanDockTopProperty); }
            set { SetValue(CanDockTopProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can dock to the bottom.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DockingWindow"/> can dock to the bottom; otherwise, <c>false</c>.
        /// </value>
        public bool CanDockBottom
        {
            get { return (bool)GetValue(CanDockBottomProperty); }
            set { SetValue(CanDockBottomProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a view's <see cref="DockingWindow"/> can be docked as a document.
        /// </summary>
        /// <value>
        /// <value><c>true</c> if the <see cref="DockingWindow"/> can be docked as a document; otherwise, <c>false</c>.</value>
        /// </value>
        public bool CanBecomeDocument
        {
            get { return (bool)GetValue(CanBecomeDocumentProperty); }
            set { SetValue(CanBecomeDocumentProperty, value); }
        }

        /// <summary>
        /// Gets the default dock location of the view.
        /// </summary>
        /// <value>The default dock location.</value>
        public DockSiteDock DefaultDock
        {
            get { return (DockSiteDock)GetValue(DefaultDockProperty); }
            set { SetValue(DefaultDockProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the view should be presented as a tool window.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tool window; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is <c>false</c>; by default, views will be presented as document windows.
        /// </remarks>
        public bool IsToolWindow
        {
            get { return (bool)GetValue(IsToolWindowProperty); }
            set { SetValue(IsToolWindowProperty, value); }
        }

        /// <summary>
        /// Gets the title to apply to the view's <see cref="DockingWindow"/>.
        /// </summary>
        /// <value>The title to apply to the view's <see cref="DockingWindow"/>.</value>
        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// CreateNewDockingGroup
        /// </summary>
        public bool CreateNewDockingGroup
        {
            get { return (bool)GetValue(CreateNewDockingGroupProperty); }
            set { SetValue(CreateNewDockingGroupProperty, value); }

        }

        /// <summary>
        /// Undock
        /// </summary>
        public bool Undock
        {
            get { return (bool)GetValue(UndockProperty); }
            set { SetValue(UndockProperty, value); }
        }


        /// <summary>
        /// DefaultSize
        /// </summary>
        public Size DefaultSize
        {
            get { return (Size)GetValue(DefaultSizeProperty); }
            set { SetValue(DefaultSizeProperty, value); }
        }


        #endregion

        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new DockingWindowMetadata();
        }

        #endregion
       
    }
}