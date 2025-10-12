using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NMediator.Core.Handling;
using NMediator.NMediator.Http.Reflection;

namespace NMediator.AspnetCore.Tests
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        public List<Action<IServiceCollection>> ServicesList { get; set; } = new List<Action<IServiceCollection>>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ServicesList.ForEach(service => service(services));
            });

            base.ConfigureWebHost(builder);
        }

        public void SetHttpDescriptors(HttpDescriptors descriptors)
        {
            ServicesList.Add(sc => sc.AddSingleton(descriptors));
        }

        public void SetErrorToHttpResponseMapper(ErrorToHttpResponseMapper mapper)
        {
            ServicesList.Add(sc => sc.AddSingleton(mapper));
        }

        public void SetServiceActivator(SimpleServiceActivator activator)
        {
            ServicesList.Add(sc => sc.AddSingleton(activator));
        }
    }
}
