using System;
using System.Linq;
using System.Reflection;
using Caliburn.Core.Invocation;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Actions;
using Luna.WPF.ApplicationFramework.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Castle.DynamicProxy;

namespace Luna.WPF.ApplicationFramework.Attributes
{
    public class QuestionAttribute : BeforeAttribute, Caliburn.PresentationFramework.Filters.IInitializable, IBeforeProcessor
    {
        private readonly string _callback;
        private IMethod _prepareMethod;
        private IMethod _callbackMethod;

        protected QuestionAttribute()
        { }

        public QuestionAttribute(string prepare)
            : this(prepare, string.Empty)
        { }

        public QuestionAttribute(string prepare, string callback)
            : base(prepare)
        {
            _callback = callback;
            Priority = -1;
        }

        public int Priority { get; set; }

        public bool AffectsTriggers { get { return true; } }

        public void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            _methodName.AsMethodInfo(targetType).SaftyInvoke(m => _prepareMethod = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(m));
            _callback.AsMethodInfo(targetType).SaftyInvoke(m => _callbackMethod = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(m));
        }

        private void Process(string methodName, object[] param)
        {
            
        }
        
        public override bool Predicate(IInvocation invocation)
        {
            //此方法只能由代理对象执行
            var questionPresenter = ServiceLocator.Current.GetInstance<IQuestionPresenter>();
            var param = invocation.Arguments.Add(questionPresenter).ToArray();

            //Prepare
            if (_methodName.IsNotNullOrEmpty())
            {
                var returnType = invocation.InvocationTarget.GetType().GetMethod(_methodName).ReturnType;
                if (returnType == typeof(void))
                    Action(invocation, _methodName, param);
                else if (returnType == typeof(bool))
                {
                    var canContinue = Func<bool>(invocation, _methodName, param);
                    if (!canContinue) return true;
                }
                else throw new NotSupportedException();
            }

            //Show
            ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(questionPresenter);

            //Callback
            if (_callback.IsNotNullOrEmpty())
                return Func<bool>(invocation, _callback, param);
            return true;
        }

        public bool BeforeExecute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            var methodName = message.As<ActionMessage>().MethodName;

            var questionPresenter = ServiceLocator.Current.GetInstance<IQuestionPresenter>();
            var param = parameters.Add(questionPresenter).ToArray();
            var target = handlingNode.MessageHandler.Unwrap();
            var targetParams = target.GetType().GetMethod(methodName).GetParameters();

            //Prepare
            if (_prepareMethod.IsNotNull())
            {
                var prepareParams = InvocationExtension.GetArguments(param, targetParams, _prepareMethod.Info.GetParameters());

                var prepareResult = _prepareMethod.Invoke(target, prepareParams);
                if (prepareResult != null)
                {
                    if (prepareResult is bool)
                        return (bool) prepareResult;
                }
            }

            //Show
            ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(questionPresenter);

            //Callback
            if (_callbackMethod.IsNull())
                return questionPresenter.Answer == Answer.Yes;

            var callbackParams = InvocationExtension.GetArguments(param, targetParams, _callbackMethod.Info.GetParameters());
            var result = _callbackMethod.Invoke(target, callbackParams);

            if (_callbackMethod.Info.ReturnType == typeof(bool)) return (bool)result;
            return true;
        }

        bool Caliburn.PresentationFramework.Filters.IPreProcessor.Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            // 因为 IBeforeProcessor 也继承 IPreProcessor, 为了让 CustomSynchronousAction 避免执行 2 次, 此方法空跑不做任何事情.
            // 交由 IBeforeProcessor 的实例 BeforeExecute 执行
            return true;
        }
    }
}
