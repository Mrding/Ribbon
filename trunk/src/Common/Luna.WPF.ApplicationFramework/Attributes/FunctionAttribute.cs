using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Filters;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Core;

namespace Luna.WPF.ApplicationFramework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FunctionAttribute : Attribute, IPreProcessor
    {
        public FunctionAttribute(string functionKey)
            : this(functionKey, null)
        { }

        public FunctionAttribute(string functionKey, IAvailabilityEffect effect)
        {
            FunctionKey = functionKey;
            Effect = effect;
        }

        public string FunctionKey
        { get; set; }

        public IAvailabilityEffect Effect
        { get; set; }

        public int Priority
        {
            get { return -1; }
        }

        public bool AffectsTriggers
        {
            get { return true; }
        }

        public bool Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            message.AvailabilityEffect = Effect;
            
            var functionKeys = ApplicationCache.Get<ICollection<string>>(Global.LoginUserFunctionKeys);
            if (functionKeys == null || functionKeys.Count == 0)
            {
                //Admin
                return true;
            }
            return functionKeys.Contains(FunctionKey);
        }
    }
}
