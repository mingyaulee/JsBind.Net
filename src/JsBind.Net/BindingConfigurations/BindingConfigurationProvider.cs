using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsBind.Net.Internal.Extensions;

namespace JsBind.Net.BindingConfigurations
{
    /// <summary>
    /// The provider of binding configuration.
    /// </summary>
    internal class BindingConfigurationProvider : IBindingConfigurationProvider
    {
        private readonly Dictionary<Type, BindingConfiguration?> bindingConfigurations = [];

        /// <inheritdoc />
        public BindingConfiguration? Get(Type type)
        {
            if (type.IsPrimitiveType() || type.IsValueType || type == typeof(object))
            {
                return null;
            }

            if (type.IsIterableType())
            {
                return GetArrayBindingConfiguration(type.GetIterableItemType());
            }

            if (!bindingConfigurations.ContainsKey(type))
            {
                TryAddFromAttribute(type);
            }

            var bindingConfiguration = bindingConfigurations[type];
            if (bindingConfiguration?.IncludeProperties is not null && bindingConfiguration.PropertyBindings is null)
            {
                bindingConfiguration.PropertyBindings = new Dictionary<string, BindingConfiguration?>();
                foreach (var propertyBinding in GetPropertyBindingsFromType(type, bindingConfiguration.IncludeProperties))
                {
                    bindingConfiguration.PropertyBindings.Add(propertyBinding);
                }
            }

            return bindingConfiguration;
        }

        /// <inheritdoc />
        public void Add(Type type, BindingConfiguration? bindingConfiguration)
        {
            if (bindingConfigurations.ContainsKey(type))
            {
                throw new InvalidOperationException($"The type {type} has been configured for binding.");
            }

            bindingConfigurations[type] = bindingConfiguration;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDeclaredProperties(Type type)
            => GetTypeProperties(type).Keys;

        private static Dictionary<string, PropertyInfo> GetTypeProperties(Type type)
            => type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propertyInfo =>
                {
                    if (!propertyInfo.CanWrite)
                    {
                        return false;
                    }

                    if (propertyInfo.IsDefined(typeof(JsonExtensionDataAttribute)))
                    {
                        return false;
                    }

                    if (propertyInfo.IsDefined(typeof(JsonIgnoreAttribute)))
                    {
                        var ignoreAttribute = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>();
                        if (ignoreAttribute?.Condition == JsonIgnoreCondition.Always)
                        {
                            return false;
                        }
                    }

                    if (propertyInfo.IsDefined(typeof(BindIgnoreAttribute)))
                    {
                        return false;
                    }

                    return true;
                })
                .ToDictionary(
                    GetPropertyName,
                    propertyInfo => propertyInfo,
                    StringComparer.OrdinalIgnoreCase
                );

        private BindingConfiguration? GetArrayBindingConfiguration(Type? type)
        {
            if (type is null)
            {
                return null;
            }

            return new BindingConfiguration()
            {
                ArrayItemBinding = Get(type)
            };
        }

        private void TryAddFromAttribute(Type type)
        {
            var bindAttributes = Enumerable.Empty<BaseJsBindAttribute>();
            if (type.IsDefined(typeof(BaseJsBindAttribute), false))
            {
                bindAttributes = type.GetCustomAttributes<BaseJsBindAttribute>(false);
            }
            else if (!type.IsIterableType())
            {
                bindAttributes = [new BindAllPropertiesAttribute()];
            }

            if (!bindAttributes.Any())
            {
                Add(type, null);
                return;
            }

            var attributeBindingConfigurations = bindAttributes.Select(bindAttribute =>
            {
                if (bindAttribute.IncludeAllProperties)
                {
                    return BindingConfiguration.IncludeAllProperties;
                }
                else if (bindAttribute.IncludeDeclaredPropertiesOnly)
                {
                    return new BindingConfiguration()
                    {
                        IncludeProperties = GetDeclaredProperties(type)
                    };
                }
                else if (bindAttribute.IncludeProperties is not null)
                {
                    return new BindingConfiguration()
                    {
                        IncludeProperties = bindAttribute.IncludeProperties
                    };
                }
                else
                {
                    return new BindingConfiguration()
                    {
                        ExcludeProperties = bindAttribute.ExcludeProperties
                    };
                }
            }).ToList();

            var bindingConfiguration = new BindingConfiguration()
            {
                IncludeProperties = attributeBindingConfigurations.SelectMany(bc => bc.IncludeProperties ?? []),
                ExcludeProperties = attributeBindingConfigurations.SelectMany(bc => bc.ExcludeProperties ?? []),
                IsBindingBase = typeof(BindingBase).IsAssignableFrom(type)
            };
            Add(type, bindingConfiguration);
        }

        private Dictionary<string, BindingConfiguration?> GetPropertyBindingsFromType(Type type, IEnumerable<string> includeProperties)
        {
            var propertyBindings = new Dictionary<string, BindingConfiguration?>();
            var properties = GetTypeProperties(type);

            if (includeProperties.Count() == 1 && includeProperties.Single() == BindingConfiguration.IncludeAllPropertiesPattern)
            {
                includeProperties = properties.Keys;
            }

            foreach (var includeProperty in includeProperties)
            {
                if (!properties.TryGetValue(includeProperty, out var propertyInfo))
                {
                    continue;
                }

                propertyBindings.Add(includeProperty.ToUpper(), Get(propertyInfo.PropertyType));
            }

            return propertyBindings;
        }

        private static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;
            if (propertyInfo.IsDefined(typeof(JsonPropertyNameAttribute)))
            {
                var propertyNameAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (!string.IsNullOrEmpty(propertyNameAttribute?.Name))
                {
                    propertyName = propertyNameAttribute.Name;
                }
            }

            // default to camel case naming policy
            return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
        }
    }
}
