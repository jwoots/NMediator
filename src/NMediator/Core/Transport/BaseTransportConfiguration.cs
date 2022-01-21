using NMediator.Core.Configuration;
using NMediator.Core.Transport.Decorator;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Core.Transport
{
    public class TransportConfiguration : BaseConfiguration
    {
        public ICollection<TransportOptionsConfiguration> TransportConfigurations { get; } = new List<TransportOptionsConfiguration>();

        protected override void Resolve()
        {
            base.Resolve();
            TransportConfigurations.ForEach(tc => tc.Configure());
        }
    }
}
