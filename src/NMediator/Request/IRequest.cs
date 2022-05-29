using NMediator.Core.Message;

namespace NMediator.Request
{
    public interface IRequest<TResult> : IMessage<TResult>
    {
    }
}
