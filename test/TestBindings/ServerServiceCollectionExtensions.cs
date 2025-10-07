using TestBindings.Server;
using TestBindings.Server.BindingTestLibrary;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class ServerServiceCollectionExtensions
    {
        public static IServiceCollection AddServerTestBindings(this IServiceCollection services)
            => services
                .AddJsBind()
                .AddTransient<Window>()
                .AddTransient<Document>()
                .AddTransient<BindingTestLibrary>();
    }
}
