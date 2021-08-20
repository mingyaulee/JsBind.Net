using System;

namespace JsBind.Net.Internal
{
    /// <summary>
    /// Performs operations on string instances that contains access path in JavaScript.
    /// </summary>
    internal static class AccessPaths
    {
        private const string ReferenceIdPrefix = "#";
        private static readonly int ReferenceIdLength = ReferenceIdPrefix.Length + Guid.Empty.ToString().Length;

        public static bool IsReferenceId(string? referenceId)
        {
            return referenceId is not null && referenceId.StartsWith(ReferenceIdPrefix) && referenceId.Length == ReferenceIdLength;
        }

        public static Guid? GetReferenceId(string? accessPath)
        {
            var referenceId = accessPath?[ReferenceIdPrefix.Length..];
            if (referenceId is null)
            {
                return null;
            }

            if (Guid.TryParse(referenceId, out var referenceIdGuid))
            {
                return referenceIdGuid;
            }

            return null;
        }
    }
}
