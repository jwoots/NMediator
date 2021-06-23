using FluentAssertions;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Request;
using System;
using System.Threading.Tasks;
using Xunit;

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

            activator.RegisterMessage<MyRequest, string>(request => { callCount++; throw new InvalidOperationException("test"); });

            config.WithActivator(activator)
                .Request(r => 
                            r.ExecuteWithInProcess(typeof(MyRequest))
                             .Retry(3)
                );

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task> result =  () => processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

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

            activator.RegisterMessage<MyRequest, string>(request => { myrequestCallCount++; throw new InvalidOperationException("test"); });
            activator.RegisterMessage<MyRequest2, string>(request => { myRequest2CallCount++; throw new InvalidOperationException("test2"); });

            config.WithActivator(activator)
                .Request(r =>
                {
                    r.ExecuteWithInProcess(typeof(MyRequest)).Retry(3);
                    r.ExecuteWithInProcess(typeof(MyRequest2)).Retry(2);
                });

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task> result = () => processor.Execute<MyRequest2, string>(new MyRequest2() { Name = "jwoots" });

            //ASSERT
            await result.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("test2");
            myRequest2CallCount.Should().Be(2);
            myrequestCallCount.Should().Be(0);
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }

        class MyRequest2 : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}
