using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        Task<RequestResult<TResult>> Handle(TRequest request);
    }
}
