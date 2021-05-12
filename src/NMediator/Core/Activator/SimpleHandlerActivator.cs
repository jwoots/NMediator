using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Activator
{
    public class SimpleHandlerActivator : IHandlerActivator
    {
        private readonly IDictionary<Type, object> _handlers = new Dictionary<Type, object>();

        public IEnumerable<T> GetInstances<T>()
        {
            yield return (T)GetInstances(typeof(T));
        }

        public IEnumerable<object> GetInstances(Type type)
        {
            if (_handlers.TryGetValue(type, out var result))
            {
                return new object[] { result };
            }
        
            throw new InvalidOperationException($"No handler instance found for type {type}");
        }

        public void RegisterRequest<TMessage, TResult>(Func<TMessage, Task<RequestResult<TResult>>> func) where TMessage : IMessage<TResult>
        {
            _handlers[typeof(IMessageHandler<TMessage,TResult>)] = new GenericRequestHandler<TMessage, TResult>(func);
        }

        class GenericRequestHandler<TMessage, TResult> : IMessageHandler<TMessage, TResult> where TMessage : IMessage<TResult>
        {
            private readonly Func<TMessage, Task<RequestResult<TResult>>> _func;

            public GenericRequestHandler(Func<TMessage, Task<RequestResult<TResult>>> func)
            {
                _func = func;
            }

            public Task<RequestResult<TResult>> Handle(TMessage request)
            {
                return _func(request);
            }
        }
    }
}
