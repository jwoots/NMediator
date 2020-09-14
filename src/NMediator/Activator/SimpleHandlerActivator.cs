using System;
using System.Collections.Generic;

namespace NMediator.Activator
{
    public class SimpleHandlerActivator : IHandlerActivator
    {
        private readonly IDictionary<Type, Func<object>> _handlers = new Dictionary<Type, Func<object>>();

        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public object GetInstance(Type type)
        {
            if (_handlers.TryGetValue(type, out var result))
            {
                return result();
            }

            throw new InvalidOperationException($"there is no handler instance for type {typeof(T)}");
        }
    }
}
