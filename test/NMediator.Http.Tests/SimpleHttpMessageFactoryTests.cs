using FluentAssertions;
using NMediator.NMediator.Http;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Linq;
using Xunit;
using System.Threading.Tasks;

namespace NMediator.Tests.Http
{
    public class SimpleHttpMessageFactoryTests
    {
        private readonly SimpleHttpMessageFactory _simpleHttpMessageFactory = new SimpleHttpMessageFactory();
        private readonly IHttpMessageFactory _sut;

        public SimpleHttpMessageFactoryTests()
        {
            _sut = _simpleHttpMessageFactory;
        }

        [Fact]
        public void CreateRequest_must_return_configured_HttpRequestMessage()
        {
            //ARRANGE
            HttpRequestMessage expected = new HttpRequestMessage();
            _simpleHttpMessageFactory.AddRequestFactory<MyRequest>(r => expected);

            //ACT
            var result = _sut.CreateRequest(new MyRequest());

            //ASSERT
            result.Should().Be(expected);
            
        }

        [Fact]
        public void CreateRequest_must_throw_InvalidOperationException_when_no_type_registered_for_message()
        {
            //ARRANGE
            HttpRequestMessage expected = new HttpRequestMessage();
            _simpleHttpMessageFactory.AddRequestFactory<MyRequest>(r => expected);

            //ACT
            Action action = () => _sut.CreateRequest(new MyRequest2());

            //ASSERT
            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [MemberData(nameof(HttpStatusCode2xx))]
        public async Task CreateResult_must_return_success_result_for_response_200(HttpStatusCode statusCode)
        {
            //ARRANGE
            HttpResponseMessage expected = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent("\"response content\"")
            };

            //ACT
            var result = await _sut.CreateResult<string>(expected);

            //ASSERT
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("response content");
        }

        public static IEnumerable<object[]> HttpStatusCode2xx()
        {
            return ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                .Where(x => (int)x >= 200 && (int)x < 300)
                .Select(x => new object[] { x });
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
