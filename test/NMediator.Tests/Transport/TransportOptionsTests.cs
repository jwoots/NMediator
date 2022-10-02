using FluentAssertions;
using NMediator.Core.Configuration;
using NMediator.Request;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NMediator.Core.Handling;

namespace NMediator.Tests.Transport
{
    public class TransportOptionsTests
    {
        [Fact]
        public async Task Retry_configuration_must_be_applied()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();
            var callCount = 0;

            activator.RegisterMessage<MyRequest, string>((request, token) => { callCount++; throw new InvalidOperationException("test"); });

            config.Handling(activator)
                .Request(r => 
                            r.ExecuteWithInProcess(typeof(MyRequest))
                             .Retry(3)
                );

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task> result =  () => processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None);

            //ASSERT
            await result.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("test");
            callCount.Should().Be(3);
        }

        [Fact]
        public async Task Retry_configuration_must_be_applied_on_good_type()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();
            var myrequestCallCount = 0;
            var myRequest2CallCount = 0;

            activator.RegisterMessage<MyRequest, string>((request, token) => { myrequestCallCount++; throw new InvalidOperationException("test"); });
            activator.RegisterMessage<MyRequest2, string>((request, token) => { myRequest2CallCount++; throw new InvalidOperationException("test2"); });

            config.Handling(activator)
                .Request(r =>
                {
                    r.ExecuteWithInProcess(typeof(MyRequest)).Retry(3);
                    r.ExecuteWithInProcess(typeof(MyRequest2)).Retry(2);
                });

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task> result = () => processor.Execute<MyRequest2, string>(new MyRequest2() { Name = "jwoots" }, CancellationToken.None);

            //ASSERT
            await result.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("test2");
            myRequest2CallCount.Should().Be(2);
            myrequestCallCount.Should().Be(0);
        }

        class MyRequest
        {
            public string Name { get; set; }
        }

        class MyRequest2
        {
            public string Name { get; set; }
        }
    }
}
