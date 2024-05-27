using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;

namespace NMediator.AspnetCore.Tests.FakeWebApp.Application
{
    public class HelloWorldQueryHandler : IMessageHandler<HelloWorldQuery, string>
    {
        public Task<RequestResult<string>> Handle(HelloWorldQuery message, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequestResult<string>("Hello World!"));
        }
    }

    public class HelloWorldQuery
    {
    }
}
