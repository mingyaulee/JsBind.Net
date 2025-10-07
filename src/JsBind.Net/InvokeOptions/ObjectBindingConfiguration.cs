using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions;

/// <summary>
/// The binding configuration for an object. In sync with Modules\InvokeOptions\ObjectBindingConfiguration.js
/// </summary>
public class ObjectBindingConfiguration
{
    /// <summary>
    /// Indicate that this is an object binding configuration instance.
    /// </summary>
    [JsonPropertyName("__isObjectBindingConfiguration")]
    public bool IsObjectBindingConfiguration { get; } = true;

    /// <summary>
    /// The identifier of the binding configuration.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The reference identifier of the binding configuration.
    /// </summary>
    [JsonPropertyName("referenceId")]
    public string? ReferenceId { get; set; }

    /// <summary>
    /// The properties to include for binding.
    /// </summary>
    [JsonPropertyName("include")]
    public IEnumerable<string>? Include { get; set; }

    /// <summary>
    /// The properties to exclude for binding.
    /// </summary>
    [JsonPropertyName("exclude")]
    public IEnumerable<string>? Exclude { get; set; }

    /// <summary>
    /// The binding for each of the property that is bound.
    /// </summary>
    [JsonPropertyName("propertyBindings")]
    public IDictionary<string, ObjectBindingConfiguration?>? PropertyBindings { get; set; }

    /// <summary>
    /// Indicates whether the access path for this object should be set when binding.
    /// </summary>
    [JsonPropertyName("isBindingBase")]
    public bool IsBindingBase { get; set; }

    /// <summary>
    /// The binding for the items in the array like object.
    /// </summary>
    [JsonPropertyName("arrayItemBinding")]
    public ObjectBindingConfiguration? ArrayItemBinding { get; set; }
}
