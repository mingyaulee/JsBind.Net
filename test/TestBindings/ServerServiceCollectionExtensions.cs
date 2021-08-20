using TestBindings.Server;
using TestBindings.Server.BindingTestLibrary;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServerServiceCollectionExtensions
    {
        public static IServiceCollection AddServerTestBindings(this IServiceCollection services)
        {
            services.AddJsBind();
            services.AddTransient<Window>();
            services.AddTransient<Document>();
            services.AddTransient<BindingTestLibrary>();

            return services;
        }
    }
}
