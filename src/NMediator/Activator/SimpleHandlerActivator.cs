using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Activator
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
                yield return result;
            }

            throw new InvalidOperationException($"there is no handler instance for type {type}");
        }

        public void RegisterRequest<TRequest, TResult>(Func<TRequest, Task<RequestResult<TResult>>> func) where TRequest : IRequest<TResult>
        {
            _handlers[typeof(IRequestHandler<TRequest,TResult>)] = new GenericRequestHandler<TRequest, TResult>(func);
        }

        class GenericRequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
        {
            private readonly Func<TRequest, Task<RequestResult<TResult>>> _func;

            public GenericRequestHandler(Func<TRequest, Task<RequestResult<TResult>>> func)
            {
                _func = func;
            }

            public Task<RequestResult<TResult>> Handle(TRequest request)
            {
                return _func(request);
            }
        }
    }

}
