using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NMediator.AspnetCore.Tests.FakeWebApp.Application;
using NMediator.NMediator.Http.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NMediator.AspnetCore.Tests
{
    public class Tests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public Tests()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task Get_without_parameters()
        {
            //Arrange
            var descriptors = new HttpDescriptors();
            descriptors.AddFor<HelloWorldQuery>(b => b.CallRelativeUri("/hello", HttpMethod.Get));
            _factory.ServicesList.Add(sc => sc.AddSingleton(descriptors));

            //Act
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/hello");
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            JsonSerializer.Deserialize<string>(content).Should().Be("Hello World!");
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var descriptors = new HttpDescriptors();
            descriptors.AddFor<PostQuery>(b => b.CallRelativeUri("/post", HttpMethod.Post));
            _factory.ServicesList.Add(sc => sc.AddSingleton(descriptors));

            //Act
            var expectedPostResult = new PostQueryResult { Name = "John", Adress = "Doe" };
            var response = await _factory.CreateClient().PostAsJsonAsync("/post", new PostQuery
            {
                Name = "John",
                Adress = "Doe"
            });

            var content = await response.Content.ReadAsStringAsync();
            JsonSerializer.Deserialize<PostQueryResult>(content, _jsonOptions).Should().BeEquivalentTo(expectedPostResult);
        }
    }
}
