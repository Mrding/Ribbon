namespace Caliburn.Core.Metadata
{
    using System;

    /// <summary>
    /// An attribute that gives directions to Caliburn concerning component registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class RegisterAttribute : Attribute, IMetadata
    {
        /// <summary>
        /// Registers the type with the specified container.
        /// </summary>
        /// <param name="decoratedType">The decorated type.</param>
        public abstract ComponentInfo GetComponentInfo(Type decoratedType);

        /// <summary>
        /// Gets a value indicating whether [should register].
        /// </summary>
        /// <value><c>true</c> if [should register]; otherwise, <c>false</c>.</value>
        public virtual bool ShouldRegister
        { get { return true; } }
    }
}