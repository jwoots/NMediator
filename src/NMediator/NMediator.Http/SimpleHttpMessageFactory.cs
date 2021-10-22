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
    public class SimpleHttpMessageFactory : IHttpMessageFactory
    {
        private IDictionary<Type, Func<object, HttpRequestMessage>> _requestMessagefactories = new Dictionary<Type, Func<object, HttpRequestMessage>>();
        private IDictionary<HttpStatusCode, Func<HttpResponseMessage, Error>> _errorfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Error>>();
        private IDictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>> _exceptionfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>>();


        public void AddRequestFactory<T>(Func<T, HttpRequestMessage> factory)
        {
            _requestMessagefactories[typeof(T)] = o => factory((T)o);
        }

        public void AddErrorFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Error> factory)
        {
            _errorfactories[httpStatusCode] = factory;
        }

        public void AddExceptionFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Exception> factory)
        {
            _exceptionfactories[httpStatusCode] = factory;
        }

        HttpRequestMessage IHttpMessageFactory.CreateRequest(object message)
        {
            if (!_requestMessagefactories.ContainsKey(message.GetType()))
                throw new InvalidOperationException($"there is no factory registered for type {message.GetType()}");

            return _requestMessagefactories[message.GetType()](message);
        }

        async Task<RequestResult<T>> IHttpMessageFactory.CreateResult<T>(HttpResponseMessage httpMessage)
        {
            if (_errorfactories.ContainsKey(httpMessage.StatusCode))
                return RequestResult.Fail<T>(_errorfactories[httpMessage.StatusCode](httpMessage));

            if (_exceptionfactories.ContainsKey(httpMessage.StatusCode))
                throw _exceptionfactories[httpMessage.StatusCode](httpMessage);

            int statusCode = (int)httpMessage.StatusCode;
            string contentString = await httpMessage.Content.ReadAsStringAsync();

            if (statusCode >= 200 && statusCode < 300)
                return RequestResult.Success(JsonSerializer.Deserialize<T>(contentString));

            if (statusCode >= 300 && statusCode < 400)
                throw new NotSupportedException("http response code 3xx are not supported");

            if (statusCode >= 400 && statusCode < 500)
                return RequestResult.Fail<T>(new Error()
                {
                    Code = httpMessage.StatusCode.ToString(),
                    Description = contentString
                });

            throw new Exception(contentString);
        }
    }
}
