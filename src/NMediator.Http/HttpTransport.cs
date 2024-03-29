﻿using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Core.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http
{
    /// <summary>
    /// Implement Http as outgoing transport.
    /// 3 Steps : 
    /// - create an http request from message
    /// - send http request
    /// - convert response to TResult
    /// </summary>
    public class HttpTransport : IOutgoingTransport
    {
        private readonly IHttpMessageFactory _factory;
        private readonly Func<HttpClient> _clientFactory;

        /// <summary>
        /// Create a new HttpTransport
        /// </summary>
        /// <param name="factory">the http message factory</param>
        /// <param name="clientFactory">the http client facotry</param>
        public HttpTransport(IHttpMessageFactory factory, Func<HttpClient> clientFactory)
        {
            _factory = factory;
            _clientFactory = clientFactory;
        }

       
        /// <inheritdoc/>
        public async Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, CancellationToken token, IDictionary<string, string> headers)
        {
            var httpRequest = _factory.CreateRequest(message);
            var httpClient = _clientFactory();
            var result = await httpClient.SendAsync(httpRequest, token);
            return await _factory.CreateResult<TResult>(result);
        }
    }
}
