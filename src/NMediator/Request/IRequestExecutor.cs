using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Request
{
    public interface IRequestExecutor

    {
        Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, CancellationToken token, IDictionary<string, string> headers = null);
    }

    public class RequestExecutor : IRequestExecutor
    {
        private readonly IMessageProcessor _processor;

        public RequestExecutor(IMessageProcessor processor)
        {
            _processor = processor;
        }

        public Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, CancellationToken token, IDictionary<string, string> headers = null)
        {
            return _processor.Process<TRequest, TResult>(request, token, headers);
        }
    }
}
