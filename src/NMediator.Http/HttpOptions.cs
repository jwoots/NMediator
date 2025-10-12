using NMediator.NMediator.Http.Reflection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NMediator.NMediator.Http
{
    public class HttpOptions
    {
        public Uri BaseUri { get; set; } = null!;
        public  HttpDescriptors HttpDescriptors { get; set; } = null!;
        public  Func<HttpClient> HttpClientFactory { get; set; } = null!;
    }
}
