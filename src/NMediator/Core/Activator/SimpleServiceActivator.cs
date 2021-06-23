using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Activator
{
    public class SimpleServiceActivator : IServiceActivator
    {
        public IDictionary<Type, ICollection<object>> Instances { get; } = new Dictionary<Type, ICollection<object>>();

        public IEnumerable<T> GetInstances<T>()
        {
            yield return (T)GetInstances(typeof(T));
        }

        public IEnumerable<object> GetInstances(Type type)
        {
            if (Instances.TryGetValue(type, out var result))
            {
                return result;
            }
        
            throw new InvalidOperationException($"No instance found for type {type}");
        }

        public void RegisterMessage<TMessage, TResult>(Func<TMessage, Task<RequestResult<TResult>>> func) where TMessage : IMessage<TResult>
        {
            if (Instances.TryGetValue(typeof(IMessageHandler<TMessage, TResult>), out var handlers))
                handlers.Add(new GenericRequestHandler<TMessage, TResult>(func));
            else
                Instances[typeof(IMessageHandler<TMessage,TResult>)] = new List<object>() { new GenericRequestHandler<TMessage, TResult>(func) };
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
