using NMediator.Request.Configuration;
using System;

namespace NMediator.Configuration
{
    public static class ConfigurationExtensions
    {
        public static MediatorConfiguration Request(this MediatorConfiguration config, Action<RequestConfiguration> action)
        {
            var rc = new RequestConfiguration(config.Container);
            action(rc);
            config.RegisterConfiguration(rc);
            return config;
        }
    }
}
