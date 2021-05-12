using NMediator.Core.Activator;
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
            RoutedRequestProcessor routedRequestProcessor = null;
            Container.Register<IRouter>(ctx => typeBasedRouter);
            Container.Register(ctx => typeBasedRouter);
            Container.Register<IRequestProcessor>(ctx => routedRequestProcessor ??= new RoutedRequestProcessor(ctx.Get<IRouter>()));
            Container.Register<ITransportLevelHandlerExecutor>(ctx => new DefaultTransportLevelHandlerExecutor(ctx.Get<IHandlerActivator>()));
            Container.Decorate<ITransportLevelHandlerExecutor>(ctx => new MessageContextTransportLevelHandlerExecutorDecorator(ctx.Get<ITransportLevelHandlerExecutor>()));
        }

        public MediatorConfiguration WithActivator(IHandlerActivator activator)
        {
            Container.Register<IHandlerActivator>(ctx => activator);
            return this;
        }
    }
}
