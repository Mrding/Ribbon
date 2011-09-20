namespace Luna.WPF.ApplicationFramework
{
    /// <summary>
    /// Defines the valid dock locations for views presented by a IDockSitePresenter.
    /// </summary>
    public enum DockSiteDock
    {
        /// <summary>
        /// Describes the dock location to the left of the center content area.
        /// </summary>
        Left,
        /// <summary>
        /// Describes the dock location to the right of the center content area.
        /// </summary>
        Right,
        /// <summary>
        /// Describes the dock location to the top of the center content area.
        /// </summary>
        Top,
        /// <summary>
        /// Describes the dock location to the bottom of the center content area.
        /// </summary>
        Bottom,
        /// <summary>
        /// Describes the center content area.
        /// </summary>
        Content,
    }
}