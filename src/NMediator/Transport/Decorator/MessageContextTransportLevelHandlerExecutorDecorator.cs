using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport.Decorator
{
    public class MessageContextTransportLevelHandlerExecutorDecorator : ITransportLevelHandlerExecutor
    {
        private readonly ITransportLevelHandlerExecutor _decoratee;

        public Task<IRequestResult> ExecuteHandler(object message)
        {
            return _decoratee.ExecuteHandler(message);
        }
    }
}
