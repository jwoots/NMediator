using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventPublisher
    {
        Task<RequestResult<Nothing>> Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
