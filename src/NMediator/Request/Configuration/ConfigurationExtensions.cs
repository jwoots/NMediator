using NMediator.Request.Configuration;
using System;

namespace NMediator.Core.Configuration
{
    public static class ConfigurationExtensions
    {
        public static MediatorConfiguration Request(this MediatorConfiguration config, Action<RequestConfiguration> action)
        {
            var rc = new RequestConfiguration(config.Container);
            config.RegisterConfiguration(rc);
            action(rc);
            return config;
        }
    }
}
