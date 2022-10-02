using NMediator.Core.Message;
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
    public class SimpleServiceActivator : IServiceActivator, IHandlerInterfaceTypeProvider
    {
        private IDictionary<Type, ICollection<object>> _instances { get; } = new Dictionary<Type, ICollection<object>>();

        public Type GetHandlerInterfaceTypeByMessageType(Type messageType)
        {
            return _instances.Keys.FirstOrDefault(i => i.GetGenericArguments()[0] == messageType);
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return GetInstances(typeof(T)).Cast<T>();
        }

        public IEnumerable<object> GetInstances(Type handlerType)
        {
            if (_instances.TryGetValue(handlerType, out var result))
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

            public Task<RequestResult<TResult>> Handle(TMessage message, CancellationToken token)
            {
                return _func(message, token);
            }
        }
    }
}
