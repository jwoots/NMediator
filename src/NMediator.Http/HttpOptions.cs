using NMediator.NMediator.Http.Reflection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NMediator.NMediator.Http
{
    public class HttpOptions
    {
        public Uri BaseUri { get; set; }
        public HttpDescriptors HttpDescriptors { get; set; }
        public Func<HttpClient> HttpClientFactory { get; set; }
    }
}
