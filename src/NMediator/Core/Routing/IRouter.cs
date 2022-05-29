using NMediator.Core.Transport;
using System;

namespace NMediator.Core.Routing
{
    interface IRouter
    {
        IOutgoingTransport Route(Type messageType);
    }
}
