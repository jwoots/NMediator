using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Routing
{
    internal class RoutedRequestProcessor : IRequestProcessor
    {
        private readonly IRouter _router;

        public RoutedRequestProcessor(IRouter router)
        {
            _router = router;
        }

        public async Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request, IDictionary<string, string> headers = null) where TRequest : IRequest<TResult>
        {
            var result = await _router.Route(typeof(TRequest)).SendMessage(request, headers);
            return (RequestResult<TResult>) result;
        }
    }
}
