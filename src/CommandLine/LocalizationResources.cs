// -----------------------------------------------------------------------
// <copyright file="LocalizationResources.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Provides localizable strings for help and error messages.
/// </summary>
internal static class LocalizationResources
{
    /// <summary>
    /// Interpolates values into a localized string similar to Directory already exists: {0}.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The localised string.</returns>
    public static string DirectoryExists(string path) => GetResourceString(Properties.Resources.Culture, Properties.Resources.DirectoryExists, path);

    /// <summary>
    /// Interpolates values into a localized string similar to File already exists: {0}.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>The localised string.</returns>
    public static string FileExists(string filePath) => GetResourceString(Properties.Resources.Culture, Properties.Resources.FileExists, filePath);

    /// <summary>
    /// Interpolates values into a localized string similar to File or directory already exists: {0}.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The localised string.</returns>
    public static string FileOrDirectoryExists(string path) => GetResourceString(Properties.Resources.Culture, Properties.Resources.FileOrDirectoryExists, path);

    /// <summary>
    /// Interpolates values into a localized string similar to Scheme for {0} must be {1}.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="scheme">The scheme.</param>
    /// <returns>The localised string.</returns>
    public static string InvalidScheme(string uri, string scheme) => GetResourceString(Properties.Resources.Culture, Properties.Resources.InvalidScheme, uri, scheme);

    /// <summary>
    /// Interpolates values into a localized string similar to Scheme for {0} must be one of {1}.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="schemes">The schemes.</param>
    /// <returns>The localised string.</returns>
    public static string InvalidSchemes(string uri, IEnumerable<string> schemes) => GetResourceString(Properties.Resources.Culture, Properties.Resources.InvalidSchemes, uri, string.Join(", ", schemes));

    /// <summary>
    /// Interpolates values into a localized string similar to Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
    /// </summary>
    /// <returns>The localised string.</returns>
    public static string VerbosityOptionDescription() => GetResourceString(Properties.Resources.Culture, Properties.Resources.VerbosityOptionDescription);

    private static string GetResourceString(IFormatProvider? provider, string? resourceString, params object?[] formatArguments) => (resourceString, formatArguments) switch
    {
        (null, _) => string.Empty,
        ({ } s, { Length: not 0 } args) => string.Format(provider, s, args),
        ({ } s, _) => s,
    };
}