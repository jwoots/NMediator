using NMediator.Core.Message;
using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Core.Context
{
    public class DefaultHeadersMessageProcessorDecorator : IMessageProcessor
    {
        private readonly IMessageProcessor _decoratee;

        public DefaultHeadersMessageProcessorDecorator(IMessageProcessor decoratee)
        {
            _decoratee = decoratee;
        }

        public Task<RequestResult<TResult>> Process<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) where TMessage : IMessage<TResult>
        {
            IDictionary<string, string> h = headers ?? new Dictionary<string, string>();
            
            if (!h.ContainsKey(Headers.ID))
                h[Headers.ID] = Guid.NewGuid().ToString();
            if (!h.ContainsKey(Headers.DATE))
                h[Headers.DATE] = DateTimeOffset.Now.ToString("o");

            return _decoratee.Process<TMessage, TResult>(message, headers);
        }
    }
}
