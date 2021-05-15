using NMediator.Core.Context;
using NMediator.Core.Result;
using NMediator.Core.Transport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Context
{
    public class MessageContextTransportLevelHandlerExecutorDecorator : ITransportLevelHandlerExecutor
    {
        private readonly ITransportLevelHandlerExecutor _decoratee;
        public MessageContextTransportLevelHandlerExecutorDecorator(ITransportLevelHandlerExecutor decoratee)
        {
            _decoratee = decoratee;
        }

        public Task<IRequestResult> ExecuteHandler(object message, IDictionary<string, string> headers)
        {
            var mc = new MessageContext();
            headers?.ForEach(kvp => mc.Values[kvp.Key] = kvp.Value);
            MessageContext.SetContext(mc);
            return _decoratee.ExecuteHandler(message, headers);
        }
    }
}
