using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.Core.SInjector;

namespace NMediator.Event
{
    public class EventConfiguration : BaseConfiguration
    {
        public Container Container { get; }

        public EventConfiguration(Container container)
        {
            Container = container;
        }

        protected override void Register()
        {
            EventPublisher ep = null;
            Container.Register<IEventPublisher>(ctx => ep ??= new EventPublisher(ctx.Get<IMessageProcessor>()));
        }
    }
}
