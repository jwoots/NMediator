using NMediator.Event;
using NMediator.Request;
using System;
using System.Collections.Generic;

namespace NMediator.Core.Handling
{
    public interface IServiceActivator
    {
        IEnumerable<object> GetInstances(Type handlerType);
    }
}
