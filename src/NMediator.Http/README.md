# Quick Start

NMediator.Http is an extension library for NMediator that provides HTTP client capabilities. It allows you to send HTTP requests and receive responses using a mediator pattern.

## Create descriptor

```csharp
HttpDescriptors descriptors = new();
descriptors.AddFor<YourRequest>(sb => {
    sb.CallRelativeUri("/api/your-endpoint",HttpMethos.POST);
});
descriptors.AddFor<AnotherRequest>(sb => {
    sb.CallRelativeUri("/api/another-endpoint",HttpMethos.GET);
});
```

With this descriptor, when you send a `YourRequest` through the mediator, it will make an HTTP POST request to the specified relative URI.
`YourRequest` will be serialized to JSON and included in the request body.

When you send an `AnotherRequest`, it will make an HTTP GET request to the specified relative URI.
The content of `AnotherRequest` will be serialized to query string parameters.

## Configure
```csharp
MediatorConfiguration config = new();
Assembly handlerAssembly = typeof(MyRequestHandler).Assembly;

//Use ServiceCollection to get an instance of handler
ServiceCollection sc = new();
sc.AddHttpClient();
sc.AddSingleton<IMessageHandler<MyRequest, string>, MyRequestHandler>();
IServiceProvider sp = sc.BuildServiceProvider();

IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
config.Request(r => r.ExecuteWithHttp(options =>
{
    options.HttpClientFactory = () => httpClientFactory.CreateClient();
    options.HttpDescriptors = descriptors;
    options.BaseUri = new Uri("http://api-to-request");
}));

BaseConfiguration.Configure(config);
```

## Use it

```csharp
var requestExecutor = config.Container.Get<IRequestExecutor>();
var result = await requestExecutor.Execute<YourRequest, YourRequestResult>(new YourRequest {...});
```

By default, 
* result is success if HTTP status code is 2xx.
* result is failure if HTTP status code is 4xx. The error message contains status code and reason phrase. 
* an exception is thrown if HTTP status code is 5xx.

## Customize Error handling

You can customize error handling by configuring descriptors.
```csharp
//add a custom error factory for 400 Bad Request with fixed response structure
...
public class BadRequestDetail
{
    public class FieldError
    {
        public string Field {get;set;}
        public string Message {get;set;}
    }
    public IEnumerable<FieldError> Errors {get;set;}
}
...
descriptors.ErrorMapper.AddErrorFactory(HttpStatusCode.BadRequest, async (response) => {
    return new Error{
        Code="BadRequest", 
        Description="The request was invalid.",
        Detail = JsonSerializer.Deserialize<FieldError>(response.Content.ReadAsStringAsync());
});
```



