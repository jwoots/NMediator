using NMediator.Core.Result;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task<RequestResult<Nothing>> Handle(TEvent @event);
    }
}
