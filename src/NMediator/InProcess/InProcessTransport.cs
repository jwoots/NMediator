using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Core.Transport;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.InProcess
{
    public class InProcessTransport : IOutgoingTransport
    {
        private readonly ITransportLevelHandlerExecutor _handlerExecutor;

        public InProcessTransport(ITransportLevelHandlerExecutor handlerExecutor)
        {
            _handlerExecutor = handlerExecutor ?? throw new ArgumentNullException(nameof(handlerExecutor));
        }

        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) where TMessage : IMessage<TResult>
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return await _handlerExecutor.ExecuteHandler(message, headers);
        }
    }

    public class InProcessEventTransport : IOutgoingTransport
    {
        private readonly ITransportLevelHandlerExecutor _handlerExecutor;

        public InProcessEventTransport(ITransportLevelHandlerExecutor handlerExecutor)
        {
            _handlerExecutor = handlerExecutor ?? throw new ArgumentNullException(nameof(handlerExecutor));
        }

        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) 
            where TMessage : IMessage<TResult>
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await _handlerExecutor.ExecuteHandler(message, headers);

            return RequestResult.Success();
        }
    }
}
