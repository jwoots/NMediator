using NMediator.Core.Transport;
using NMediator.Core.Transport.Decorator;
using NMediator.NMediator.Http;
using NMediator.Request.Configuration;
using System;

namespace NMediator.Core.Configuration
{
    public static class RequestConfigurationHttpExtensions
    {
        public static TransportOptionsConfiguration ExecuteWithHttp(this RequestConfiguration config, Action<HttpOptions> optionsFactory, params Type[] types)
        {
            var tc = new TransportConfiguration();
            config.RegisterConfiguration(tc);

            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            var options = new HttpOptions();
            optionsFactory(options);

            var ht = new HttpTransport(options.HttpMessageFactory, options.HttpClientFactory);
            var toc = new TransportOptionsConfiguration(config.Container, () => ht, types);
            tc.TransportConfigurations.Add(toc);

            return toc;
        }
    }
}
