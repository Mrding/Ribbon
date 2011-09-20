using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;
using System;
using Castle.Windsor;

namespace Luna.Data.Transform
{
    public class ResultTransformer<T> : IResultTransformer
    {
        private readonly Func<object[],Dictionary<string, object>> _dictionaryInstanceCreation;
        public ResultTransformer(Func<object[],Dictionary<string, object>> dictionaryInstanceCreation)
        {
            _dictionaryInstanceCreation = dictionaryInstanceCreation;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return Container.Resolve(typeof (T), _dictionaryInstanceCreation(tuple));
        }
        public IList TransformList(IList collection)
        {
            return collection;
        }

        private static IWindsorContainer Container
        {
            get { return ((IContainerAccessor)Microsoft.Practices.ServiceLocation.ServiceLocator.Current).Container; }
        }
    }

    public class ResultTransformer : IResultTransformer
    {
        private readonly Type _resloveType;
        private readonly Func<object[], Dictionary<string, object>> _dictionaryInstanceCreation;
        public ResultTransformer(Func<object[], Dictionary<string, object>> dictionaryInstanceCreation, Type resloveType)
        {
            _dictionaryInstanceCreation = dictionaryInstanceCreation;
            _resloveType = resloveType;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return Container.Resolve(_resloveType, _dictionaryInstanceCreation(tuple));
        }
        public IList TransformList(IList collection)
        {
            return collection;
        }

        private static IWindsorContainer Container
        {
            get { return ((IContainerAccessor)Microsoft.Practices.ServiceLocation.ServiceLocator.Current).Container; }
        }
    }
}