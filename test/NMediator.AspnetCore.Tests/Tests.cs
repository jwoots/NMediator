using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NMediator.AspnetCore.Tests
{
    public class Tests
    {
        private readonly WebApplicationFactory<Program> _factory;
        public Tests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task Test1()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/hello");
            var content = await response.Content.ReadAsStringAsync();

            JsonSerializer.Deserialize<string>(content).Should().Be("Hello World!");
        }
    }
}
