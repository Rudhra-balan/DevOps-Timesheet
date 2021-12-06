using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace HI.DevOps.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}