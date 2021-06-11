using NMediator.Core.Configuration;
using NMediator.Core.SInjector;
using NMediator.Core.Transport;
using NMediator.Core.Transport.Decorator;
using System;
using System.Collections.Generic;

namespace NMediator.InProcess
{
    public class InProcessConfiguration : BaseConfiguration
    {
        private readonly Container _container;
        private Type[] _types;
        private ICollection<TransportOptionsConfiguration> _transportConfigurations = new List<TransportOptionsConfiguration>();

        public InProcessConfiguration(Container container)
        {
            _container = container;
            _container.Register(ctx => new InProcessTransport(ctx.Get<ITransportLevelHandlerExecutor>()));
            _container.Register(ctx => new InProcessEventTransport(ctx.Get<ITransportLevelHandlerExecutor>()));
        }

        public TransportOptionsConfiguration ExecuteWithInProcess(params Type[] types)
        {
            var transportConfiguration = new TransportOptionsConfiguration(_container, () => _container.Get<InProcessTransport>(), types);
            _transportConfigurations.Add(transportConfiguration);

            return transportConfiguration;
        }

        public TransportOptionsConfiguration PublishInProcess(params Type[] types)
        {
            var transportConfiguration = new TransportOptionsConfiguration(_container, () => _container.Get<InProcessEventTransport>(), types);
            _transportConfigurations.Add(transportConfiguration);

            return transportConfiguration;
        }

        protected override void Resolve()
        {
            _transportConfigurations.ForEach(tc => tc.Configure());
        }
    }
}
