using System.Text.Json.Serialization;

namespace JsBind.Net.Internal.References;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum ReferenceType
{
    Object,
    Delegate
}
