using NMediator.Core.Configuration;
using NMediator.Event;
using System;

namespace NMediator.Configuration
{
    public static class EventConfigurationExtensions
    {
        public static MediatorConfiguration Event (this MediatorConfiguration config, Action<EventConfiguration> action)
        {
            var rc = new EventConfiguration(config.Container);
            action(rc);
            config.RegisterConfiguration(rc);
            return config;
        }
    }
}
