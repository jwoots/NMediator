using NMediator.Event;
using NMediator.Request;
using System;
using System.Collections.Generic;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Activator to get handlers instance
    /// </summary>
    public interface IServiceActivator
    {
        /// <summary>
        /// get all handlers instances for handler interface type (IMessageHandler<T,R>)
        /// </summary>
        /// <param name="handlerInterfaceType"></param>
        /// <returns></returns>
        IEnumerable<object> GetInstances(Type handlerInterfaceType);
    }
}
