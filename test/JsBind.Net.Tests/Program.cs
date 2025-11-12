using JsBind.Net.Tests.Infrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace JsBind.Net.Tests;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services
            .AddTestBindings()
            .AddServerTestBindings()
            .AddScoped<ITestFactory, TestFactory>()
            .AddScoped<ITestRunner, TestRunner>();

        await builder.Build().RunAsync();
    }
}
