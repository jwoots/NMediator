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
         private readonly MockHttpMessageHandler _mockHttpMessageHandler = new MockHttpMessageHandler();
         
        public HttpDescriptorBuilderTests()
        {
        }

        [Fact]
        public async Task TestOverrideBody()
        {
            var request = new MyRequest()
            {
                Property1 = "p1",
                Property2 = "p2"
            };

            var httpDescriptors = new HttpDescriptors();
            httpDescriptors.AddFor<MyRequest>(b => b.CallRelativeUri("/myrelativeURI", HttpMethod.Get, ParameterLocation.BODY).OverrideParameterLocation(x => x.Property2, ParameterLocation.QUERY_STRING));

            var requestExecutor = ConfigureRequestExecutor(httpDescriptors);

            var mock = _mockHttpMessageHandler.Expect("http://test/myrelativeURI");
            mock.WithContent("{\"Property1\"=\"p1\"}");
            mock.WithQueryString("Property2", "p2");
             mock.Respond(() => Task.FromResult(new HttpResponseMessage()));

            var result = await requestExecutor.Execute<MyRequest, Nothing>(request, CancellationToken.None);

            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();

            

        }

        private IRequestExecutor ConfigureRequestExecutor(HttpDescriptors descriptors)
        {
            var configuration = new MediatorConfiguration();
            var serviceActivator = new SimpleServiceActivator();

            configuration.Handling(serviceActivator)
                    .Request(r => r.ExecuteWithHttp(options =>
                    {
                        options.HttpClientFactory = () => _mockHttpMessageHandler.ToHttpClient();
                        options.HttpDescriptors = descriptors;
                        options.BaseUri = new Uri("http://test");
                    }));

            BaseConfiguration.Configure(configuration);
            var requestExecutor = configuration.Container.Get<IRequestExecutor>();
            return requestExecutor;
        }

        class MyRequest
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }
    }
}