namespace Luna.WPF.ApplicationFramework.Controls
{
    /// <summary>
    /// A data structure of limited range 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitRange<T>
    {
        /// <summary>
        /// Gets or sets the mininum extent.
        /// </summary>
        /// <value>The min X.</value>
        public T Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum extent.
        /// </summary>
        /// <value>The max.</value>
        public T Max { get; set; }

        public T ViewMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum extent of view.
        /// </summary>
        /// <value>The view max.</value>
        public T ViewMax { get; set; }
    }
}
