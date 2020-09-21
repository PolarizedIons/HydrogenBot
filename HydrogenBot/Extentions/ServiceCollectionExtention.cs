using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HydrogenBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HydrogenBot.Extentions
{
    public static class ServiceProvider
    {
        public static IServiceCollection DiscoverAndMakeDiServicesAvailable(this IServiceCollection services)
        {
            var discoveredTypes = GetTypesInAssembly(typeof(IDiService));
            foreach (var serviceType in discoveredTypes)
            {
                if (typeof(IScopedDiService).IsAssignableFrom(serviceType))
                {
                    services.AddScoped(serviceType);
                }
                else if (typeof(ISingletonDiService).IsAssignableFrom(serviceType))
                {
                    services.AddSingleton(serviceType);
                }
                else
                {
                    throw new InvalidConstraintException("Unknown type of DI Service found! " + serviceType); 
                }
            }

            return services;
        }

        private static IEnumerable<Type> GetTypesInAssembly(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }
    }
}