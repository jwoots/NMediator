using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Extensions;
using NMediator.InProcess;
using NMediator.Routing;
using NMediator.SInjector;
using NMediator.Transport;
using System;

namespace NMediator.Request.Configuration
{
    public class RequestConfiguration : BaseConfiguration
    {
        private readonly Container _container;
        private Type[] _types;

        public RequestConfiguration(Container container)
        {
            _container = container;
        }

        public RequestConfiguration ExecuteWithInProcess(params Type[] types)
        {
            _types = types;
            
            return this;
        }

        protected override void Resolve()
        {
            var router = _container.Get<TypeBasedRouter>();
            var transport = new InProcessTransport(_container.Get<ITransportLevelHandlerExecutor>());
            _types.ForEach(t => router.AddRoute(t, transport));
        }
    }
}
