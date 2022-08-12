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
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new MockHttpMessageHandler();

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

            var httpDescriptors = new HttpDescriptors();
            httpDescriptors.AddFor<MyQueryStringRequest<T>>(b => b.CallRelativeUri("/myrelativeURI", HttpMethod.Get, ParameterLocation.QUERY_STRING));

            var requestExecutor = ConfigureRequestExecutor(httpDescriptors);
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

        private IRequestExecutor ConfigureRequestExecutor(HttpDescriptors descriptors)
        {
            var configuration = new MediatorConfiguration();
            var serviceActivator = new SimpleServiceActivator();

            configuration.WithActivator(serviceActivator)
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