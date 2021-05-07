using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task<RequestResult<Nothing>> Handle(TEvent @event);
    }
}
