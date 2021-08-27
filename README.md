# JsBind.Net
[![Nuget](https://img.shields.io/nuget/v/JsBind.Net?style=flat-square&color=blue)](https://www.nuget.org/packages/JsBind.Net/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mingyaulee/JsBind.Net/Build?style=flat-square&color=blue)](https://github.com/mingyaulee/JsBind.Net/actions/workflows/JsBind.Net-Build.yml)
[![Sonar Tests](https://img.shields.io/sonar/tests/JsBind.Net?compact_message&server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/dashboard?id=JsBind.Net)
[![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/JsBind.Net?server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/dashboard?id=JsBind.Net)

A package for creating binding from .Net to JavaScript.

## How to use this package

> **Important for v0.\*.\*:**<br />
> This package is still in pre-release stage so the versioning does not comply with semantic versioning. Feature and bug fix increments the patch version and breaking change increments the minor version. So be sure to check the release note before upgrading between minor version.

This package can be used in two ways.

### Create a binding library
This is the case when a binding library can be created to be consumed by other projects or published to NuGet.
You would want to do this if the binding is for common JS libraries (e.g. jQuery) or JavaScript API (e.g. File API and Storage API).

First thing that you will need to do is decide on what is the strategy that you would want to publish your library.

The best approach is to separate the library into multiple packages:
1. WebAssembly bindings
0. WebAssembly dependency injection extension
0. Server bindings
0. Server dependency injection extension

Of course this can also be separated into just 2 packages:
1. WebAssembly bindings (with or without dependency injection extension)
0. Server bindings (with or without dependency injection extension)

To get started with creating a WebAssembly or Server binding project:
1. Create a new class library project, targeting `.Net 5.0` or above.
0. Install `JsBind.Net` from Nuget.
0. Add `PrivateAssets="contentfiles"` to the package reference in the `csproj` project file. For example:
   
   `<PackageReference Include="JsBind.Net" Version="x.x.x" PrivateAssets="contentfiles" />`
   > This is so that the projects consuming your binding library will also include the build assets in the `JsBind.Net` package.
0. Start creating bindings. You can refer to the section [How to create bindings](#how-to-create-bindings) below.

To create a dependency injection extension project:
1. Create a new class library project, targeting `.Net 5.0` or above.
0. Install `JsBind.Net.Extensions.DependencyInjection` from Nuget.
0. Create the extension method for registering the binding services.

### Create bindings in a web project directly
When you want to interop to the JavaScript libraries in your project from .Net, you can simply create the bindings required and use it.

1. Install `JsBind.Net` (if you intend to use the binding without dependency injection) or `JsBind.Net.Extensions.DependencyInjection` from Nuget.
0. Create the bindings, you can refer to the section [How to create bindings](#how-to-create-bindings) below.
0. Register the bindings in the dependency container (if you are using dependency injection).
   ```csharp
   services.AddJsBind();
   // if you need this class to be injected to be used
   services.AddTransient<MyBindingClass>();
   ```
0. Add the script tag as shown in the section below to import the `JsBind.Net` JavaScript file.

## For projects using the binding
The projects using the binding from a binding library or from the project itself will need to include the JavaScript file from `JsBind.Net` with
```html
<script src="_content/JsBind.Net/JsBindNet.js"></script>
```

## How to create bindings
The [test binding project](test/TestBindings) showcases how to create bindings for:
1. [WebAssembly (synchronous)](test/TestBindings/WebAssembly)
0. [Server (asynchronous)](test/TestBindings/Server)
0. [WebAssembly dependency injection extension](test/TestBindings/WebAssemblyServiceCollectionExtensions.cs)
0. [Server dependency injection extension](test/TestBindings/ServerServiceCollectionExtensions.cs)

> For simplicity the test binding project is not separated into multiple projects as advised in the project separation strategy above.

You can start creating binding from the root object that needs to be bound, for example the storage API:
```csharp
public class LocalStorage : ObjectBindingBase
{
    public LocalStorage(IJsRuntimeAdapter jsRuntime)
    {
        Initialize(jsRuntime, "localStorage");
    }

    public string GetItem(string key) => Invoke<string>("getItem", key);
    public string SetItem(string key, string value) => Invoke<string>("setItem", key, value);
    public string RemoveItem(string key) => Invoke<string>("removeItem", key);
    public void Clear() => InvokeVoid("clear");
}
```

As for the asynchronous version of the binding, all the return type has to be wrapped in `ValueTask` and the `Async` version of `Invoke`, `InvokeVoid` and `GetProperty` has to be used instead.
```csharp
public class LocalStorage : ObjectBindingBase
{
    public LocalStorage(IJsRuntimeAdapter jsRuntime)
    {
        Initialize(jsRuntime, "localStorage");
    }

    public ValueTask<string> GetItem(string key) => InvokeAsync<string>("getItem", key);
    public ValueTask<string> SetItem(string key, string value) => InvokeAsync<string>("setItem", key, value);
    public ValueTask<string> RemoveItem(string key) => InvokeAsync<string>("removeItem", key);
    public ValueTask Clear() => InvokeVoidAsync("clear");
}
```

## Customize build

The following MSBuild properties can be specified in your project file or when running `dotnet build` command.

| Property             | Default value                | Description                                                                                   |
| -------------------- | ---------------------------- | --------------------------------------------------------------------------------------------- |
| IncludeJsBindAssets  | true                         | If set to false, the JavaScript files will not be added to the project.                       |
| LinkJsBindAssets     | false                        | If set to false, the JavaScript files are added as static web assets instead of linked files. |
| LinkJsBindAssetsPath | wwwroot\\_content\JsBind.Net | The root folder where the JavaScript files should be added as link.                           |

## Example projects
- [WebExtensions.Net](https://github.com/mingyaulee/WebExtensions.Net) - browser extensions API
