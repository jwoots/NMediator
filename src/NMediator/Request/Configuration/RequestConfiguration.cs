using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Extensions;
using NMediator.InProcess;
using NMediator.Routing;
using NMediator.SInjector;
using NMediator.Transport;
using NMediator.Transport.Decorator;
using System;
using System.Collections.Generic;

namespace NMediator.Request.Configuration
{
    public class RequestConfiguration : BaseConfiguration
    {
        private readonly Container _container;
        private Type[] _types;
        private readonly ICollection<Func<IOutgoingTransport,IOutgoingTransport>> _decorators = new List<Func<IOutgoingTransport,IOutgoingTransport>>();

        public RequestConfiguration(Container container)
        {
            _container = container;
        }

        public RequestConfiguration ExecuteWithInProcess(params Type[] types)
        {
            _types = types;
            
            return this;
        }

        public RequestConfiguration Retry(int retryTimes)
        {
            _decorators.Add(ctx => new RetryOutgoingTransportDecorator(ctx, retryTimes));
            return this;
        }

        protected override void Resolve()
        {
            var router = _container.Get<TypeBasedRouter>();
            IOutgoingTransport transport = new InProcessTransport(_container.Get<ITransportLevelHandlerExecutor>());
            _decorators.ForEach(d => transport = d(transport));
            _types.ForEach(t => router.AddRoute(t, transport));
        }
    }
}
