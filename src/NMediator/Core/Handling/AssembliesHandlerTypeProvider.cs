using NMediator.Core.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NMediator.Core.Handling;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Handler type provider implementation from assemblies
    /// Search in assemblies all classe implmenting IMessageHandler<,>
    /// </summary>
    public class AssembliesHandlerInterfaceTypeProvider : IHandlerInterfaceTypeProvider
    {
        private readonly IDictionary<Type, Type> _handlerTypes = new Dictionary<Type, Type>();

        /// <summary>
        /// Scan assemblies and search IMessageHandler<,> implementation
        /// </summary>
        /// <param name="assemblies"></param>
        public AssembliesHandlerInterfaceTypeProvider(IEnumerable<Assembly> assemblies)
        {
            foreach(var assembly in assemblies)
            {
                foreach(var type in assembly.GetTypes())
                {
                    if(type.IsOpenGenericInterface(typeof(IMessageHandler<,>), out var interfaceType))
                    {
                        _handlerTypes[interfaceType.GetGenericArguments()[0]] = interfaceType;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public Type GetHandlerInterfaceTypeByMessageType(Type messageType)
        {
            return _handlerTypes[messageType];
        }
    }
}
