using JsBind.Net.Tests.Infrastructure;
using TestBindings.Server;

namespace JsBind.Net.Tests.Tests;

[TestClass(Description = "Dynamic Object Asynchronous (Server)")]
public class DynamicObjectTestAsynchronous(Window window, Document document)
{
    private readonly Window window = window;
    private readonly Document document = document;

    [Fact(Description = "Primitive property get value")]
    public async Task PrimitivePropertyGetValue()
    {
        // Arrange
        var originProperty = Any.From(window)["origin"];

        // Act
        var origin = await originProperty.GetValueAsync<string>();

        // Assert
        origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Reference property get value")]
    public async Task ReferencePropertyGetValue()
    {
        // Arrange
        var innerWindowProperty = Any.From(window)["window"];

        // Act
        var innerWindow = await innerWindowProperty.GetValueAsync<Window>();

        // Assert
        innerWindow.ShouldNotBeNull();
        innerWindow.Origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Get primitive property value")]
    public async Task GetPrimitivePropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var origin = await dynamicTypeWindow.GetPropertyValueAsync<string>("origin");

        // Assert
        origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Get reference property value")]
    public async Task GetReferencePropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var innerWindow = await dynamicTypeWindow.GetPropertyValueAsync<Window>("window");

        // Assert
        innerWindow.ShouldNotBeNull();
        innerWindow.Origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Set property value")]
    public async Task SetPropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);
        var testValue = Guid.NewGuid().ToString();

        // Act
        await dynamicTypeWindow.SetPropertyValueAsync("testProperty", testValue);
        var actualValue = await dynamicTypeWindow.GetPropertyValueAsync<string>("testProperty");

        // Assert
        actualValue.ShouldBe(testValue);
    }

    [Fact(Description = "Invoke function with primitive return value")]
    public async Task InvokeFunctionWithPrimitiveReturnValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var result = await dynamicTypeWindow.InvokeFunctionAsync<int>("parseInt", "30");

        // Assert
        result.ShouldBe(30);
    }

    [Fact(Description = "Invoke function with reference return value")]
    public async Task InvokeFunctionWithReferenceReturnValue()
    {
        // Arrange
        var dynamicTypeDocument = Any.From(document);

        // Act
        var result = await dynamicTypeDocument.InvokeFunctionAsync<Element>("getElementById", "app");

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("app");
    }

    [Fact(Description = "Invoke function with array like return value")]
    public async Task InvokeFunctionWithArrayLikeReturnValue()
    {
        // Arrange
        var dynamicTypeDocument = Any.From(document);

        // Act
        var results = await dynamicTypeDocument.InvokeFunctionAsync<IEnumerable<Element>>("querySelectorAll", "#app");

        // Assert
        results.ShouldNotBeNull();
        results.Count().ShouldBe(1);
        results.Single().ShouldNotBeNull();
        results.Single().Id.ShouldBe("app");
    }
}
