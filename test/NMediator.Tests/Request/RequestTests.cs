﻿using FluentAssertions;
using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NMediator.Core.Handling;

namespace NMediator.Tests.Request
{
    public class RequestTests
    {
        [Fact]
        public async Task Execute_must_be_executed_correctly()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            activator.RegisterMessage<MyRequest, string>((request, token) => Task.FromResult(RequestResult.Success("Hello World "+request.Name)));

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None);

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
        }

        [Fact]
        public async Task Execute_must_be_executed_correctly_with_generic_request()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            activator.RegisterMessage<MyGenericRequest<int>, string>((request, token) => Task.FromResult(RequestResult.Success("Hello World " + request.Data)));

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyGenericRequest<>)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyGenericRequest<int>, string>(new MyGenericRequest<int>() { Data = 15 }, CancellationToken.None);

            //ASSERT
            result.Data.Should().Be("Hello World 15");
        }

        [Fact]
        public async Task Execute_must_be_executed_correctly_with_multiple_request_definition()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            activator.RegisterMessage<MyRequest, string>((request, token) => Task.FromResult(RequestResult.Success("Hello World " + request.Name)));

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)))
                .Request(r => r.ExecuteWithInProcess(typeof(MyGenericRequest<>)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None);

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
        }

        [Fact]
        public async Task Execute_must_throw_InvalidOperationException_when_no_handler_registered()
        {
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task<RequestResult<string>>> action = () => processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, CancellationToken.None);

            //ASSERT
            await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"No handler found for message {typeof(MyRequest)}");
        }

        [Fact]
        public async Task Execute_must_throw_invalid_operation_if_no_route_find()
        {
            //ACT
            var config = new MediatorConfiguration();
            var activator = new SimpleServiceActivator();

            activator.RegisterMessage<MyGenericRequest<int>, string>((request, token) => Task.FromResult(RequestResult.Success("Hello World " + request.Data)));

            config.Handling(activator)
                .Request(r => r.ExecuteWithInProcess());

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            Func<Task<RequestResult<string>>> result = () => processor.Execute<MyGenericRequest<int>, string>(new MyGenericRequest<int>() { Data = 15 }, CancellationToken.None);

            //ASSERT
            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"No route find for message type {typeof(MyGenericRequest<int>)}");
        }

        class MyRequest
        {
            public string Name { get; set; }
        }

        class MyGenericRequest<T>
        {
            public T Data { get; set; }
        }
    }
}
