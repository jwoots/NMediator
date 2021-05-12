using NMediator.Event;
using NMediator.Request;
using System;
using System.Collections.Generic;

namespace NMediator.Core.Activator
{
    public interface IHandlerActivator
    {
        IEnumerable<THandler> GetInstances<THandler>();
        IEnumerable<object> GetInstances(Type handlerType);
    }
}
