﻿using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Event;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Activator
{
    public static class IServiceActivatorExtensions
    {
        public static async Task<IRequestResult> ProcessMessageHandler(this IServiceActivator handlerActivator, object message, CancellationToken token)
        {
            var messageType = message.GetType();
            var responseType = messageType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessage<>)).GetGenericArguments()[0];
            var handlerType = typeof(IMessageHandler<,>).MakeGenericType(messageType, responseType);

            IEnumerable<dynamic> handlers = handlerActivator.GetInstances(handlerType);
            if (handlers.Count() > 1 && responseType != typeof(Nothing))
                throw new InvalidOperationException("more than 1 handler found for message with result");

            try
            {
                if (handlers.Count() == 1)
                {
                    dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handlers.First(), new object[] { message, token });
                    await awaitable;
                    return (IRequestResult)awaitable.GetAwaiter().GetResult();
                }
                else
                {
                    foreach (var handler in handlers)
                    {
                        dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handler, new object[] { message, token });
                        await awaitable;
                    }

                    return null;
                }
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
