using NMediator.Configuration;
using NMediator.SInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Event
{
    public class EventConfiguration : BaseConfiguration
    {
        public Container Container { get; }

        public EventConfiguration(Container container)
        {
            Container = container;
        }
    }
}
