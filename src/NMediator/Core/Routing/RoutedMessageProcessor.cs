using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Routing
{
    internal class RoutedMessageProcessor: IMessageProcessor
    {
        private readonly IRouter _router;

        public RoutedMessageProcessor(IRouter router)
        {
            _router = router;
        }

        public async Task<RequestResult<TResult>> Process<TMessage, TResult>(TMessage message, CancellationToken token, IDictionary<string, string> headers = null) where TMessage : IMessage<TResult>
        {
            var result = await _router.Route(typeof(TMessage)).SendMessage<TMessage, TResult>(message, token, headers);
            return (RequestResult<TResult>)result;
        }
    }
}
