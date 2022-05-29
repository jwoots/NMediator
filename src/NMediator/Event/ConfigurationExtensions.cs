using NMediator.Event;
using System;

namespace NMediator.Core.Configuration
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
