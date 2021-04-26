using NMediator.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestProcessor
    {
        Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, IDictionary<string, string> headers = null) where TRequest : IRequest<TResult>;
    }
}
