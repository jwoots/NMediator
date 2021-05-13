using FluentAssertions;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Context;
using NMediator.Core.Result;
using NMediator.Request;
using System.Collections.Generic;
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

            activator.RegisterMessage<MyRequest, string>(request => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
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

            activator.RegisterMessage<MyRequest, string>(request => { mc = MessageContext.Current; return Task.FromResult(RequestResult.Success("Hello World " + request.Name)); });

            config.WithActivator(activator)
                .Request(r => r.ExecuteWithInProcess(typeof(MyRequest)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<MyRequest, string>(new MyRequest() { Name = "jwoots" }, new Dictionary<string, string> { { "my-header", "my-value" } });

            //ASSERT
            result.Data.Should().Be("Hello World jwoots");
            mc.Values.Contains(new KeyValuePair<string, string>("my-header", "my-value"));
            mc.Values.ContainsKey(MessageContext.KEY_ID).Should().BeTrue();
            mc.Values.ContainsKey(MessageContext.KEY_DATE).Should().BeTrue();
        }

        class MyRequest : IRequest<string>
        {
            public string Name { get; set; }
        }
    }
}
