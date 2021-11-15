using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsBind.Net.BindingConfigurations;
using JsBind.Net.InvokeOptions;

namespace JsBind.Net.Internal.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if the type is iterable, either array type or type inheriting from generic <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is iterable.</returns>
        public static bool IsIterableType(this Type type)
        {
            return type.IsArray || (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        /// <summary>
        /// Gets the iterable item type from either array type or type inheriting from generic <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The iterable item type.</returns>
        public static Type? GetIterableItemType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            return type.GetGenericArguments().FirstOrDefault();
        }

        /// <summary>
        /// Checks if the type is primitive, including <see href="https://docs.microsoft.com/dotnet/api/system.type.isprimitive">system primitive types</see>, string and Guid.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is primitive.</returns>
        public static bool IsPrimitiveType(this Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(Guid);
        }

        /// <summary>
        /// Checks if the type is either a <see cref="Task" /> or <see cref="ValueTask" />, or their generic version.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is awaitable.</returns>
        public static bool IsTaskOrValueTask(this Type type)
        {
            // Checking for inheritance from Task is also checking for inheritance from Task<>
            if (typeof(Task).IsAssignableFrom(type) || typeof(ValueTask).IsAssignableFrom(type))
            {
                return true;
            }

            return type.IsGenericType && typeof(ValueTask<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value of the type.</returns>
        public static object? GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// Gets the mapped object binding configuration for the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingConfigurationProvider">The binding configuration provider.</param>
        /// <returns>The mapped object binding configuration for the type.</returns>
        public static ObjectBindingConfiguration? GetBindingConfiguration(this Type type, IBindingConfigurationProvider bindingConfigurationProvider)
        {
            var bindingConfiguration = bindingConfigurationProvider.Get(type);
            return new BindingConfigurationMapper().MapFromBindingConfiguration(bindingConfiguration);
        }

        private sealed class BindingConfigurationMapper
        {
            private readonly Dictionary<BindingConfiguration, ObjectBindingConfiguration> ProcessedBindings = new();

            public ObjectBindingConfiguration? MapFromBindingConfiguration(BindingConfiguration? bindingConfiguration)
            {
                if (bindingConfiguration is null)
                {
                    return null;
                }

                if (ProcessedBindings.TryGetValue(bindingConfiguration, out var processedBinding))
                {
                    if (string.IsNullOrEmpty(processedBinding.Id))
                    {
                        processedBinding.Id = Guid.NewGuid().ToString();
                    }

                    return new ObjectBindingConfiguration()
                    {
                        ReferenceId = processedBinding.Id
                    };
                }

                var objectBindingConfiguration = new ObjectBindingConfiguration();
                ProcessedBindings.Add(bindingConfiguration, objectBindingConfiguration);

                objectBindingConfiguration.Include = bindingConfiguration.IncludeProperties;
                objectBindingConfiguration.Exclude = bindingConfiguration.ExcludeProperties;
                objectBindingConfiguration.IsBindingBase = bindingConfiguration.IsBindingBase;
                objectBindingConfiguration.PropertyBindings = bindingConfiguration.PropertyBindings?.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => MapFromBindingConfiguration(keyValuePair.Value));
                objectBindingConfiguration.ArrayItemBinding = MapFromBindingConfiguration(bindingConfiguration.ArrayItemBinding);
                return objectBindingConfiguration;
            }
        }
    }
}
