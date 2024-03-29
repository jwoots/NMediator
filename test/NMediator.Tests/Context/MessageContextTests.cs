﻿using FluentAssertions;
using NMediator.Core.Configuration;
using NMediator.Core.Context;
using NMediator.Core.Result;
using NMediator.Request;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NMediator.Core.Handling;

namespace NMediator.Tests.Context
{
    public class MessageContextTests
    {
        [Fact]
        public async Task Execute_should_forward_headers()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();
            MessageContext mc = null;

            activator.RegisterMessage<MyRequest, string>((request, token) => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None, new Dictionary<string,string>{ { "my-header","my-value"} });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
            mc.Values.Contains(new KeyValuePair<string, string>("my-header", "my-value"));
        }

        [Fact]
        public async Task Execute_should_forward_headers_with_default_guid_and_date()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();
            MessageContext mc = null;

            activator.RegisterMessage<MyRequest, string>((request, token) => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None, new Dictionary<string, string> { { "my-header", "my-value" } });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
            mc.Values.Contains(new KeyValuePair<string, string>("my-header", "my-value"));
            mc.Values.ContainsKey(Headers.DATE).Should().BeTrue();
            mc.Values.ContainsKey(Headers.ID).Should().BeTrue();
        }

        class MyRequest
        {
            public string Name { get; set; }
        }
    }
}
