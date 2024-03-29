﻿using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NMediator.Core.Handling;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Built in service activator
    /// </summary>
    public class SimpleServiceActivator : IServiceActivator, IHandlerInterfaceTypeProvider
    {
        private IDictionary<Type, ICollection<object>> _instances { get; } = new Dictionary<Type, ICollection<object>>();

        public Type GetHandlerInterfaceTypeByMessageType(Type messageType)
        {
            return _instances.Keys.FirstOrDefault(i => i.GetGenericArguments()[0] == messageType);
        }

        public IEnumerable<object> GetInstances(Type handlerInterfaceType)
        {
            if (_instances.TryGetValue(handlerInterfaceType, out var result))
            {
                return result;
            }
        
            return Enumerable.Empty<object>();
        }

        public void RegisterMessage<TMessage, TResult>(Func<TMessage, CancellationToken, Task<RequestResult<TResult>>> func)
        {
            if (_instances.TryGetValue(typeof(IMessageHandler<TMessage, TResult>), out var handlers))
                handlers.Add(new GenericRequestHandler<TMessage, TResult>(func));
            else
                _instances[typeof(IMessageHandler<TMessage,TResult>)] = new List<object>() { new GenericRequestHandler<TMessage, TResult>(func) };
        }

        class GenericRequestHandler<TMessage, TResult> : IMessageHandler<TMessage, TResult>
        {
            private readonly Func<TMessage, CancellationToken, Task<RequestResult<TResult>>> _func;

            public GenericRequestHandler(Func<TMessage, CancellationToken, Task<RequestResult<TResult>>> func)
            {
                _func = func;
            }

            public Task<RequestResult<TResult>> Handle(TMessage message, CancellationToken cancellationToken)
            {
                return _func(message, cancellationToken);
            }
        }
    }
}
