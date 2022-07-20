using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Threading.Tasks;

namespace NMediator.Event
{
    public interface IEventHandler<TEvent>  : IMessageHandler<TEvent, Nothing>
    {
    }
}
