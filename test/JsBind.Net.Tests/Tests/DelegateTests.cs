using JsBind.Net.Tests.Infrastructure;
using TestBindings.WebAssembly.BindingTestLibrary;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Delegate Tests")]
    public class DelegateTests(BindingTestLibrary bindingTestLibrary)
    {
        private readonly BindingTestLibrary bindingTestLibrary = bindingTestLibrary;

        [Fact(Description = "Async ValueTask delegate reference can be invoked from JS")]
        public async Task AsyncValueTaskDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<ValueTask> testDelegateAsync = () =>
            {
                currentInvocationCount++;
                return ValueTask.CompletedTask;
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            await bindingTestLibrary.TestInvokeDelegateAsync(testDelegateAsync);

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "Async Task delegate reference can be invoked from JS")]
        public async Task AsyncTaskDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Task> testDelegateAsync = () =>
            {
                currentInvocationCount++;
                return Task.CompletedTask;
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            await bindingTestLibrary.TestInvokeDelegateAsync(testDelegateAsync);

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "Async ValueTask with result delegate reference can be invoked from JS")]
        public async Task AsyncValueTaskWithResultDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<ValueTask<int>> testDelegateAsync = () =>
            {
                currentInvocationCount++;
                return ValueTask.FromResult(currentInvocationCount);
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            var result = await bindingTestLibrary.TestInvokeDelegateAsync<int>(testDelegateAsync);

            // Assert
            currentInvocationCount.ShouldBe(1);
            result.ShouldBe(1);
        }

        [Fact(Description = "Async Task with result delegate reference can be invoked from JS")]
        public async Task AsyncTaskWithResultDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Task<int>> testDelegateAsync = () =>
            {
                currentInvocationCount++;
                return Task.FromResult(currentInvocationCount);
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            var result = await bindingTestLibrary.TestInvokeDelegateAsync<int>(testDelegateAsync);

            // Assert
            currentInvocationCount.ShouldBe(1);
            result.ShouldBe(1);
        }

        [Fact(Description = "Delegate reference with exception can be invoked from JS")]
        public void DelegateReferenceWithExceptionCanBeInvoked()
        {
            // Arrange
            Action testDelegate = () => ThrowExceptionInTest("A test exception");

            // Act
            Action action = () => bindingTestLibrary.TestInvokeDelegate(testDelegate);

            // Assert
            action.ShouldThrow<JsBindException>()
                .ShouldSatisfyAllConditions(
                    exception => exception.Message.ShouldBe("A test exception"),
                    exception => exception.StackTrace.ShouldContain(nameof(ThrowExceptionInTest))
                );
        }

        [Fact(Description = "Async delegate reference with exception can be invoked from JS")]
        public async Task AsyncDelegateReferenceWithExceptionCanBeInvoked()
        {
            // Arrange
            Func<ValueTask> testDelegateAsync = () =>
            {
                ThrowExceptionInTest("A test exception");
                return ValueTask.CompletedTask;
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            Func<Task> action = async () => await bindingTestLibrary.TestInvokeDelegateAsync(testDelegateAsync);

            // Assert
            (await action.ShouldThrowAsync<JsBindException>())
                .ShouldSatisfyAllConditions(
                    exception => exception.Message.ShouldBe("A test exception"),
                    exception => exception.StackTrace.ShouldContain(nameof(ThrowExceptionInTest))
                );
        }

        [Fact(Description = "Delegate reference can be invoked with missing parameters from JS")]
        public void DelegateReferenceCanBeInvokedWithMissingParameters()
        {
            // Arrange
            Func<int, string, BindingTestLibrary, string> testDelegate = (a, b, c)
                => $"{a},{(b is null ? "null" : "notnull")},{(c is null ? "null" : "notnull")}";

            // Act
            var result = bindingTestLibrary.TestInvokeDelegate<string>(testDelegate);

            // Assert
            result.ShouldBe("0,null,null");
        }

        [Fact(Description = "Delegate reference can be invoked with partially missing parameters from JS")]
        public void DelegateReferenceCanBeInvokedWithPartiallyMissingParameters()
        {
            // Arrange
            Func<int, string, BindingTestLibrary, string> testDelegate = (a, b, c)
                => $"{a},{(b is null ? "null" : "notnull")},{(c is null ? "null" : "notnull")}";

            // Act
            var result = bindingTestLibrary.TestInvokeDelegateWithParams<string>(testDelegate, 100, "test");

            // Assert
            result.ShouldBe("100,notnull,null");
        }

        private static void ThrowExceptionInTest(string message) => throw new InvalidOperationException(message);
    }
}
