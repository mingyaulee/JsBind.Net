using System;
using System.Linq;

namespace JsBind.Net;

/// <summary>
/// Performs operations on string instances that contains access path in JavaScript.
/// </summary>
public static class AccessPaths
{
    private const string AccessPathSeparator = ".";
    private const string ReferenceIdPrefix = "#";
    private static readonly int ReferenceIdLength = ReferenceIdPrefix.Length + Guid.Empty.ToString().Length;

    /// <summary>
    /// Checks if the access path is a reference identifier.
    /// </summary>
    /// <param name="accessPath">The access path.</param>
    /// <returns><c>true</c> if the access path is a reference identifier, <c>false</c> otherwise.</returns>
    public static bool IsReferenceId(string? accessPath)
        => accessPath is not null && accessPath.StartsWith(ReferenceIdPrefix) && accessPath.Length == ReferenceIdLength;

    /// <summary>
    /// Gets the reference identifier from the access path.
    /// </summary>
    /// <param name="accessPath">The access path.</param>
    /// <returns>The reference identifier.</returns>
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

    /// <summary>
    /// Gets the access path from the reference identifier.
    /// </summary>
    /// <param name="referenceId">The reference identifier.</param>
    /// <returns>The access path.</returns>
    public static string? FromReferenceId(Guid? referenceId)
    {
        if (referenceId is null)
        {
            return null;
        }

        return ReferenceIdPrefix + referenceId.Value.ToString();
    }

    /// <summary>
    /// Gets the access path from the reference identifier.
    /// </summary>
    /// <param name="referenceId">The reference identifier.</param>
    /// <returns>The access path.</returns>
    public static string? FromReferenceId(string? referenceId)
    {
        if (referenceId is null)
        {
            return null;
        }

        if (referenceId.Length != Guid.Empty.ToString().Length)
        {
            throw new InvalidOperationException($"{referenceId} is not a valid reference identifier.");
        }

        return ReferenceIdPrefix + referenceId;
    }

    /// <summary>
    /// Combines multiple access paths.
    /// </summary>
    /// <param name="accessPaths">The access paths.</param>
    /// <returns>The combined access path.</returns>
    public static string? Combine(params string?[] accessPaths)
        => string.Join(AccessPathSeparator, accessPaths.Where(accessPath => !string.IsNullOrEmpty(accessPath)));

    /// <summary>
    /// Splits the access path based on the access path separator.
    /// </summary>
    /// <param name="accessPath">The access path.</param>
    /// <returns>The array of individual access paths.</returns>
    public static string[]? Split(string? accessPath)
        => accessPath?.Split(AccessPathSeparator);
}
