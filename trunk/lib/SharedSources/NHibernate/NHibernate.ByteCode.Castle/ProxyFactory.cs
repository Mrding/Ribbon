using System;
using Castle.DynamicProxy;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernate.ByteCode.Castle
{
	public class ProxyFactory : AbstractProxyFactory
	{
		protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (ProxyFactory));
		private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

		protected static ProxyGenerator DefaultProxyGenerator
		{
			get { return ProxyGenerator; }
		}

		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod,
				                                            SetIdentifierMethod, ComponentIdType, session);

				object generatedProxy = IsClassProxy
				                        	? ProxyGenerator.CreateClassProxy(PersistentClass, Interfaces, initializer)
				                        	: ProxyGenerator.CreateInterfaceProxyWithoutTarget(Interfaces[0], Interfaces,
				                        	                                                    initializer);

				initializer._constructed = true;
				return (INHibernateProxy) generatedProxy;
			}
			catch (Exception e)
			{
				log.Error("Creating a proxy instance failed", e);
				throw new HibernateException("Creating a proxy instance failed", e);
			}
		}


		public override object GetFieldInterceptionProxy()
		{
			var proxyGenerationOptions = new ProxyGenerationOptions();
			var interceptor = new LazyFieldInterceptor();
			proxyGenerationOptions.AddMixinInstance(interceptor);
			return ProxyGenerator.CreateClassProxy(PersistentClass, proxyGenerationOptions, interceptor);
		}
	}
}