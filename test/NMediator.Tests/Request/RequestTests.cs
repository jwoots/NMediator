﻿using FluentAssertions;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Request
{
    public class RequestTests
    {
        [Fact]
        public async Task Request_must_be_executed_correctly()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();

            activator.RegisterMessage<MyRequest, string>(request => Task.FromResult(RequestResult.Success("Hello World "+request.Name)));

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
        }

        [Fact]
        public async Task Execute_must_throw_InvalidOperationException_when_no_handler_registered()
        {
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task<RequestResult<string>>> action = () => processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" });

            //ASSERT
            await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"No handler instance found for type {typeof(IMessageHandler<MyRequest,string>)}");
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}
