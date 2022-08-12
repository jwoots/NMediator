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
using NMediator.Core.Result;

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
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent("\"response content\"")
            };

            //ACT
            var result = await _sut.CreateResult<string>(response);

            //ASSERT
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("response content");
        }

        [Theory]
        [MemberData(nameof(HttpStatusCode3xx))]
        public async Task CreateResult_must_throw_NotSupportedException(HttpStatusCode statusCode)
        {
            //ARRANGE
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent("\"response content\"")
            };

            //ACT
            var result = () => _sut.CreateResult<string>(response);

            //ASSERT
            await result.Should().ThrowExactlyAsync<NotSupportedException>();
        }

        [Theory]
        [MemberData(nameof(HttpStatusCode4xx))]
        public async Task CreateResult_must_return_fail_result(HttpStatusCode statusCode)
        {
            //ARRANGE
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent("response content")
            };

            var expected = RequestResult.Fail<string>(new Error() { Code = statusCode.ToString(), Description = "response content" });

            //ACT
            var result = await _sut.CreateResult<string>(response);

            //ASSERT
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(HttpStatusCode5xx))]
        public async Task CreateResult_must_throw_exception(HttpStatusCode statusCode)
        {
            //ARRANGE
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent("response content")
            };

            //ACT
            var result = () => _sut.CreateResult<string>(response);

            //ASSERT
            await result.Should().ThrowExactlyAsync<Exception>().WithMessage("response content");
        }

        [Fact]
        public async Task CreateResult_must_return_fail_result_according_configuration()
        {
            //ARRANGE
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Content = new StringContent("a not functional content")
            };

            var expectedError = new Error() { Code = "ERR_PROCESS", Description = "a functional content" };
            var expected = RequestResult.Fail<string>(expectedError);
            _simpleHttpMessageFactory.AddErrorFactory(HttpStatusCode.UnprocessableEntity, _ => expectedError);

            //ACT
            var result = await _sut.CreateResult<string>(response);

            //ASSERT
            result.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
        }

        [Fact]
        public async Task CreateResult_must_throw_exception_according_configuration()
        {
            //ARRANGE
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("response content")
            };
            _simpleHttpMessageFactory.AddExceptionFactory(HttpStatusCode.Unauthorized, _ => new AccessViolationException());

            //ACT
            var result = () => _sut.CreateResult<string>(response);

            //ASSERT
            await result.Should().ThrowExactlyAsync<AccessViolationException>();
        }

        public static IEnumerable<object[]> HttpStatusCode2xx()
        {
            return ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                .Where(x => (int)x >= 200 && (int)x < 300)
                .Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> HttpStatusCode3xx()
        {
            return ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                .Where(x => (int)x >= 300 && (int)x < 400)
                .Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> HttpStatusCode4xx()
        {
            return ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                .Where(x => (int)x >= 400 && (int)x < 500)
                .Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> HttpStatusCode5xx()
        {
            return ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                .Where(x => (int)x >= 500)
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
