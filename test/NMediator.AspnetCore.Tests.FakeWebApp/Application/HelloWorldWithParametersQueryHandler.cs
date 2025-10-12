using NMediator.Core.Message;
using NMediator.Core.Result;

namespace NMediator.AspnetCore.Tests.FakeWebApp.Application
{
    public class HelloWorldWithParametersQueryHandler : IMessageHandler<HelloWorldWithParametersQuery, string>
    {
        public Task<RequestResult<string>> Handle(HelloWorldWithParametersQuery message, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequestResult<string>($"Hello World! {message.Id} {message.Name}"));
        }
    }

    public class HelloWorldWithParametersQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

}
