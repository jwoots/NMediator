using FluentAssertions;
using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Transport
{
    public class TransportOptionsTests
    {
        [Fact]
        public async Task Request_must_be_executed_correctly()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();
            var callCount = 0;

            activator.RegisterRequest<MyRequest, string>(request => { callCount++; throw new InvalidOperationException("test"); });

            config.WithActivator(activator)
                .Request(r => 
                            r.ExecuteWithInProcess(typeof(MyRequest))
                             .Retry(3)
                );

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestProcessor>();
            Func<Task> result =  () => processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

            //ASSERT
            await result.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("test");
            callCount.Should().Be(3);
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}
