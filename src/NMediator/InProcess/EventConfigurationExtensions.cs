using NMediator.Core.Transport.Decorator;
using NMediator.Event;
using NMediator.InProcess;
using System;

namespace NMediator.Core.Configuration
{
    public static class InProccessEventConfigurationExtensions
    {
        public static TransportOptionsConfiguration ExecuteWithInProcess(this EventConfiguration rc, params Type[] types)
        {
            var inProcessConfig = rc.GetOrRegisterConfiguration(() => new InProcessConfiguration(rc.Container));
            return inProcessConfig.ExecuteWithInProcess(types);
        }
    }
}
