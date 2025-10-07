using TestBindings.WebAssembly;
using TestBindings.WebAssembly.BindingTestLibrary;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class WebAssemblyServiceCollectionExtensions
    {
        public static IServiceCollection AddTestBindings(this IServiceCollection services)
            => services
                .AddJsBind(options => options.UseInProcessJsRuntime())
                .AddTransient<Window>()
                .AddTransient<Document>()
                .AddTransient<BindingTestLibrary>();
    }
}
