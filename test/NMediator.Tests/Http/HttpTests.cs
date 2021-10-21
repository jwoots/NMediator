using FluentAssertions;
using Moq;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Result;
using NMediator.NMediator.Http;
using NMediator.Request;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Http
{
    public class HttpTests
    {
        private MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        private Mock<IHttpMessageFactory> mockHttpMessageFactory = new Mock<IHttpMessageFactory>();

        [Fact]
        public async Task Execute_must_call_provided_options()
        {
            //ARRANGE
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();

            var responseMessage = new HttpResponseMessage();
            mockHttp.When(HttpMethod.Get, "http://test/").Respond(req => responseMessage);
            mockHttpMessageFactory.Setup(x => x.CreateRequest(It.IsAny<object>())).Returns(new HttpRequestMessage(HttpMethod.Get, "http://test/"));
            mockHttpMessageFactory.Setup(x => x.CreateResult<string>(responseMessage)).Returns(RequestResult.Success("Hello World"));

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithHttp(options =>
                {
                    options.HttpMessageFactory = mockHttpMessageFactory.Object;
                    options.HttpClientFactory = () => mockHttp.ToHttpClient();
                }, typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

            //ASSERT
            result.Data.Should().Be("Hello World");
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }

        class MyGenericRequest<T> : IRequest<string>
        {
            public T Data { get; set; }
        }
    }
}
