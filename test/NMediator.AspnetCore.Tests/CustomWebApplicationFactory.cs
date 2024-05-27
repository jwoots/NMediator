using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
