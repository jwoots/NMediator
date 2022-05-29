using NMediator.Core.Activator;
using NMediator.Core.Result;
using NMediator.Request;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public class DefaultTransportLevelHandlerExecutor : ITransportLevelHandlerExecutor
    {
        private readonly IServiceActivator _handlerActivator;

        public DefaultTransportLevelHandlerExecutor(IServiceActivator handlerActivator)
        {
            _handlerActivator = handlerActivator;
        }

        public Task<IRequestResult> ExecuteHandler(object message, CancellationToken token, IDictionary<string, string> headers)
        {
            return _handlerActivator.ProcessMessageHandler(message, token);
        }
    }
}
