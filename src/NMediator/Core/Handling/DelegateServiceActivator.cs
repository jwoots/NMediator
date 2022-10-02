using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMediator.Core.Handling;

namespace NMediator.Core.Handling
{
    public class DelegateServiceActivator : IServiceActivator
    {
        private readonly Func<Type, IEnumerable<object>> _func;

        public DelegateServiceActivator(Func<Type, IEnumerable<object>> func)
        {
            _func = func;
        }

        public IEnumerable<THandler> GetInstances<THandler>()
        {
            return GetInstances(typeof(THandler)).Cast<THandler>();
        }

        public IEnumerable<object> GetInstances(Type handlerType)
        {
            return _func(handlerType);
        }
    }
}
