using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JsBind.Net.Tests.Infrastructure;
using TestBindings.Server;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Dynamic Object Asynchronous (Server)")]
    public class DynamicObjectTestAsynchronous
    {
        private readonly Window window;
        private readonly Document document;

        public DynamicObjectTestAsynchronous(Window window, Document document)
        {
            this.window = window;
            this.document = document;
        }

        [Fact(Description = "Primitive property get value")]
        public async Task PrimitivePropertyGetValue()
        {
            // Arrange
            var originProperty = Any.From(window)["origin"];

            // Act
            var origin = await originProperty.GetValueAsync<string>();

            // Assert
            origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Reference property get value")]
        public async Task ReferencePropertyGetValue()
        {
            // Arrange
            var innerWindowProperty = Any.From(window)["window"];

            // Act
            var innerWindow = await innerWindowProperty.GetValueAsync<Window>();

            // Assert
            innerWindow.Should().NotBeNull();
            innerWindow.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Get primitive property value")]
        public async Task GetPrimitivePropertyValue()
        {
            // Arrange
            var dynamicTypeWindow = Any.From(window);

            // Act
            var origin = await dynamicTypeWindow.GetPropertyValueAsync<string>("origin");

            // Assert
            origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Get reference property value")]
        public async Task GetReferencePropertyValue()
        {
            // Arrange
            var dynamicTypeWindow = Any.From(window);

            // Act
            var innerWindow = await dynamicTypeWindow.GetPropertyValueAsync<Window>("window");

            // Assert
            innerWindow.Should().NotBeNull();
            innerWindow.Origin.Should().NotBeNullOrEmpty();
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
            actualValue.Should().Be(testValue);
        }

        [Fact(Description = "Invoke function with primitive return value")]
        public async Task InvokeFunctionWithPrimitiveReturnValue()
        {
            // Arrange
            var dynamicTypeWindow = Any.From(window);

            // Act
            var result = await dynamicTypeWindow.InvokeFunctionAsync<int>("parseInt", "30");

            // Assert
            result.Should().Be(30);
        }

        [Fact(Description = "Invoke function with reference return value")]
        public async Task InvokeFunctionWithReferenceReturnValue()
        {
            // Arrange
            var dynamicTypeDocument = Any.From(document);

            // Act
            var result = await dynamicTypeDocument.InvokeFunctionAsync<Element>("getElementById", "app");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("app");
        }

        [Fact(Description = "Invoke function with array like return value")]
        public async Task InvokeFunctionWithArrayLikeReturnValue()
        {
            // Arrange
            var dynamicTypeDocument = Any.From(document);

            // Act
            var results = await dynamicTypeDocument.InvokeFunctionAsync<IEnumerable<Element>>("querySelectorAll", "#app");

            // Assert
            results.Should().NotBeNull().And.HaveCount(1);
            results.Single().Should().NotBeNull();
            results.Single().Id.Should().Be("app");
        }
    }
}
