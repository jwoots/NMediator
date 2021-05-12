using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Event;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace NMediator.Core.Activator
{
    public static class IHandlerActivatorExtensions
    {
        public static async Task ExecuteEventHandlers(this IHandlerActivator handlerActivator, object @event)
        {
            var requestType = @event.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(requestType);

            dynamic param = @event;
            IEnumerable<dynamic> handlers = handlerActivator.GetInstances(handlerType);
            try
            {
                foreach (var handler in handlers)
                {
                    dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    await awaitable;
                }
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        public static async Task<IRequestResult> ExecuteRequestHandler(this IHandlerActivator handlerActivator, object request)
        {
            var requestType = request.GetType();
            var responseType = requestType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            dynamic param = request;
            IEnumerable<dynamic> handlers = handlerActivator.GetInstances(handlerType);
            
            try
            {
                dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handlers.First(), new object[] { request });
                await awaitable;
                return (IRequestResult)awaitable.GetAwaiter().GetResult();
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        public static async Task<IRequestResult> ProcessMessageHandler(this IHandlerActivator handlerActivator, object message)
        {
            var messageType = message.GetType();
            var responseType = messageType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessage<>)).GetGenericArguments()[0];
            var handlerType = typeof(IMessageHandler<,>).MakeGenericType(messageType, responseType);

            dynamic param = message;
            IEnumerable<dynamic> handlers = handlerActivator.GetInstances(handlerType);
            if (handlers.Count() > 1 && responseType != typeof(Nothing))
                throw new InvalidOperationException("more than 1 handler found for message with result");

            try
            {
                if (handlers.Count() == 1)
                {
                    dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handlers.First(), new object[] { message });
                    await awaitable;
                    return (IRequestResult)awaitable.GetAwaiter().GetResult();
                }
                else
                {
                    foreach (var handler in handlers)
                    {
                        dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handler, new object[] { message });
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
