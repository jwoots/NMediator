using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestHandler
    {
        Task<RequestResult<TResult>> Handle<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>;
    }
}
