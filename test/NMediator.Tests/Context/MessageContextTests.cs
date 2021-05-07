﻿using FluentAssertions;
using NMediator.Activator;
using NMediator.Configuration;
using NMediator.Context;
using NMediator.Request;
using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Context
{
    public class MessageContextTests
    {
        [Fact]
        public async Task Execute_should_forward_headers()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();
            MessageContext mc = null;

            activator.RegisterRequest<MyRequest, string>(request => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestProcessor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, new Dictionary<string,string>{ { "my-header","my-value"} });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
            mc.Values.Contains(new KeyValuePair<string, string>("my-header", "my-value"));
        }

        [Fact]
        public async Task Execute_should_forward_headers_with_default_guid_and_date()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();
            MessageContext mc = null;

            activator.RegisterRequest<MyRequest, string>(request => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestProcessor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, new Dictionary<string, string> { { "my-header", "my-value" } });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
            mc.Values.Contains(new KeyValuePair<string, string>("my-header", "my-value"));
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}