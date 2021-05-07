using NMediator.Event;
using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Activator
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
            dynamic handlerResult = handlerActivator.GetInstances(handlerType);
            try
            {
                dynamic awaitable = handlerType.GetMethod("Handle").Invoke(handlerResult, new object[] { request });
                await awaitable;
                return (IRequestResult)awaitable.GetAwaiter().GetResult();
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
