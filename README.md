# JsBind.Net
[![Nuget](https://img.shields.io/nuget/v/JsBind.Net?style=for-the-badge&color=blue)](https://www.nuget.org/packages/JsBind.Net/)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/mingyaulee/JsBind.Net/JsBind.Net-Build.yml?branch=main&style=for-the-badge&color=blue)](https://github.com/mingyaulee/JsBind.Net/actions/workflows/JsBind.Net-Build.yml)
[![Sonar Tests](https://img.shields.io/sonar/coverage/JsBind.Net?server=https%3A%2F%2Fsonarcloud.io&style=for-the-badge)](https://sonarcloud.io/dashboard?id=JsBind.Net)
[![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/JsBind.Net?server=https%3A%2F%2Fsonarcloud.io&style=for-the-badge)](https://sonarcloud.io/dashboard?id=JsBind.Net)

A package for creating binding from .Net to JavaScript.

## How to use this package

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
1. Create a new class library project.
0. Install `JsBind.Net` from Nuget.
0. Add `PrivateAssets="contentfiles"` to the package reference in the `csproj` project file. For example:
   
   `<PackageReference Include="JsBind.Net" Version="x.x.x" PrivateAssets="contentfiles" />`
   > This is so that the projects consuming your binding library will also include the build assets in the `JsBind.Net` package.
0. Start creating bindings. You can refer to the section [How to create bindings](#how-to-create-bindings) below.

To create a dependency injection extension project:
1. Create a new class library project.
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

### Binding classes

You can start creating binding from the root object that needs to be bound, for example the storage API:
```csharp
public class LocalStorage : ObjectBindingBase
{
    public LocalStorage(IJsRuntimeAdapter jsRuntime)
    {
        SetAccessPath("localStorage");
        Initialize(jsRuntime);
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
        SetAccessPath("localStorage");
        Initialize(jsRuntime);
    }

    public ValueTask<string> GetItem(string key) => InvokeAsync<string>("getItem", key);
    public ValueTask<string> SetItem(string key, string value) => InvokeAsync<string>("setItem", key, value);
    public ValueTask<string> RemoveItem(string key) => InvokeAsync<string>("removeItem", key);
    public ValueTask Clear() => InvokeVoidAsync("clear");
}
```

The simplest way is to inherit from `ObjectBindingBase` class which offers the following APIs:

| API                                  | Description                                                                                           |
| ------------------------------------ | ----------------------------------------------------------------------------------------------------- |
| `SetAccessPath`                      | Sets the access path of the object relative to the `globalThis` variable.                             |
| `GetProperty`/`GetPropertyAsync`     | Gets a property value from the JavaScript object with the specified property name.                    |
| `SetProperty`/`SetPropertyAsync`     | Sets a property value to the JavaScript object with the specified property name.                      |
| `Invoke`/`InvokeAsync`               | Invoke a function matching the specified function name to the JavaScript object with return value.    |
| `InvokeVoid`/`InvokeVoidAsync`       | Invoke a function matching the specified function name to the JavaScript object without return value. |
| `ConvertToType`/`ConvertToTypeAsync` | Converts the current object to the specified type.                                                    |

#### Binding class constructor
If the binding class can be used directly to perform interop, meaning they can be accessed from the top level `globalThis` (e.g. globalThis.document/globalThis.window/globalThis.jQuery), the binding class needs a constructor that receives the `IJsRuntimeAdapter` to be able to interop to JavaScript. The constructor has to use the `SetAccessPath` API to set the path relative to the `globalThis` variable. Example of constructor:
```csharp
public LocalStorage(IJsRuntimeAdapter jsRuntime)
{
    SetAccessPath("localStorage");
    Initialize(jsRuntime);
}

public Window(IJsRuntimeAdapter jsRuntime)
{
    SetAccessPath("window");
    Initialize(jsRuntime);
}
```

If the binding class represents the structure of objects that can be returned from a JavaScript interop, it needs to have an parameterless constructor, or a constructor that can be deserialized (e.g. decorated with `JsonConstructor` attribute).
```
// Location class representing the object returned from window.location
public class Location
{
    // Initialized from JSON deserialization
    public Location()
    {
    }

    [JsonPropertyName("href")]
    public string Href { get; set; }
}
```

### Binding attributes
You should use the binding attributes to define the behaviour of the interop and serialization/deserialization.

Attributes from the `System.Text.Json` can be used for the serialization and deserialization behaviour.

Attributes that can be used for binding are

| Attribute                       | Usage    | Description                                                                                                |
| ------------------------------- | -------- | ---------------------------------------------------------------------------------------------------------- |
| BindDeclaredPropertiesAttribute | Class    | Include the public properties with setter that are declared in the class for binding.                      |
| BindAllPropertiesAttribute      | Class    | Include all properties from the JavaScript object for binding.                                             |
| BindIncludePropertiesAttribute  | Class    | Include the specified properties from the JavaScript object for binding.                                   |
| BindExcludePropertiesAttribute  | Class    | Include all properties except for the specified properties from the JavaScript object for binding.         |
| BindIgnoreAttribute             | Property | Ignore this property from binding (Only when the class is decorated with BindDeclaredPropertiesAttribute). |

Properties decorated with `JsonIgnoreAttribute` and `BindIgnoreAttribute` will be excluded from binding.

### Dynamic binding class
There are cases where you may want to create a binding dynamically, either from an existing ObjectBindingBase instance, or just simply from an access path.
For example, to achieve this in JavaScript:
```javascript
const newDiv = document.createElement("div");
document.body.append(newDiv);
```
The equivalent code would be:
```csharp
var document = Any.From("document", jsRuntime);
var newDiv = document.InvokeFunction<Any>("createElement", "div");
document["body"].InvokeFunctionVoid("append", newDiv);
```

### Object reference disposal
Objects returned from function invocation are stored as object references, and delegates passed in as parameter to function invocation are stored as delegate references.
The object references are stored in the JavaScript, whereas delegate references are stored in both JavaScript and DotNet.

If your library or the consuming project invokes a lot of functions, it will be good to dispose the object and delegate references.
If the object reference is an instance of the `BindingBase` and is the root of the object reference, either the `Dispose` or `DisposeAsync` method can be used to dispose it in JavaScript.
Otherwise, the `JsObjectManager` can be used with the following APIs:

| Method                                                         | Description                                                                                    |
| -------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| `DisposeObjectReference`/`DisposeObjectReferenceAsync`         | Disposes the object reference, if the object (can be enumerable) is the root object reference. |
| `DisposeRootObjectReference`/`DisposeRootObjectReferenceAsync` | Disposes the root object reference.                                                            |
| `DisposeDelegateReference`/`DisposeDelegateReferenceAsync`     | Disposes the delegate reference in both JavaScript and DotNet.                                 |
| `DisposeSession`/`DisposeSessionAsync`                         | Disposes all the references for the session.                                                   |

Any disposed object reference and delegate reference can no longer perform any operation such as `GetProperty` or `InvokeFunction`.

If the consuming project is a Blazor server, the references in the server needs to be cleared everytime a session ends.
In order to do this, you can create a component.
```csharp
public class SessionController : ComponentBase, IDisposable
{
    [Inject] public IJsRuntimeAdapter JsRuntime { get; set; }

    public void Dispose()
    {
        JsObjectManager.DisposeSession(JsRuntime);
    }
}
```
And in `App.razor`, add this component next to the `<Router>` component.
```razor
<Router AppAssembly="@typeof(Program).Assembly">
    // ...
</Router>
<SessionController />
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