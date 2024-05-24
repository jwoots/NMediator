
using NMediator.AspnetCore.Tests.FakeWebApp.Application;
using NMediator.Core.Configuration;
using NMediator.Core.Message;
using NMediator.NMediator.Http.Reflection;
using NMediator.Request;

namespace NMediator.AspnetCore.Tests;

public  class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IMessageHandler<HelloWorldQuery, string>, HelloWorldQueryHandler>();

        var app = builder.Build();

        var mediatorConfig = new MediatorConfiguration();
        mediatorConfig.Handling(h => h
        .ScanHandlersFromAssemblies(typeof(HelloWorldQueryHandler).Assembly)
        .UseDelegateActivator(t => app.Services.GetServices(t)));

        HttpDescriptors descriptors = new HttpDescriptors();
        ErrorToHttpResponseMapper errorMapper = new ErrorToHttpResponseMapper();
        descriptors.AddFor<HelloWorldQuery>(b => b.CallRelativeUri("/hello", HttpMethod.Get, ParameterLocation.QUERY_STRING));

        app.MapMessages(mediatorConfig.Container, descriptors, errorMapper);

        BaseConfiguration.Configure(mediatorConfig);

        app.Run();
    }
}
