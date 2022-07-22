using FluentAssertions;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.InProcess
{
    public class InProcessTransportTests
    {
        [Fact]
        public async Task Execute_must_throw_ArgumentNullException()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            activator.RegisterMessage<MyRequest, string>((request, token) => Task.FromResult(RequestResult.Success("Hello World " + request.Name)));

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task<RequestResult<string>>> action = async () => await processor.Execute<MyRequest, string>(null, CancellationToken.None);

            //ASSERT
            await action.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        private class MyRequest
        {
            public string Name { get; set; }
        }
    }
}
