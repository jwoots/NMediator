using FluentAssertions;
using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Request;
using NMediator.Result;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Request
{
    public class RequestTests
    {
        [Fact]
        public async Task Request_must_be_executed_correctly()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();

            activator.RegisterRequest<MyRequest, string>(request => Task.FromResult(RequestResult.Success("Hello World "+request.Name)));

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess());

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestProcessor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}
