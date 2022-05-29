using NMediator.Core.Routing;
using NMediator.Core.SInjector;
using System;
using System.Collections.Generic;

namespace NMediator.Core.Transport.Decorator
{
    public class TransportOptionsConfiguration
    {
        private readonly Container _container;
        private readonly Func<IOutgoingTransport> _transport;
        private readonly IEnumerable<Type> _types;
        private readonly ICollection<Func<IOutgoingTransport, IOutgoingTransport>> _decorators = new List<Func<IOutgoingTransport, IOutgoingTransport>>();

        public TransportOptionsConfiguration(Container container, Func<IOutgoingTransport> transport, IEnumerable<Type> types)
        {
            this._container = container;
            this._transport = transport;
            _types = types;
        }

        public TransportOptionsConfiguration Retry(int retryTimes)
        {
            _decorators.Add(ctx => new RetryOutgoingTransportDecorator(ctx, retryTimes));
            return this;
        }

        internal void Configure()
        {
            var router = _container.Get<TypeBasedRouter>();
            var transport = _transport();
            _decorators.ForEach(d => transport = d(transport));
            _types.ForEach(t => router.AddRoute(t, transport));
        }
    }
}
