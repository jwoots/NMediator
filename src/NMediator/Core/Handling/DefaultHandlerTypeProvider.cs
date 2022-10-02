using NMediator.Core.Message;
using System;
using System.Collections.Generic;
using System.Text;
using NMediator.Core.Handling;

namespace NMediator.Core.Handling
{
    public class DefaultHandlerInterfaceTypeProvider : IHandlerInterfaceTypeProvider
    {
        private readonly IDictionary<Type, Type> _handlerTypes = new Dictionary<Type, Type>();

        public void RegisterHandlerInterfaceType(Type handlerType)
        {
            if (!handlerType.IsOpenGenericInterface(typeof(IMessageHandler<,>), out var interfaceType))
                throw new ArgumentException("handlerType must be a IMessageHandler");

            var messageType = interfaceType.GenericTypeArguments[0];
            _handlerTypes[messageType] = interfaceType;
        }

        public void RegisterHandlerInterfaceType<T>() => RegisterHandlerInterfaceType(typeof(T));

        public Type GetHandlerInterfaceTypeByMessageType(Type messageType)
        {
            return _handlerTypes[messageType];
        }
    }
}
