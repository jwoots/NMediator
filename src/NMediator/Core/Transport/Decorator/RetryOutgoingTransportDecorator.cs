﻿using NMediator.Core.Message;
using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Threading;
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

            if(_retryTimes <= 0)
                throw new ArgumentOutOfRangeException(nameof(retryTimes));
        }
        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, CancellationToken token, IDictionary<string,string> headers)
        {
            Exception e = null;
            for(int i=0;i<_retryTimes;i++)
            {
                try
                {
                    return await _decoratee.SendMessage<TMessage, TResult>(message, token, headers);
                }
                catch(Exception ex)
                {
                    e = ex;
                }
            }

#pragma warning disable S2259 // Null pointers should not be dereferenced
            throw e;
#pragma warning restore S2259 // Null pointers should not be dereferenced

        }
    }
}
