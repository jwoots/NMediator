using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Result;
using NMediator.NMediator.Http.Reflection;
using NMediator.Request;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace NMediator.Http.Tests
{
    public class BindingTests
    {
        private readonly MediatorConfiguration _configuration = new MediatorConfiguration();
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new MockHttpMessageHandler();
        private readonly IRequestExecutor _requestExecutor;
        private readonly HttpDescriptors _httpDescriptors = new HttpDescriptors();
        private readonly HttpDescriptor _httpDescriptor = new HttpDescriptor()
        {
            Method = System.Net.Http.HttpMethod.Get,
            ParameterLocation = ParameterLocation.QUERY_STRING,
            RelativeUri = "/myrelativeURI"
        };

        public BindingTests()
        {
            var serviceActivator = new SimpleServiceActivator();
            

            _configuration.WithActivator(serviceActivator)
                    .Request(r => r.ExecuteWithHttp(options =>
                    {
                        options.HttpClientFactory = () => _mockHttpMessageHandler.ToHttpClient();
                        options.HttpMessageFactory = new ReflectionHttpMessageFactory(new ReflectionHttpMessageOptions()
                        {
                            BaseUri = new Uri("http://test/"),
                            Descriptors = _httpDescriptors
                        });
                    }, typeof(MyQueryStringTestRequest)));

            BaseConfiguration.Configure(_configuration);
            _requestExecutor = _configuration.Container.Get<IRequestExecutor>();
            
        }

        [Fact]
        public void Execute_must_send_http_request_with_correct_querystring_Binding()
        {
            var date = DateTime.Now;
            var request = new MyQueryStringTestRequest
            {
                StringProperty = "test:value",
                ArrayProperty = new decimal[] {1.18m, -5.87m},
                IEnumerableProperty = new int[] {6, 9, -50},
                IntProperty = 35,
                ListProperty = new List<string>(),
                NullableProperty = 15,
                NullValueProperty = null,
                DateProperty = date,
            };

            _httpDescriptors.AddFor<MyQueryStringTestRequest>(_httpDescriptor);

            _mockHttpMessageHandler.Expect("http://test/myrelativeURI")
                                    .WithQueryString(new List<KeyValuePair<string, string>>
                                    {
                                        { KeyValuePair.Create("StringProperty", "test:value" )},
                                        { KeyValuePair.Create("ArrayProperty", "1.18") },
                                        { KeyValuePair.Create("ArrayProperty", "-5.87") },
                                        { KeyValuePair.Create("IEnumerableProperty", "6") },
                                        { KeyValuePair.Create("IEnumerableProperty", "9")},
                                        { KeyValuePair.Create("IEnumerableProperty", "-50") },
                                        { KeyValuePair.Create("IntProperty", "35") },
                                        { KeyValuePair.Create("NullableProperty","15") },
                                        { KeyValuePair.Create("DateProperty", date.ToString(CultureInfo.InvariantCulture)) }
                                    });

            _requestExecutor.Execute<MyQueryStringTestRequest, Nothing>(request);
            
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        class MyQueryStringTestRequest : IRequest<Nothing>
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public int? NullableProperty { get; set; }
            public int? NullValueProperty { get; set; }
            public IEnumerable<int> IEnumerableProperty { get; set; }
            public List<string> ListProperty { get; set; }
            public decimal[] ArrayProperty { get; set; }
            public DateTime DateProperty { get; set; }  

        }
    }
}