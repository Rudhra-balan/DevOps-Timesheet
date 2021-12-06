using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.Infrastructure.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HI.DevOps.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddSingleton<IRequestBrokerService>(c =>
                new RequestBroker(configuration.GetValue<string>("Services:BaseUrl")));
            services.AddSingleton<IDevOpsRequestBroker>(c =>
                new DevOpsRequestBroker(configuration.GetValue<string>("Services:BaseDevOpsUrl")));

            return services;
        }
    }
}