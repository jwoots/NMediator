using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http
{
    public interface IHttpMessageFactory
    {
        HttpRequestMessage CreateRequest(object message);
        Task<RequestResult<T>> CreateResult<T>(HttpResponseMessage httpMessage);
    }
}
