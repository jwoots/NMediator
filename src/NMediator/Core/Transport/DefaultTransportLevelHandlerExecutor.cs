﻿using NMediator.Core.Activator;
using NMediator.Core.Result;
using NMediator.Request;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public class DefaultTransportLevelHandlerExecutor : ITransportLevelHandlerExecutor
    {
        private readonly IHandlerActivator _handlerActivator;

        public DefaultTransportLevelHandlerExecutor(IHandlerActivator handlerActivator)
        {
            _handlerActivator = handlerActivator;
        }

        public Task<IRequestResult> ExecuteHandler(object message, IDictionary<string, string> headers)
        {
            return _handlerActivator.ProcessMessageHandler(message);
        }
    }
}