using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Filters;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public interface IBeforeProcessor : IPreProcessor
    {
        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <returns></returns>
        bool BeforeExecute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters);
    }
}
