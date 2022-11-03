using System.Threading;
using FluentAssertions;
using NMediator.Core.Configuration;
using NMediator.Core.Handling;
using NMediator.NMediator.Http;
using NMediator.NMediator.Http.Reflection;
using NMediator.Request;
using RichardSzalay.MockHttp;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NMediator.Core.Result;

namespace NMediator.Http.Tests
{
    public class HttpDescriptorBuilderTests
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new ();
        private readonly HttpDescriptors _descriptors = new ();
        private readonly MediatorConfiguration configuration = new ();

        public HttpDescriptorBuilderTests()
        {
            var serviceActivator = new SimpleServiceActivator();
            serviceActivator.RegisterMessage<MyRequest, Nothing>((m, ct) => Task.FromResult(RequestResult.Success()));

            configuration.Handling(serviceActivator)
                    .Request(r => r.ExecuteWithHttp(options =>
                    {
                        options.HttpClientFactory = () => _mockHttpMessageHandler.ToHttpClient();
                        options.HttpDescriptors = _descriptors;
                        options.BaseUri = new Uri("http://test");
                    }));

        }

        private IRequestExecutor BuildRequestExecutor()
        {
            BaseConfiguration.Configure(configuration);
            return configuration.Container.Get<IRequestExecutor>();
        }

        [Fact]
        public async Task TestOverrideBody()
        {
            //ARRANGE
            var request = new MyRequest()
            {
                Property1 = "p1",
                Property2 = "p2"
            };

            _descriptors.AddFor<MyRequest>(b => b.CallRelativeUri("/myrelativeURI", HttpMethod.Post, ParameterLocation.BODY).OverrideParameterLocation(x => x.Property2, ParameterLocation.QUERY_STRING));

            var mock = _mockHttpMessageHandler.Expect("http://test/myrelativeURI");
            mock.WithContent("{\"Property1\":\"p1\"}");
            mock.WithQueryString("Property2", "p2");
            mock.Respond(() => Task.FromResult(new HttpResponseMessage()));

            //ACT
            await BuildRequestExecutor().Execute<MyRequest, Nothing>(request, CancellationToken.None);

            //ASSERT
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task TestOverrideQuery()
        {
            //ARRANGE
            var request = new MyRequest()
            {
                Property1 = "p1",
                Property2 = "p2"
            };

            _descriptors.AddFor<MyRequest>(b => b.CallRelativeUri("/myrelativeURI", HttpMethod.Get, ParameterLocation.QUERY_STRING)
                                                    .OverrideParameterLocation(x => x.Property2, ParameterLocation.BODY));
            var mock = _mockHttpMessageHandler.Expect("http://test/myrelativeURI");
            mock.WithContent("{\"Property2\":\"p2\"}");
            mock.WithQueryString("Property1", "p1");
            mock.Respond(() => Task.FromResult(new HttpResponseMessage()));

            //ACT
            await BuildRequestExecutor().Execute<MyRequest, Nothing>(request, CancellationToken.None);

            //ASSERT
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        class MyRequest
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }
    }
}