using NMediator.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Routing
{
    public class TypeBasedRouter : IRouter
    {
        private IDictionary<Type, IOutgoingTransport> _routes = new Dictionary<Type, IOutgoingTransport>();

        public IOutgoingTransport Route(Type messageType)
        {
            if (_routes.TryGetValue(messageType, out var transport))
                return transport;

            if (messageType.IsGenericType && _routes.TryGetValue(messageType.GetGenericTypeDefinition(), out transport))
                return transport;

            throw new InvalidOperationException($"No route find for message type {messageType}");   
        }

        public void AddRoute(Type messageType, IOutgoingTransport transport)
            => _routes[messageType] = transport;

    }
}
