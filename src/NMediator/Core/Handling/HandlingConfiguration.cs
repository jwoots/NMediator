using NMediator.Core.Configuration;
using NMediator.Core.SInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NMediator.Core.Handling
{
    public class HandlingConfiguration : BaseConfiguration
    {
        public Container Container { get; }

        public HandlingConfiguration(Container container)
        {
            Container = container;
        }

        public HandlingConfiguration UseDelegateActivator(Func<Type,IEnumerable<object>> func)
        {
            DelegateServiceActivator activator = new(func);
            Container.Register<IServiceActivator>(ctx => activator);
            return this;
        }

        public HandlingConfiguration ScanHandlersFromAssemblies(params Assembly[] handlersAssemblies)
        {
            AssembliesHandlerInterfaceTypeProvider tp = new(handlersAssemblies);
            Container.Register<IHandlerInterfaceTypeProvider>(ctx => tp);
            return this;
        }
        
    }
}
