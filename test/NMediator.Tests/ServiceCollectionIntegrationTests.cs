using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NMediator.Core.Handling;

namespace NMediator.Tests
{
    public class ServiceCollectionIntegrationTests
    {
        [Fact]
        public async Task UseServiceCollectionToGetHandlerInstance()
        {
            MediatorConfiguration config = new();

            ServiceCollection sc = new();
            sc.AddSingleton<IMessageHandler<string, string>>(new MyTestHandler());

            IServiceProvider sp = sc.BuildServiceProvider();

            config.Handling(h => h
                    .ScanHandlersFromAssemblies(Assembly.GetExecutingAssembly())
                    .UseDelegateActivator(t => sp.GetServices(t)))
                  .Request(r => r.ExecuteWithInProcess(typeof(string)));

            BaseConfiguration.Configure(config);

            var processor = config.Container.Get<IRequestExecutor>();
            var result = await processor.Execute<string, string>("my test", CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("test");
        }

        public class MyTestHandler : IMessageHandler<string, string>
        {
            public Task<RequestResult<string>> Handle(string message, CancellationToken cancellationToken)
            {
                return Task.FromResult(RequestResult.Success("test"));
            }
        }
    }
}
