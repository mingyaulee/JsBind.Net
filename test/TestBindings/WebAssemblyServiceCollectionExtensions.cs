using TestBindings.WebAssembly;
using TestBindings.WebAssembly.BindingTestLibrary;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebAssemblyServiceCollectionExtensions
    {
        public static IServiceCollection AddTestBindings(this IServiceCollection services)
        {
            services.AddJsBind(options => options.UseInProcessJsRuntime());
            services.AddTransient<Window>();
            services.AddTransient<Document>();
            services.AddTransient<BindingTestLibrary>();

            return services;
        }
    }
}
