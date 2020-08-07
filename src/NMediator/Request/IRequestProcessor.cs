using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestProcessor
    {
        Task<RequestResult<TResult>> Process<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>;
    }
}
