using JsBind.Net.Tests.Infrastructure;
using TestBindings.WebAssembly;

namespace JsBind.Net.Tests.Tests;

[TestClass(Description = "Dynamic Object Synchronous (WebAssembly)")]
public class DynamicObjectTestSynchronous(Window window, Document document)
{
    private readonly Window window = window;
    private readonly Document document = document;

    [Fact(Description = "Primitive property get value")]
    public void PrimitivePropertyGetValue()
    {
        // Arrange
        var originProperty = Any.From(window)["origin"];

        // Act
        var origin = originProperty.GetValue<string>();

        // Assert
        origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Reference property get value")]
    public void ReferencePropertyGetValue()
    {
        // Arrange
        var innerWindowProperty = Any.From(window)["window"];

        // Act
        var innerWindow = innerWindowProperty.GetValue<Window>();

        // Assert
        innerWindow.ShouldNotBeNull();
        innerWindow.Origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Get primitive property value")]
    public void GetPrimitivePropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var origin = dynamicTypeWindow.GetPropertyValue<string>("origin");

        // Assert
        origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Get reference property value")]
    public void GetReferencePropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var innerWindow = dynamicTypeWindow.GetPropertyValue<Window>("window");

        // Assert
        innerWindow.ShouldNotBeNull();
        innerWindow.Origin.ShouldNotBeNullOrEmpty();
    }

    [Fact(Description = "Set property value")]
    public void SetPropertyValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);
        var testValue = Guid.NewGuid().ToString();

        // Act
        dynamicTypeWindow.SetPropertyValue("testProperty", testValue);
        var actualValue = dynamicTypeWindow.GetPropertyValue<string>("testProperty");

        // Assert
        actualValue.ShouldBe(testValue);
    }

    [Fact(Description = "Invoke function with primitive return value")]
    public void InvokeFunctionWithPrimitiveReturnValue()
    {
        // Arrange
        var dynamicTypeWindow = Any.From(window);

        // Act
        var result = dynamicTypeWindow.InvokeFunction<int>("parseInt", "30");

        // Assert
        result.ShouldBe(30);
    }

    [Fact(Description = "Invoke function with reference return value")]
    public void InvokeFunctionWithReferenceReturnValue()
    {
        // Arrange
        var dynamicTypeDocument = Any.From(document);

        // Act
        var result = dynamicTypeDocument.InvokeFunction<Element>("getElementById", "app");

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("app");
    }

    [Fact(Description = "Invoke function with array like return value")]
    public void InvokeFunctionWithArrayLikeReturnValue()
    {
        // Arrange
        var dynamicTypeDocument = Any.From(document);

        // Act
        var results = dynamicTypeDocument.InvokeFunction<IEnumerable<Element>>("querySelectorAll", "#app");

        // Assert
        results.ShouldNotBeNull();
        results.Count().ShouldBe(1);
        results.Single().ShouldNotBeNull();
        results.Single().Id.ShouldBe("app");
    }
}
