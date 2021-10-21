using NMediator.Core.SInjector;
using NMediator.Core.Transport;
using NMediator.Core.Transport.Decorator;
using System;

namespace NMediator.NMediator.Http
{
    public class HttpConfiguration : TransportConfiguration
    {
        public Container Container { get; }

        public HttpConfiguration(Container container)
        {
            Container = container;
        }

        public TransportOptionsConfiguration ExecuteWithHttp(Action<HttpOptions> optionsFactory, params Type[] types)
        {
            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            var options = new HttpOptions();
            optionsFactory(options);
            var ht = new HttpTransport(options.HttpMessageFactory, options.HttpClientFactory);
            var toc = new TransportOptionsConfiguration(Container, () => ht, types);
            TransportConfigurations.Add(toc);
            return toc;
        }     
    }
}
