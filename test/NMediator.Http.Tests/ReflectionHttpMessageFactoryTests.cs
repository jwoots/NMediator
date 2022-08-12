using FluentAssertions;
using NMediator.NMediator.Http;
using NMediator.NMediator.Http.Reflection;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NMediator.Http.Tests
{
    public class ReflectionHttpMessageFactoryTests
    {
        private readonly HttpDescriptors _descriptors = new();
        private ReflectionHttpMessageFactory _reflectionHttpMessageFactory;
        private IHttpMessageFactory _sut;

        public ReflectionHttpMessageFactoryTests()
        {
            _reflectionHttpMessageFactory = new(new Uri("http://test"), _descriptors);
            _sut = _reflectionHttpMessageFactory;
        }

        [Fact]
        public void CreateRequest_must_use_the_provided_HttpRequest()
        {
            var expected = new HttpRequestMessage();
            _reflectionHttpMessageFactory.AddRequestFactory<MyRequest>(_ => expected);

            //ACT
            var result = _sut.CreateRequest(new MyRequest());

            //ASSERT
            result.Should().Be(expected);
        }

        [Fact]
        public void CreateRequest_must_return_HttpRequest_with_body()
        {
            var expected = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://test/testBody"),
                Method = HttpMethod.Post,
                Content = new StringContent("{\"Name\":\"jeff\"}",Encoding.UTF8,"application/json"),
                
            };
            _descriptors.AddFor<MyRequest>(b => b.CallRelativeUri("/testBody", HttpMethod.Post, ParameterLocation.BODY));

            //ACT
            var result = _sut.CreateRequest(new MyRequest() { Name = "jeff"});

            //ASSERT
            result.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
            result.Content.Should().BeOfType<StringContent>()
                .Subject.ReadAsStringAsync().Result.Should().Be(expected.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void CreateRequest_must_return_HttpRequest_with_route_parameter()
        {
            var expected = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://test/testBody/jeff"),
                Method = HttpMethod.Get

            };
            _descriptors.AddFor<MyRequest>(b => b.CallRelativeUri("/testBody/{Name}", HttpMethod.Get, ParameterLocation.QUERY_STRING));

            //ACT
            var result = _sut.CreateRequest(new MyRequest() { Name = "jeff" });

            //ASSERT
            result.RequestUri.Should().BeEquivalentTo(expected.RequestUri, options => options.RespectingRuntimeTypes());
        }

        [Theory]
        [InlineData("Http://host/", "segment", "http://host/segment")]
        [InlineData("Http://host/", "/segment", "http://host/segment")]
        [InlineData("Http://host", "/segment", "http://host/segment")]
        [InlineData("Http://host", "segment", "http://host/segment")]
        public void CreateRequest_must_return_HttpRequest_with_correct_uri(string host, string path, string expectedUri)
        {
            _reflectionHttpMessageFactory = new(new Uri(host), _descriptors);
            _sut = _reflectionHttpMessageFactory;

            _descriptors.AddFor<MyRequest>(b => b.CallRelativeUri(path, HttpMethod.Post, ParameterLocation.BODY));

            //ACT
            var result = _sut.CreateRequest(new MyRequest());

            //ASSERT
            result.RequestUri.Should().BeEquivalentTo(new Uri(expectedUri), options => options.RespectingRuntimeTypes());
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
