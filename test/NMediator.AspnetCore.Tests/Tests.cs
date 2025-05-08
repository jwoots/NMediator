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
        private readonly CustomWebApplicationFactory<Program> _webAppFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public Tests()
        {
            _webAppFactory = new CustomWebApplicationFactory<Program>();
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
            descriptors.AddFor<HelloWorldQuery>(b => b.CallRelativeUri("/hello/{id}", HttpMethod.Get));
            _webAppFactory.ServicesList.Add(sc => sc.AddSingleton(descriptors));

            //Act
            var client = _webAppFactory.CreateClient();
            var response = await client.GetAsync("/hello/18");
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            JsonSerializer.Deserialize<string>(content).Should().Be("Hello World! 18");
        }

        [Fact]
        public async Task Get_with_parameters()
        {
            //Arrange
            var descriptors = new HttpDescriptors();
            descriptors.AddFor<HelloWorldWithParametersQuery>(b => b.CallRelativeUri("/hello/{Id}", HttpMethod.Get));
            _webAppFactory.ServicesList.Add(sc => sc.AddSingleton(descriptors));

            //Act
            var client = _webAppFactory.CreateClient();
            var response = await client.GetAsync("/hello/22?name=james");
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            JsonSerializer.Deserialize<string>(content).Should().Be("Hello World! 22 james");
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var descriptors = new HttpDescriptors();
            descriptors.AddFor<PostQuery>(b => b.CallRelativeUri("/post", HttpMethod.Post));
            _webAppFactory.ServicesList.Add(sc => sc.AddSingleton(descriptors));

            //Act
            var expectedPostResult = new PostQueryResult { Name = "John", Adress = "Doe" };
            var response = await _webAppFactory.CreateClient().PostAsJsonAsync("/post", new PostQuery
            {
                Name = "John",
                Adress = "Doe"
            });

            var content = await response.Content.ReadAsStringAsync();
            JsonSerializer.Deserialize<PostQueryResult>(content, _jsonOptions).Should().BeEquivalentTo(expectedPostResult);
        }
    }
}
