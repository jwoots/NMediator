using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Extensions;
using NMediator.InProcess;
using NMediator.Routing;
using NMediator.SInjector;
using NMediator.Transport;
using NMediator.Transport.Decorator;
using System;
using System.Collections.Generic;

namespace NMediator.Request.Configuration
{
    public class RequestConfiguration : BaseConfiguration
    {
        public Container Container { get; }
        
        public RequestConfiguration(Container container)
        {
            Container = container;
        }
    }
}
