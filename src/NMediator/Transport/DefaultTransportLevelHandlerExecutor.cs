using NMediator.Activator;
using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport
{
    public class DefaultTransportLevelHandlerExecutor : ITransportLevelHandlerExecutor
    {
        private readonly IHandlerActivator _handlerActivator;

        public DefaultTransportLevelHandlerExecutor(IHandlerActivator handlerActivator)
        {
            _handlerActivator = handlerActivator;
        }

        public async Task<IRequestResult> ExecuteHandler(object message)
        {
            var requestType = message.GetType();
            var responseType = requestType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            dynamic param = message;
            object handlerResult = _handlerActivator.GetInstance(handlerType);
            dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handlerResult, new object[] {  message });
            await awaitable;
            return (IRequestResult) awaitable.GetAwaiter().GetResult();
        }
    }
}
