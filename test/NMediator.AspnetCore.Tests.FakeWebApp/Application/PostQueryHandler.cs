using NMediator.Core.Message;
using NMediator.Core.Result;
using NMediator.Request;

namespace NMediator.AspnetCore.Tests.FakeWebApp.Application;

public class PostQueryHandler : IMessageHandler<PostQuery, PostQueryResult>
{
    public Task<RequestResult<PostQueryResult>> Handle(PostQuery message, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RequestResult<PostQueryResult>(new PostQueryResult
        {
            Name = message.Name,
            Adress = message.Adress
        }));
    }
}

public class PostQueryResult
{
    public string Name { get; set; }
    public string Adress { get; set; }
}

public class PostQuery
{
    public string Name { get; set; }
    public string Adress { get; set; }
}


