using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Core.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http
{
    public class HttpTransport : IOutgoingTransport
    {
        private readonly IHttpMessageFactory _factory;
        private readonly Func<HttpClient> _clientFactory;

        public HttpTransport(IHttpMessageFactory factory, Func<HttpClient> clientFactory)
        {
            _factory = factory;
            _clientFactory = clientFactory;
        }

        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) where TMessage : IMessage<TResult>
        {
            var httpRequest = _factory.CreateRequest(message);
            var result = await _clientFactory().SendAsync(httpRequest);
            return await _factory.CreateResult<TResult>(result);
        }
    }
}
