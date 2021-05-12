using NMediator.Core.Result;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventPublisher
    {
        Task<RequestResult<Nothing>> Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
