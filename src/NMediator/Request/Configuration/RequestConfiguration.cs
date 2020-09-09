using NMediator.Configuration;
using NMediator.InProcess;
using NMediator.SInjector;
using System;

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
            var inProcessProcessor = new InProcessRequestProcessor()
            _container.Register<IRequestProcessor>()
        }
    }
}
