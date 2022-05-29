using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestHandler<TRequest, TResult> : IMessageHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
    }
}
