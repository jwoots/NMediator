using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NMediator.Core.Handling;

namespace NMediator.Core.Transport
{
    public class DefaultTransportLevelHandlerExecutor : ITransportLevelHandlerExecutor
    {
        private readonly IServiceActivator _handlerActivator;
        private readonly IHandlerInterfaceTypeProvider _handlerProvider;

        public DefaultTransportLevelHandlerExecutor(IServiceActivator handlerActivator, IHandlerInterfaceTypeProvider handlerProvider)
        {
            _handlerActivator = handlerActivator;
            _handlerProvider = handlerProvider;
        }

        public async Task<IRequestResult> ExecuteHandler(object message, CancellationToken token, IDictionary<string, string> headers)
        { 
            var handlerType = _handlerProvider.GetHandlerInterfaceTypeByMessageType(message.GetType());
            if (handlerType == null)
                throw new InvalidOperationException($"No handler found for message {message.GetType()}");

            var responseType = handlerType.GetGenericArguments()[1];

            IEnumerable<dynamic> handlers = _handlerActivator.GetInstances(handlerType);
            if (handlers.Count() > 1 && responseType != typeof(Nothing))
                throw new InvalidOperationException("more than 1 handler found for message with result");

            try
            {
                if (handlers.Count() == 1)
                {
                    var task = (Task)handlerType.GetMethod("Handle").Invoke(handlers.First(), new object[] { message, token });
                    await task;
                    var resultProperty = task.GetType().GetProperty("Result");
                    return (IRequestResult)resultProperty.GetValue(task);
                }
                else
                {
                    foreach (var handler in handlers)
                    {
                        var task = (Task)handlerType.GetMethod("Handle").Invoke(handler, new object[] { message, token });
                        await task;
                    }

                    return null;
                }
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
