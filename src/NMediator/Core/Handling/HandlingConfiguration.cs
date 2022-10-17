using NMediator.Core.Configuration;
using NMediator.Core.SInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Configure Handling : How NMediator find and active handlers
    /// </summary>
    public class HandlingConfiguration : BaseConfiguration
    {
        public Container Container { get; }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="container"></param>
        public HandlingConfiguration(Container container)
        {
            Container = container;
        }

        /// <summary>
        /// Delegate handler activation with your favorite DI tools or with manual activation
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public HandlingConfiguration UseDelegateActivator(Func<Type,IEnumerable<object>> func)
        {
            DelegateServiceActivator activator = new(func);
            Container.Register<IServiceActivator>(ctx => activator);
            return this;
        }

        /// <summary>
        /// Use assemblies scanning to provide handler to NMediator
        /// </summary>
        /// <param name="handlersAssemblies"></param>
        /// <returns></returns>
        public HandlingConfiguration ScanHandlersFromAssemblies(params Assembly[] handlersAssemblies)
        {
            AssembliesHandlerInterfaceTypeProvider tp = new(handlersAssemblies);
            Container.Register<IHandlerInterfaceTypeProvider>(ctx => tp);
            return this;
        }
        
    }
}
