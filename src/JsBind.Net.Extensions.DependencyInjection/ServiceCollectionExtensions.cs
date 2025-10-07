using System;
using System.Linq;
using JsBind.Net;
using JsBind.Net.Configurations;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension method for service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required services for using JsBind.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddJsBind(this IServiceCollection services)
        => services.AddJsBind(null);

    /// <summary>
    /// Adds the required services for using JsBind.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <param name="configAction">The action to configure JsBind.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddJsBind(this IServiceCollection services, Action<IJsBindOptionsConfigurator>? configAction)
    {
        var configurator = GetRegisteredConfigurator(services);
        if (configurator is null)
        {
            configurator = new JsBindOptionsConfigurator();
            services
                .AddTransient<IJsRuntimeAdapter, JsRuntimeAdapter>()
                .AddSingleton(configurator)
                .AddSingleton(configurator.Options);
        }
        configAction?.Invoke(configurator);
        return services;
    }

    private static JsBindOptionsConfigurator? GetRegisteredConfigurator(IServiceCollection services)
        => services
            .FirstOrDefault(service => service.ServiceType == typeof(JsBindOptionsConfigurator))
            ?.ImplementationInstance as JsBindOptionsConfigurator;
}
