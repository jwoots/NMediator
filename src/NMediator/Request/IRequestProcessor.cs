using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestProcessor
    {
        Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>;
    }
}
