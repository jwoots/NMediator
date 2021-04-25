using NMediator.InProcess;
using NMediator.Request.Configuration;
using NMediator.Transport.Decorator;
using System;

namespace NMediator.Configuration
{
    public static class RequestConfigurationExtensions
    {
        public static TransportOptionsConfiguration ExecuteWithInProcess(this RequestConfiguration rc, params Type[] types)
        {
            var inProcessConfig = rc.GetOrRegisterConfiguration(() => new InProcessConfiguration(rc.Container));
            return inProcessConfig.ExecuteWithInProcess(types);
        }
    }
}
