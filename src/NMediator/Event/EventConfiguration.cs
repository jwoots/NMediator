using NMediator.Core.Configuration;
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
    }
}
