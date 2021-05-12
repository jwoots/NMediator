using NMediator.Core.Configuration;
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
    }
}
