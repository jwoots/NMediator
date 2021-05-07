using NMediator.Request.Configuration;
using NMediator.Transport.Decorator;
using System;
using System.Collections.Generic;
using System.Text;

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
