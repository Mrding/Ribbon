using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Core;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework
{
    public class ViewStrategy : DefaultViewStrategy
    {
        public ViewStrategy(IAssemblySource assemblySource, IServiceLocator serviceLocator)
            : base(assemblySource, serviceLocator)
        {
        }

        protected override IEnumerable<string> GetTypeNamesToCheck(Type modelType, string context)
        {
            //if (context == "BlankView") return new[] { "Luna.GUI.Views.BlankView" };

            var className = modelType.Name.Substring(0, modelType.Name.IndexOf("Presenter")) + "View";

            if (string.IsNullOrEmpty(context))
                return new[] { GetFullClassName(modelType, className) };

            return new[] { GetFullClassName(modelType, context), GetFullClassName(modelType, className) };
        }

        private static string GetFullClassName(Type modelType, string className)
        {
            var fullClassName = Assembly.GetEntryAssembly() == modelType.Assembly ? string.Format("Luna.GUI.Views.{0}", className) :
                            string.Format("Luna.GUI.Views.{0}.{1}", modelType.Namespace.SaftyGetProperty(n => n.Split('.')[1]), className);
            return fullClassName;
        }
    }





}