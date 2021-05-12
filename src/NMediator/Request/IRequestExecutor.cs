using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestExecutor

    {
        Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, IDictionary<string, string> headers = null) where TRequest : IRequest<TResult>;
    }

    public class RequestExecutor : IRequestExecutor
    {
        private readonly IMessageProcessor _processor;

        public RequestExecutor(IMessageProcessor processor)
        {
            _processor = processor;
        }

        public Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, IDictionary<string, string> headers = null) where TRequest : IRequest<TResult>
        {
            return _processor.Process<TRequest, TResult>(request, headers);
        }
    }
}
