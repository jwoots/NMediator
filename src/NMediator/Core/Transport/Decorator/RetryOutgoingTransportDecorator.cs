using NMediator.Core.Message;
using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Transport.Decorator
{
    class RetryOutgoingTransportDecorator : IOutgoingTransport
    {
        private readonly int _retryTimes;
        private readonly IOutgoingTransport _decoratee;

        public RetryOutgoingTransportDecorator(IOutgoingTransport decoratee, int retryTimes)
        {
            _decoratee = decoratee;
            _retryTimes = retryTimes;
        }
        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string,string> headers) where TMessage : IMessage<TResult>
        {
            Exception e = null;
            for(int i=0;i<_retryTimes;i++)
            {
                try
                {
                    return await _decoratee.SendMessage<TMessage, TResult>(message, headers);
                }
                catch(Exception ex)
                {
                    e = ex;
                }
            }

            throw e;
        }
    }
}
