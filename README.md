# NMediator

IN DEVELOPMENT

A .net mediator with these features: 
* A core system for create mediator simply (request processor, event publisher, command sender...).
* No coupling between message and mediator contract
* A transport level abstraction

## Quick start (DRAFT)

### Create Request and HandlerRequest
```csharp
public class MyRequest
{
	public string MyParameter {get;set;}
}

public class MyRequestHandler : IMessageHandler<MyRequest,string>
{
	public async Task<RequestResult<string>> Handle(MyRequest request, CancellationToken token)
	{
		return Task.FromResult("my result");
	}
}
```

### Configure NMediator
```csharp
MediatorConfiguration config = new();
Assembly handlerAssembly = typeof(MyRequestHandler).Assembly;

//Use ServiceCollection to get an instance of handler
ServiceCollection sc = new();
sc.AddSingleton<IMessageHandler<MyRequest, string>, MyRequestHandler>();
IServiceProvider sp = sc.BuildServiceProvider();

config
	.Request(r => r.ExecuteInProcess(typeof(MyRequest)))
	.Handling(h => h
		.ScanHandlersFromAssemblies(handlerAssembly)
		.UseDelegateActivator(t => sp.GetServices(t)));

BaseConfiguration.Configure(config);
```

### Use NMediatior
```c#
var requestExecutor = config.Container.Get<IRequestExecutor>();
var result = requestExecutor<MyRequest, string>(new MyRequest {MyParameter="MyValue"});
```

