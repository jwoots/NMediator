using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.Core.SInjector;

namespace NMediator.Request.Configuration
{
    public class RequestConfiguration : BaseConfiguration
    {
        public Container Container { get; }
        
        public RequestConfiguration(Container container)
        {
            Container = container;
        }

        protected override void Register()
        {
            RequestExecutor re = null;
            Container.Register<IRequestExecutor>(ctx => re ??= new RequestExecutor(ctx.Get<IMessageProcessor>()));
        }
    }
}
