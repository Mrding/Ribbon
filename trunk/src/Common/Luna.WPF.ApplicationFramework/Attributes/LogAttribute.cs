using System;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Domain;
using Luna.Common.Model;
using Luna.Core.Extensions;
using Luna.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LogAttribute : Attribute, IInitializable, IPostProcessor
    {
        public LogAttribute(): this(string.Empty)
        {
        }

        public LogAttribute(string key)
        {
            LanguageKey = key;
        }

        public string Message { get; set; }

        public string LanguageKey { get; set; }

        public int Priority { get; set; }

        public void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
        }

        public void Execute(IRoutedMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            var content = LanguageKey.IsNullOrEmpty() ? Message : LanguageReader.GetValue(LanguageKey);
            ServiceLocator.Current.GetInstance<IAuditLogModel>().Write(new AuditLog { Action = content, CurrentUser = ApplicationCache.Get<string>(Global.LoggerId) });
        }
    }
}