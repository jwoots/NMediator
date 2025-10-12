using NMediator.Core.Result;
using NMediator.Http;
using NMediator.Http.BodyConverter;
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
        protected IBodyConverter BodyConverter { get; }

        public HttpResponseToErrorMapper ErrorFactory { get;  } = new HttpResponseToErrorMapper();

        public SimpleHttpMessageFactory(IBodyConverter bodyConverter)
        {
            BodyConverter = bodyConverter;
        }

        /// <summary>
        /// Add a factory for create an HttpRequestMessage from TMessage object
        /// </summary>
        /// <typeparam name="TMessage">type of message</typeparam>
        /// <param name="factory">the factory to create HttpRequestMessage from TMessage object</param>
        public void AddRequestFactory<TMessage>(Func<TMessage, HttpRequestMessage> factory)
        {
            _requestMessagefactories[typeof(TMessage)] = o => factory((TMessage)o);
        }

        HttpRequestMessage IHttpMessageFactory.CreateRequest(object message)
        {
            var result = CreateRequest(message);
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error!.Description);

            return result.Data!;
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
            if (ErrorFactory.TryGetError(httpMessage, out Error error))
                return RequestResult.Fail<TMessage>(error);

            if (ErrorFactory.TryGetException(httpMessage, out Exception exception))
                throw exception;

            int statusCode = (int)httpMessage.StatusCode;
            string contentString = await httpMessage.Content.ReadAsStringAsync();

            if (statusCode >= 200 && statusCode < 300)
            {
                if (string.IsNullOrWhiteSpace(contentString) && typeof(TMessage) == typeof(Nothing))
                {
                    return RequestResult.Success((TMessage)(object)new Nothing());
                }

                return RequestResult.Success(BodyConverter.ConvertToType<TMessage>(contentString)!);
            }

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
