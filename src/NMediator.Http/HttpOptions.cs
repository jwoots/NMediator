using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NMediator.NMediator.Http
{
    public class HttpOptions
    {
        public IHttpMessageFactory HttpMessageFactory { get; set; }
        public Func<HttpClient> HttpClientFactory { get; set; }
    }
}
