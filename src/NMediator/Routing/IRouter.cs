using NMediator.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Routing
{
    interface IRouter
    {
        IOutgoingTransport Route(Type messageType);
    }
}
