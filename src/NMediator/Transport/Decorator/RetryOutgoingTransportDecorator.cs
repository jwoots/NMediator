using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport.Decorator
{
    class RetryOutgoingTransportDecorator : IOutgoingTransport
    {
        private readonly int _retryTimes;
        private IOutgoingTransport _decoratee;

        public RetryOutgoingTransportDecorator(IOutgoingTransport decoratee, int retryTimes)
        {
            _decoratee = decoratee;
            _retryTimes = retryTimes;
        }
        public async Task<IRequestResult> SendMessage(object message, IDictionary<string,string> headers)
        {
            Exception e = null;
            for(int i=0;i<_retryTimes;i++)
            {
                try
                {
                    return await _decoratee.SendMessage(message, headers);
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
