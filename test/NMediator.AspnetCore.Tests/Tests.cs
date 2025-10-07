using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NMediator.AspnetCore.Tests.FakeWebApp.Application;
using NMediator.Core.Handling;
using NMediator.Core.Result;
using NMediator.NMediator.Http.Reflection;
using System.Net.Http.Json;
using System.Text.Json;

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

        private class PutQuery
        {
            public string Name { get; set; }
            public string Adress { get; set; }
        }

        private class PutQueryResult
        {
            public string Name { get; set; }
            public string Adress { get; set; }
        }
        [Fact]
        public async Task Put()
        {
            //Arrange
            SimpleServiceActivator activator = new();
            activator.RegisterMessage<PutQuery, PutQueryResult>((request, token) =>
            {
                return Task.FromResult(RequestResult.Success(new PutQueryResult
                {
                    Name = request.Name,
                    Adress = request.Adress
                }));
            });

            var descriptors = new HttpDescriptors();
            descriptors.AddFor<PutQuery>(b => b.CallRelativeUri("/put", HttpMethod.Put));

            _webAppFactory.SetServiceActivator(activator);
            _webAppFactory.SetHttpDescriptors(descriptors);

            //Act
            var expectedPutResult = new PutQuery { Name = "John", Adress = "Doe" };
            var response = await _webAppFactory.CreateClient().PutAsJsonAsync("/put", new PutQuery
            {
                Name = "John",
                Adress = "Doe"
            });

            //Assert
            var content = await response.Content.ReadAsStringAsync();
            JsonSerializer.Deserialize<PutQueryResult>(content, _jsonOptions).Should().BeEquivalentTo(expectedPutResult);
        }

        private class GetAdvancedTypesQuery
        {
            public DateTime Date { get; set; }
            public DateTime? NullableDate { get; set; }
            public DateTimeOffset DateOffset { get; set; }
            public DateTimeOffset? NullableDateOffset { get; set; }
            public float FloatValue { get; set; }
            public float? NullableFloat { get; set; }
            public double DoubleValue { get; set; }
            public double? NullableDouble { get; set; }
            public long LongValue { get; set; }
            public long? NullableLong { get; set; }
        }

        private class GetAdvancedTypesResult
        {
            public DateTime Date { get; set; }
            public DateTime? NullableDate { get; set; }
            public DateTimeOffset DateOffset { get; set; }
            public DateTimeOffset? NullableDateOffset { get; set; }
            public float FloatValue { get; set; }
            public float? NullableFloat { get; set; }
            public double DoubleValue { get; set; }
            public double? NullableDouble { get; set; }
            public long LongValue { get; set; }
            public long? NullableLong { get; set; }
        }

        [Fact]
        public async Task Get_binding_advanced_types()
        {
            // Arrange
            SimpleServiceActivator activator = new();
            activator.RegisterMessage<GetAdvancedTypesQuery, GetAdvancedTypesResult>((request, token) =>
            {
                return Task.FromResult(RequestResult.Success(new GetAdvancedTypesResult
                {
                    Date = request.Date,
                    NullableDate = request.NullableDate,
                    DateOffset = request.DateOffset,
                    NullableDateOffset = request.NullableDateOffset,
                    FloatValue = request.FloatValue,
                    NullableFloat = request.NullableFloat,
                    DoubleValue = request.DoubleValue,
                    NullableDouble = request.NullableDouble,
                    LongValue = request.LongValue,
                    NullableLong = request.NullableLong
                }));
            });

            var descriptors = new HttpDescriptors();
            descriptors.AddFor<GetAdvancedTypesQuery>(b => b.CallRelativeUri("/get-advanced", HttpMethod.Get));

            _webAppFactory.SetServiceActivator(activator);
            _webAppFactory.SetHttpDescriptors(descriptors);

            // Act
            var url = "/get-advanced"
                + "?date=2024-01-01T12:34:56"
                + "&nullableDate=2024-12-31T23:59:59"
                + "&dateOffset=2024-01-01T12:34:56%2B02:00"
                + "&nullableDateOffset=2024-12-31T23:59:59%2B01:00"
                + "&floatValue=1.23"
                + "&nullableFloat=4.56"
                + "&doubleValue=7.89"
                + "&nullableDouble=0.12"
                + "&longValue=1234567890123"
                + "&nullableLong=9876543210987";
            var response = await _webAppFactory.CreateClient().GetAsync(url);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetAdvancedTypesResult>(content, _jsonOptions);
            result.Should().BeEquivalentTo(new GetAdvancedTypesResult
            {
                Date = new DateTime(2024, 1, 1, 12, 34, 56),
                NullableDate = new DateTime(2024, 12, 31, 23, 59, 59),
                DateOffset = new DateTimeOffset(2024, 1, 1, 12, 34, 56, TimeSpan.FromHours(2)),
                NullableDateOffset = new DateTimeOffset(2024, 12, 31, 23, 59, 59, TimeSpan.FromHours(1)),
                FloatValue = 1.23f,
                NullableFloat = 4.56f,
                DoubleValue = 7.89,
                NullableDouble = 0.12,
                LongValue = 1234567890123L,
                NullableLong = 9876543210987L
            });
        }

        private class GetQuery
        {
            public string Name { get; set; }
            public int? Age { get; set; }
            public List<string> Tags { get; set; }
        }

        private class GetQueryResult
        {
            public string Name { get; set; }
            public int? Age { get; set; }
            public List<string> Tags { get; set; }
        }

        [Fact]
        public async Task Get_binding_simple_types()
        {
            // Arrange
            SimpleServiceActivator activator = new();
            activator.RegisterMessage<GetQuery, GetQueryResult>((request, token) =>
            {
                return Task.FromResult(RequestResult.Success(new GetQueryResult
                {
                    Name = request.Name,
                    Age = request.Age,
                    Tags = request.Tags
                }));
            });

            var descriptors = new HttpDescriptors();
            descriptors.AddFor<GetQuery>(b => b.CallRelativeUri("/get", HttpMethod.Get));

            _webAppFactory.SetServiceActivator(activator);
            _webAppFactory.SetHttpDescriptors(descriptors);

            // Act
            var url = "/get?name=John&age=42";
            var response = await _webAppFactory.CreateClient().GetAsync(url);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetQueryResult>(content, _jsonOptions);
            result.Should().BeEquivalentTo(new GetQueryResult
            {
                Name = "John",
                Age = 42,
                Tags = null
            });
        }

        [Fact]
        public async Task Get_binding_collections_and_nullables()
        {
            // Arrange
            SimpleServiceActivator activator = new();
            activator.RegisterMessage<GetQuery, GetQueryResult>((request, token) =>
            {
                return Task.FromResult(RequestResult.Success(new GetQueryResult
                {
                    Name = request.Name,
                    Age = request.Age,
                    Tags = request.Tags
                }));
            });

            var descriptors = new HttpDescriptors();
            descriptors.AddFor<GetQuery>(b => b.CallRelativeUri("/get", HttpMethod.Get));

            _webAppFactory.SetServiceActivator(activator);
            _webAppFactory.SetHttpDescriptors(descriptors);

            // Act
            var url = "/get?name=Jane&tags=dev&tags=api&age="; // age nullable, tags multiple
            var response = await _webAppFactory.CreateClient().GetAsync(url);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetQueryResult>(content, _jsonOptions);
            result.Should().BeEquivalentTo(new GetQueryResult
            {
                Name = "Jane",
                Age = null,
                Tags = new List<string> { "dev", "api" }
            });
        }
    }
}
