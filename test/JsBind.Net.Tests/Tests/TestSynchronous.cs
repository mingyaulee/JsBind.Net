using System.Linq;
using FluentAssertions;
using JsBind.Net.Tests.Infrastructure;
using TestBindings.WebAssembly;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Synchronous (WebAssembly)")]
    public class TestSynchronous
    {
        private readonly Window window;
        private readonly Document document;

        public TestSynchronous(Window window, Document document)
        {
            this.window = window;
            this.document = document;
        }

        [Fact(Description = "Property should be initialized")]
        public void PropertyShouldBeInitialized()
        {
            // Assert
            window.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Property should be initialized from JSON deserializer")]
        public void PropertyShouldBeInitializedFromJsonDeserializer()
        {
            // Act
            var innerWindow = window.window;

            // Assert
            innerWindow.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value")]
        public void InvokeFunctionWithPrimitiveReturnValue()
        {
            // Act
            var result = window.ParseInt("30");

            // Assert
            result.Should().Be(30);
        }

        [Fact(Description = "Invoke function with reference return value")]
        public void InvokeFunctionWithReferenceReturnValue()
        {
            // Act
            var result = document.GetElementById("app");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("app");
        }

        [Fact(Description = "Get property on reference return value")]
        public void GetPropertyOnReferenceReturnValue()
        {
            // Arrange
            var element = document.GetElementById("app");

            // Act
            var result = element.TagName;

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value on reference return value")]
        public void InvokeFunctionWithPrimitiveReturnValueOnReferenceReturnValue()
        {
            // Arrange
            var element = document.GetElementById("app");

            // Act
            var result = element.GetAttribute("id");

            // Assert
            result.Should().Be("app");
        }

        [Fact(Description = "Invoke function with array like return value")]
        public void InvokeFunctionWithArrayLikeReturnValue()
        {
            // Act
            var results = document.QuerySelectorAll("#app");

            // Assert
            results.Should().NotBeNull().And.HaveCount(1);
            results.Single().Should().NotBeNull();
            results.Single().Id.Should().Be("app");
        }

        [Fact(Description = "Get property on array like return value")]
        public void GetPropertyOnArrayLikeReturnValue()
        {
            // Arrange
            var element = document.QuerySelectorAll("#app").Single();

            // Act
            var result = element.TagName;

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value on array like return value")]
        public void InvokeFunctionWithPrimitiveReturnValueOnArrayLikeReturnValue()
        {
            // Arrange
            var element = document.QuerySelectorAll("#app").Single();

            // Act
            var result = element.GetAttribute("id");

            // Assert
            result.Should().Be("app");
        }
    }
}
