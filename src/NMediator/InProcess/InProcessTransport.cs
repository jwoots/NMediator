using NMediator.Result;
using NMediator.Transport;
using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<IRequestResult> SendMessage(object message, IDictionary<string, string> headers)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _handlerExecutor.ExecuteHandler(message, headers);
        }
    }
}
