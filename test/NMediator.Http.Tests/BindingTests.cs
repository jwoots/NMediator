using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Result;
using NMediator.NMediator.Http.Reflection;
using NMediator.Request;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace NMediator.Http.Tests
{
    public class BindingTests
    {
        private readonly MediatorConfiguration _configuration = new MediatorConfiguration();
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new MockHttpMessageHandler();
        private readonly HttpDescriptors _httpDescriptors = new HttpDescriptors();
        private readonly HttpDescriptor _httpDescriptor = new HttpDescriptor()
        {
            Method = System.Net.Http.HttpMethod.Get,
            ParameterLocation = ParameterLocation.QUERY_STRING,
            RelativeUri = "/myrelativeURI"
        };

        public IRequestExecutor ConfigureRequestExecutor()
        {
            var serviceActivator = new SimpleServiceActivator();

            _configuration.WithActivator(serviceActivator)
                    .Request(r => r.ExecuteWithHttp(options =>
                    {
                        options.HttpClientFactory = () => _mockHttpMessageHandler.ToHttpClient();
                        options.HttpDescriptors = _httpDescriptors;
                        options.BaseUri = new Uri("http://test");
                    }));

            BaseConfiguration.Configure(_configuration);
            var requestExecutor = _configuration.Container.Get<IRequestExecutor>();
            return requestExecutor;
        }

        //[Fact]
        //public async Task Execute_must_send_http_request_with_correct_querystring_Binding()
        //{
        //    var date = DateTime.Now;
        //    var request = new MyQueryStringTestRequest
        //    {
        //        StringProperty = "test:value",
        //        ArrayProperty = new decimal[] {1.18m, -5.87m},
        //        IEnumerableProperty = new int[] {6, 9, -50},
        //        IntProperty = 35,
        //        ListProperty = new List<string>(),
        //        NullableProperty = 15,
        //        NullValueProperty = null,
        //        DateProperty = date,
        //    };

        //    _httpDescriptors.AddFor<MyQueryStringTestRequest>(_httpDescriptor);

        //    _mockHttpMessageHandler.Expect("http://test/myrelativeURI")
        //                            .WithQueryString(new List<KeyValuePair<string, string>>
        //                            {
        //                                { KeyValuePair.Create("StringProperty", "test:value" )},
        //                                { KeyValuePair.Create("ArrayProperty", "1.18") },
        //                                { KeyValuePair.Create("ArrayProperty", "-5.87") },
        //                                { KeyValuePair.Create("IEnumerableProperty", "6") },
        //                                { KeyValuePair.Create("IEnumerableProperty", "9")},
        //                                { KeyValuePair.Create("IEnumerableProperty", "-50") },
        //                                { KeyValuePair.Create("IntProperty", "35") },
        //                                { KeyValuePair.Create("NullableProperty","15") },
        //                                { KeyValuePair.Create("DateProperty", date.ToString(CultureInfo.InvariantCulture)) }
        //                            });

        //    await _requestExecutor.Execute<MyQueryStringTestRequest, Nothing>(request);

        //    _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        //}

        [Theory]
        [InlineData("", "")]
        [InlineData("test://35", "test://35")]
        [InlineData(" ", " ")]
        [InlineData(null, "#NO_PRESENT#")]
        public async Task Execute_must_bind_String_property_to_query_string(string value, string expected)
        {
            await TestBinding(value, expected);
        }

        [Theory]
        [InlineData(5, "5")]
        [InlineData(0, "0")]
        [InlineData(-56, "-56")]
        public async Task Execute_must_bind_int_property_to_query_string(int value, string expected)
        {
            await TestBinding(value, expected);
        }

        [Theory]
        [InlineData(5, "5")]
        [InlineData(0, "0")]
        [InlineData(-56, "-56")]
        [InlineData(null, "#NO_PRESENT#")]
        public async Task Execute_must_bind_nullable_int_property_to_query_string(int? value, string expected)
        {
            await TestBinding(value, expected);
        }

        [Fact]
        public async Task Execute_must_bind_Enumerable_property_to_query_string()
        {
            var list = new List<decimal?> { 1, 2.38m, -10.9453m, 0, null };
            await TestBinding<IEnumerable<decimal?>>(list,  "1", "2.38", "-10.9453" );
        }

        [Fact]
        public async Task Execute_must_bind_Array_property_to_query_string()
        {
            var list = new string[] { "a", "b://c", null };
            await TestBinding(list, "a", "b://c");
        }

        [Fact]
        public async Task Execute_must_bind_List_property_to_query_string()
        {
            var now = DateTimeOffset.Now;
            var tomorrow = now.AddDays(1);
            var list = new List<DateTimeOffset> { now, tomorrow };
            await TestBinding(list, now.ToString(CultureInfo.InvariantCulture), now.ToString(CultureInfo.InvariantCulture));
        }


        private async Task TestBinding<T>(T value, params string[] expectedParameters)
        {
            var request = new MyQueryStringRequest<T>()
            {
                Property = value
            };

            _httpDescriptors.AddFor<MyQueryStringRequest<T>>(_httpDescriptor);

            var requestExecutor = ConfigureRequestExecutor();
            string expectedQueryStringKey = "Property";

            var mock = _mockHttpMessageHandler.Expect("http://test/myrelativeURI");

            foreach (var expected in expectedParameters)
            {
                if (expected != "#NO_PRESENT#")
                    mock.WithQueryString(expectedQueryStringKey, expected);
                else
                {
                    mock.With(m =>
                    {
                        var queryString = HttpUtility.ParseQueryString(m.RequestUri.Query);
                        return !queryString.AllKeys.Contains(expectedQueryStringKey);
                    });
                }
            }

            mock.Respond(() => Task.FromResult(new HttpResponseMessage()));

            await requestExecutor.Execute<MyQueryStringRequest<T>, Nothing>(request, CancellationToken.None);

            VerifyQueryStringExpectation(expectedQueryStringKey, string.Join(',',expectedParameters));
        }

        private void VerifyQueryStringExpectation(string exepctedKey, string expectedValue)
        {
            try
            {
                _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"query string not match for key {exepctedKey} and value {expectedValue}", ex);
            }
        }


        class MyQueryStringTestRequest
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

        class MyQueryStringRequest<T>
        {
            public T Property { get; set; }
        }
    }
}