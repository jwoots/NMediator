using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http
{
    /// <summary>
    /// Simple HttpMessageFactory based on factory provided by the caller
    /// 
    /// </summary>
    public class SimpleHttpMessageFactory : IHttpMessageFactory
    {
        private readonly IDictionary<Type, Func<object, HttpRequestMessage>> _requestMessagefactories = new Dictionary<Type, Func<object, HttpRequestMessage>>();
        private readonly IDictionary<HttpStatusCode, Func<HttpResponseMessage, Error>> _errorfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Error>>();
        private readonly IDictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>> _exceptionfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>>();

        /// <summary>
        /// Add a factory for create an HttpRequestMessage from TMessage object
        /// </summary>
        /// <typeparam name="TMessage">type of message</typeparam>
        /// <param name="factory">the factory to create HttpRequestMessage from TMessage object</param>
        public void AddRequestFactory<TMessage>(Func<TMessage, HttpRequestMessage> factory)
        {
            _requestMessagefactories[typeof(TMessage)] = o => factory((TMessage)o);
        }

        /// <summary>
        /// Add a factory to provide a request result Error from HttpResponseMessage for a http status code
        /// </summary>
        /// <param name="httpStatusCode">the status code for which apply the factory</param>
        /// <param name="factory">the factory de create RequestResult Error from Http response message</param>
        public void AddErrorFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Error> factory)
        {
            _errorfactories[httpStatusCode] = factory;
        }

        /// <summary>
        /// Add a factory to create an exception from HttpResponseMessage for a http status code
        /// </summary>
        /// <param name="httpStatusCode">the status code for which apply the factory</param>
        /// <param name="factory">the factory to create Exception from http response message</param>
        public void AddExceptionFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Exception> factory)
        {
            _exceptionfactories[httpStatusCode] = factory;
        }

        HttpRequestMessage IHttpMessageFactory.CreateRequest(object message)
        {
            var result = CreateRequest(message);
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error.Description);

            return result.Data;
        }

        /// <summary>
        /// Create a HttpRequestMessage from message
        /// </summary>
        /// <param name="message">the message from which create HttpRquestMessage</param>
        protected virtual RequestResult<HttpRequestMessage> CreateRequest(object message)
        {
            if (!_requestMessagefactories.ContainsKey(message.GetType()))
                return RequestResult.Fail<HttpRequestMessage>(new Error()
                {
                    Code = "NO_FACTORY_REGISTERED",
                    Description = $"there is no factory registered for type {message.GetType()}"
                });

            return RequestResult.Success(_requestMessagefactories[message.GetType()](message));
        }

        Task<RequestResult<T>> IHttpMessageFactory.CreateResult<T>(HttpResponseMessage httpMessage)
        {
            return CreateResult<T>(httpMessage);
        }

        /// <summary>
        /// create a result from http response message
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="httpMessage">the http response message</param>
        protected virtual async Task<RequestResult<TMessage>> CreateResult<TMessage>(HttpResponseMessage httpMessage)
        {
            if (_errorfactories.ContainsKey(httpMessage.StatusCode))
                return RequestResult.Fail<TMessage>(_errorfactories[httpMessage.StatusCode](httpMessage));

            if (_exceptionfactories.ContainsKey(httpMessage.StatusCode))
                throw _exceptionfactories[httpMessage.StatusCode](httpMessage);

            int statusCode = (int)httpMessage.StatusCode;
            string contentString = await httpMessage.Content.ReadAsStringAsync();

            if (statusCode >= 200 && statusCode < 300)
                return RequestResult.Success(string.IsNullOrWhiteSpace(contentString) ? default : JsonSerializer.Deserialize<TMessage>(contentString));

            if (statusCode >= 300 && statusCode < 400)
                throw new NotSupportedException("http response code 3xx are not supported");

            if (statusCode >= 400 && statusCode < 500)
                return RequestResult.Fail<TMessage>(new Error()
                {
                    Code = httpMessage.StatusCode.ToString(),
                    Description = contentString
                });

#pragma warning disable S112 // General exceptions should never be thrown
            throw new Exception(contentString);
#pragma warning restore S112 // General exceptions should never be thrown
        }
    }
}
