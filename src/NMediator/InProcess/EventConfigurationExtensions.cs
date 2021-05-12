using NMediator.Core.Transport.Decorator;
using NMediator.Request.Configuration;
using System;

namespace NMediator.InProcess
{
    public static class EventConfigurationExtensions
    {
        public static TransportOptionsConfiguration ExecuteWithInProcess(this RequestConfiguration rc, params Type[] types)
        {
            var inProcessConfig = rc.GetOrRegisterConfiguration(() => new InProcessConfiguration(rc.Container));
            return inProcessConfig.ExecuteWithInProcess(types);
        }
    }
}
