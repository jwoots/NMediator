using NMediator.Core.Transport.Decorator;
using NMediator.InProcess;
using NMediator.Request.Configuration;
using System;

namespace NMediator.Core.Configuration
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
