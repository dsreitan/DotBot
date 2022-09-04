using Core.Data;
using Core.Intel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core;

public static class ServiceInstaller
{
    public static IHostBuilder AddCoreServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IIntelService, IntelService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMicroService, MicroService>();
            services.AddScoped<IUnitService, UnitService>();
        });
    }
}