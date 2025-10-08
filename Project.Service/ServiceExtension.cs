using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core;
using Project.Service.Common;
using Project.Service.Services.AD;
using System.Reflection;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Caching.Distributed;

namespace Project.Service
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { cfg.AddExpressionMapping(); }, typeof(MappingProfile).Assembly);
           
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Connection")));
            
            var allProviderTypes = Assembly.GetAssembly(typeof(IMessageService))
             .GetTypes().Where(t => t.Namespace != null).ToList();
            foreach (var intfc in allProviderTypes.Where(t => t.IsInterface))
            {
                var impl = allProviderTypes.FirstOrDefault(c => c.IsClass && !c.IsAbstract && intfc.Name[1..] == c.Name);
                if (impl != null) services.AddScoped(intfc, impl);
            }

            // Explicit registrations for cache service
            services.AddScoped<Project.Service.Services.AD.IMessageCacheService, Project.Service.Services.AD.MessageCacheService>();

            return services;
        }
    }
}
