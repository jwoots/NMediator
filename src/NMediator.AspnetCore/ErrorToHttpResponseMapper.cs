using NMediator.Core.Result;
using System.Text.Json;

namespace NMediator.AspnetCore
{
    public class ErrorToHttpResponseMapper
    {
        private readonly IDictionary<string, Func<Error, NMediatorHttpResponse>> _httpResponseFromErrorFactories = new Dictionary<string, Func<Error, NMediatorHttpResponse>>();
        private readonly IDictionary<Type, Func<Exception, NMediatorHttpResponse>> _httpResponseFromExceptionFactories = new Dictionary<Type, Func<Exception, NMediatorHttpResponse>>();

        public Func<Error, NMediatorHttpResponse> DefaultHttpResponseFromErrorFactory { get; set; } = e => new NMediatorHttpResponse() { Body = JsonSerializer.Serialize(e), StatusCode = 400 };
        public Func<Exception, NMediatorHttpResponse> DefaultHttpResponseFromExceptionFactory { get; set; } = e => new NMediatorHttpResponse { Body = e.Message, StatusCode = 500 };

        public void AddHttpResponseFactory(string code, Func<Error, NMediatorHttpResponse> factory)
        {
            _httpResponseFromErrorFactories[code] = factory;
        }

        public NMediatorHttpResponse GetResponseFromErrorOrDefault(Error error) 
        {
            if (_httpResponseFromErrorFactories.TryGetValue(error.Code, out var response))
                return response(error);

            return DefaultHttpResponseFromErrorFactory(error);
        }

        public void AddHttpResponseFactory<TException>(Func<TException, NMediatorHttpResponse> factory) where TException : Exception
        {
            _httpResponseFromExceptionFactories[typeof(TException)] = e => factory((TException)e);
        }

        public NMediatorHttpResponse GetResponseFromExceptionOrDefault<TException>(TException exception) where TException : Exception
        {
            if (_httpResponseFromExceptionFactories.TryGetValue(typeof(TException), out var response))
                return response(exception);

            return DefaultHttpResponseFromExceptionFactory(exception);
        }
    }
}
