using NMediator.Core.Activator;
using NMediator.Core.Context;
using NMediator.Core.Message;
using NMediator.Core.Routing;
using NMediator.Core.SInjector;
using NMediator.Core.Transport;
using NMediator.Core.Transport.Decorator;
using NMediator.Request;

namespace NMediator.Core.Configuration
{
    public class MediatorConfiguration : BaseConfiguration
    {
        public Container Container { get; } = new Container();

        public MediatorConfiguration()
        {
            var typeBasedRouter = new TypeBasedRouter();
            RoutedMessageProcessor routedRequestProcessor = null;
            Container.Register<IRouter>(ctx => typeBasedRouter);
            Container.Register(ctx => typeBasedRouter);
            Container.Register<IMessageProcessor>(ctx => routedRequestProcessor ??= new RoutedMessageProcessor(ctx.Get<IRouter>()));
            Container.Register<ITransportLevelHandlerExecutor>(ctx => new DefaultTransportLevelHandlerExecutor(ctx.Get<IServiceActivator>(), ctx.Get<IHandlerProvider>()));
            Container.Decorate<ITransportLevelHandlerExecutor>(ctx => new MessageContextTransportLevelHandlerExecutorDecorator(ctx.Get<ITransportLevelHandlerExecutor>()));
            Container.Decorate<IMessageProcessor>(ctx => new DefaultHeadersMessageProcessorDecorator(ctx.Get<IMessageProcessor>()));
        }

        public MediatorConfiguration WithActivator(IServiceActivator activator, IHandlerProvider handlerProvider)
        {
            Container.Register(ctx => activator);
            Container.Register(ctx => handlerProvider);
            return this;
        }

        public MediatorConfiguration WithActivator(SimpleServiceActivator activator)
            => WithActivator(activator, activator);
    }
}
