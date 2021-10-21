using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NMediator.NMediator.Http
{
    public interface IHttpMessageFactory
    {
        HttpRequestMessage CreateRequest(object message);
        RequestResult<T> CreateResult<T>(HttpResponseMessage httpMessage);
    }
}
