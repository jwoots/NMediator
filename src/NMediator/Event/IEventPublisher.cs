﻿using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventPublisher 
    {
        Task<RequestResult<Nothing>> Publish<TEvent>(TEvent @event, CancellationToken token, IDictionary<string,string> headers= null);
    }

    public class EventPublisher : IEventPublisher
    {
        private readonly IMessageProcessor _processsor;

        public EventPublisher(IMessageProcessor processsor)
        {
            _processsor = processsor;
        }

        public Task<RequestResult<Nothing>> Publish<TEvent>(TEvent @event, CancellationToken token, IDictionary<string, string> headers  = null)
        {
            return _processsor.Process<TEvent, Nothing>(@event, token, headers);
        }
    }
}
