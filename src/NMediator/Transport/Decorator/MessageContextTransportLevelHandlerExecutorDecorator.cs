using NMediator.Context;
using NMediator.Extensions;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport.Decorator
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
