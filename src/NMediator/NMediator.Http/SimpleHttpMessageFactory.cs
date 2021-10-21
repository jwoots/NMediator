using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace NMediator.NMediator.Http
{
    public class SimpleHttpMessageFactory : IHttpMessageFactory
    {
        private IDictionary<Type, Func<object, HttpRequestMessage>> _factories = new Dictionary<Type, Func<object, HttpRequestMessage>>();

        public void AddFactory<T>(Func<T, HttpRequestMessage> factory)
        {
            _factories[typeof(T)] = o => factory((T)o);
        }
        
        HttpRequestMessage IHttpMessageFactory.CreateRequest(object message)
        {
            return _factories[message.GetType()](message);
        }

        RequestResult<T> IHttpMessageFactory.CreateResult<T>(HttpResponseMessage httpMessage)
        {
            if ((int)httpMessage.StatusCode >= 200 && (int)httpMessage.StatusCode < 300)
                return RequestResult.Success(JsonSerializer.Deserialize<T>(httpMessage.Content.ReadAsStringAsync().Result));

            if((int)httpMessage.StatusCode < 500)
                return RequestResult.Fail<T>(new Error()
                {
                    Code = httpMessage.StatusCode.ToString(),
                    Description = httpMessage.Content.ReadAsStringAsync().Result
                });

            throw new Exception(httpMessage.Content.ReadAsStringAsync().Result);
        }
    }
}
