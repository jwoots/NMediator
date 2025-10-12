
using NMediator.AspnetCore.Tests.FakeWebApp.Application;
using NMediator.Core.Configuration;
using NMediator.Core.Handling;
using NMediator.Core.Message;
using NMediator.NMediator.Http.Reflection;

namespace NMediator.AspnetCore.Tests;

public  class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Scan(scan => scan
            .FromAssemblyOf<HelloWorldQueryHandler>()
            .AddClasses(c => c.AssignableTo(typeof(IMessageHandler<,>)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        var app = builder.Build();

        HttpDescriptors descriptors = app.Services.GetRequiredService<HttpDescriptors>();
        ErrorToHttpResponseMapper? errorMapper = app.Services.GetService<ErrorToHttpResponseMapper>();
        SimpleServiceActivator? activator = app.Services.GetService<SimpleServiceActivator>();

        var mediatorConfig = new MediatorConfiguration();
        if (activator != null)
        {
            mediatorConfig.Handling(activator);
        }
        else
        {
            mediatorConfig.Handling(h => h
                .ScanHandlersFromAssemblies(typeof(HelloWorldQueryHandler).Assembly)
                .UseDelegateActivator(t => app.Services.GetServices(t)));
        }



        app.MapMessages(mediatorConfig.Container, descriptors, errorMapper);

        BaseConfiguration.Configure(mediatorConfig);

        app.Run();
    }
}
