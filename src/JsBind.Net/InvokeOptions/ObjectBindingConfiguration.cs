﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsBind.Net.InvokeOptions
{
    /// <summary>
    /// The binding configuration for an object. In sync with JsBindScripts\Modules\InvokeOptions\ObjectBindingConfiguration.js
    /// </summary>
    public class ObjectBindingConfiguration
    {
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
        /// The binding for the items in the array like object.
        /// </summary>
        [JsonPropertyName("arrayItemBinding")]
        public ObjectBindingConfiguration? ArrayItemBinding { get; set; }
    }
}
