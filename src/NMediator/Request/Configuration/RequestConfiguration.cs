using NMediator.Activator;
using NMediator.Configuration;
using NMediator.InProcess;
using NMediator.SInjector;

namespace NMediator.Request.Configuration
{
    public class RequestConfiguration : BaseConfiguration
    {
        private readonly Container _container;

        public RequestConfiguration(Container container)
        {
            _container = container;
        }

        public RequestConfiguration ExecuteWithInProcess()
        {
            _container.Register<IRequestProcessor>(ctx => new InProcessRequestProcessor(ctx.Get<IHandlerActivator>()));
            return this;
        }
    }
}
