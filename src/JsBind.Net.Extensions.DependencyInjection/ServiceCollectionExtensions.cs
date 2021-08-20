using System;
using System.Linq;
using JsBind.Net;
using JsBind.Net.Configurations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsBind(this IServiceCollection services)
            => services.AddJsBind(null);

        public static IServiceCollection AddJsBind(this IServiceCollection services, Action<IJsBindOptionsConfigurator>? configAction)
        {
            var configurator = GetRegisteredConfigurator(services);
            if (configurator is null)
            {
                configurator = new JsBindOptionsConfigurator();
                services.AddTransient<IJsRuntimeAdapter, JsRuntimeAdapter>();
                services.AddSingleton(configurator);
                services.AddSingleton(configurator.Options);
            }
            configAction?.Invoke(configurator);
            return services;
        }

        private static JsBindOptionsConfigurator? GetRegisteredConfigurator(IServiceCollection services)
        {
            return services
                .FirstOrDefault(service => service.ServiceType == typeof(JsBindOptionsConfigurator))
                ?.ImplementationInstance as JsBindOptionsConfigurator;
        }
    }
}
