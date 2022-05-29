using NMediator.Core.Routing;
using NMediator.Core.Transport;
using NMediator.Core.Transport.Decorator;
using NMediator.NMediator.Http;
using NMediator.NMediator.Http.Reflection;
using NMediator.Request.Configuration;
using System;

namespace NMediator.Core.Configuration
{
    public static class RequestConfigurationHttpExtensions
    {
        public static RequestConfiguration ExecuteWithHttp(this RequestConfiguration config, Action<HttpOptions> optionsFactory)
        {
            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            var options = new HttpOptions();
            optionsFactory(options);

            var ht = new HttpTransport(new ReflectionHttpMessageFactory(options.BaseUri, options.HttpDescriptors), options.HttpClientFactory);
            var router = config.Container.Get<TypeBasedRouter>();
            foreach (var type in options.HttpDescriptors.GetRegisteredTypes())
                router.AddRoute(type, ht);

            return config;
        }
    }
}
