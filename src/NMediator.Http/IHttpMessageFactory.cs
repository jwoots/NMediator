using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http
{
    /// <summary>
    /// interface for create HttpRequestMessage from object
    /// and create typed object from HttpResponseMessage
    /// </summary>
    public interface IHttpMessageFactory
    {
        /// <summary>
        /// Create a HttpRequestMessage from message object.
        /// All public properties are content of message
        /// </summary>
        /// <param name="message">the message</param>
        HttpRequestMessage CreateRequest(object message);

        /// <summary>
        /// create a T Message from http response message
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="httpMessage">the http response message</param>
        /// <returns></returns>
        Task<RequestResult<TMessage>> CreateResult<TMessage>(HttpResponseMessage httpMessage);
    }
}
