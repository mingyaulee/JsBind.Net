﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JsBind.Net.Tests.Infrastructure;
using TestBindings.Server;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Asynchronous (Server)")]
    public class TestAsynchronous
    {
        private readonly Window window;
        private readonly Document document;

        public TestAsynchronous(Window window, Document document)
        {
            this.window = window;
            this.document = document;
        }

        [Fact(Description = "Property should be initialized from JSON deserializer")]
        public async Task PropertyShouldBeInitializedFromJsonDeserializer()
        {
            // Act
            var innerWindow = await window.GetWindow();

            // Assert
            innerWindow.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value")]
        public async Task InvokeFunctionWithPrimitiveReturnValue()
        {
            // Act
            var result = await window.ParseInt("30");

            // Assert
            result.Should().Be(30);
        }

        [Fact(Description = "Invoke function with reference return value")]
        public async Task InvokeFunctionWithReferenceReturnValue()
        {
            // Act
            var result = await document.GetElementById("app");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("app");
        }

        [Fact(Description = "Get property on reference return value")]
        public async Task GetPropertyOnReferenceReturnValue()
        {
            // Arrange
            var element = await document.GetElementById("app");

            // Act
            var result = await element.GetTagName();

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value on reference return value")]
        public async Task InvokeFunctionWithPrimitiveReturnValueOnReferenceReturnValue()
        {
            // Arrange
            var element = await document.GetElementById("app");

            // Act
            var result = await element.GetAttribute("id");

            // Assert
            result.Should().Be("app");
        }

        [Fact(Description = "Invoke function with array like return value")]
        public async Task InvokeFunctionWithArrayLikeReturnValue()
        {
            // Act
            var results = await document.QuerySelectorAll("#app");

            // Assert
            results.Should().NotBeNull().And.HaveCount(1);
            results.Single().Should().NotBeNull();
            results.Single().Id.Should().Be("app");
        }

        [Fact(Description = "Get property on array like return value")]
        public async Task GetPropertyOnArrayLikeReturnValue()
        {
            // Arrange
            var element = (await document.QuerySelectorAll("#app")).Single();

            // Act
            var result = await element.GetTagName();

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact(Description = "Invoke function with primitive return value on array like return value")]
        public async Task InvokeFunctionWithPrimitiveReturnValueOnArrayLikeReturnValue()
        {
            // Arrange
            var element = (await document.QuerySelectorAll("#app")).Single();

            // Act
            var result = await element.GetAttribute("id");

            // Assert
            result.Should().Be("app");
        }

        [Fact(Description = "Set property value with primitive value")]
        public async Task SetPropertyValueWithPrimitiveValue()
        {
            // Arrange
            var variableName = "v_" + Guid.NewGuid().ToString().Substring(0, 8);
            var variableValue = 3000;

            // Act
            await window.SetVariableValue(variableName, variableValue);

            // Assert
            var actualValue = await window.GetVariableValue<int>(variableName);
            actualValue.Should().Be(variableValue);
        }

        [Fact(Description = "Set property value with reference value")]
        public async Task SetPropertyValueWithReferenceValue()
        {
            // Arrange
            var variableName = "v_" + Guid.NewGuid().ToString().Substring(0, 8);
            var variableValue = document;

            // Act
            await window.SetVariableValue(variableName, variableValue);

            // Assert
            var actualValue = await window.GetVariableValue<Document>(variableName);
            actualValue.Should().NotBeNull();
            (await actualValue.InstanceEqualsAsync(document)).Should().BeTrue();
        }

        [Fact(Description = "Convert reference value type")]
        public async Task ConvertReferenceValueType()
        {
            // Arrange
            var customPropertyValue = Guid.NewGuid().ToString();
            await window.SetVariableValue("customProperty", customPropertyValue);

            // Act
            var windowWithCustomProperty = await window.ToType<WindowWithCustomProperty>();

            // Assert
            windowWithCustomProperty.Should().NotBeNull();
            windowWithCustomProperty.CustomProperty.Should().Be(customPropertyValue);
        }

        [Fact(Description = "Convert reference value type with nested reference value")]
        public async Task ConvertReferenceValueTypeWithNestedReferenceValue()
        {
            // Act
            var windowWithLocation = await window.ToType<WindowWithLocation>();

            // Assert
            windowWithLocation.Should().NotBeNull();
            windowWithLocation.Location.Should().NotBeNull();
            windowWithLocation.Location.Href.Should().NotBeNull();
        }
    }
}
