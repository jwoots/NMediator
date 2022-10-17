using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMediator.Core.Handling;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Delegating service activator
    /// </summary>
    public class DelegateServiceActivator : IServiceActivator
    {
        private readonly Func<Type, IEnumerable<object>> _func;

        public DelegateServiceActivator(Func<Type, IEnumerable<object>> func)
        {
            _func = func;
        }

        public IEnumerable<object> GetInstances(Type handlerInterfaceType)
        {
            return _func(handlerInterfaceType);
        }
    }
}
