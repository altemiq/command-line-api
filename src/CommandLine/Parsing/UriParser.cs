// -----------------------------------------------------------------------
// <copyright file="UriParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="Uri" /> parsers for <see cref="Argument{T}.CustomParser"/> or <see cref="Option{T}.CustomParser"/>.
/// </summary>
public static class UriParser
{
    /// <summary>
    /// Creates a <see cref="Uri"/> from a <see cref="FileInfo"/>.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The <see cref="Uri"/> representing <paramref name="file"/>.</returns>
    public static Uri CreateUri(FileInfo file) => CreateFileUri(file.FullName);

    /// <summary>
    /// Tries to create a <see cref="FileInfo"/> from a <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">The input URI.</param>
    /// <param name="fileInfo">The output <see cref="FileInfo"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="uri"/> can be represented as a <see cref="FileInfo"/>; otherwise <see langword="false"/>.</returns>
    public static bool TryCreateFileInfo(Uri uri, [Diagnostics.CodeAnalysis.NotNullWhen(true)] out FileInfo? fileInfo)
    {
        if (uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.Ordinal))
        {
            var path = uri.LocalPath;
            fileInfo = new FileInfo(path);
            return true;
        }

        fileInfo = default;
        return false;
    }

    /// <summary>
    /// Parses the URIs from the argument results.
    /// </summary>
    /// <param name="argumentResult">The argument results.</param>
    /// <returns>The URIs.</returns>
    public static Uri[] ParseAll(ArgumentResult argumentResult) => ParseAll(argumentResult.Tokens);

    /// <summary>
    /// Parses the URIs from the tokens.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>The URIs.</returns>
    public static Uri[] ParseAll(IEnumerable<Token> tokens) => ParseAll(tokens.Select(token => token.Value));

    /// <summary>
    /// Parses the URIs from the values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The URIs.</returns>
    public static Uri[] ParseAll(IEnumerable<string?> values) => values.SelectMany(ParseAllCore).ToArray();

    /// <summary>
    /// Parses the URIs from the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The URIs.</returns>
    public static Uri[] ParseAll(Token token) => ParseAll(token.Value);

    /// <summary>
    /// Parses the URIs from the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The URIs.</returns>
    public static Uri[] ParseAll(string? value) => ParseAllCore(value).ToArray();

    /// <summary>
    /// Parses the URI from the argument results.
    /// </summary>
    /// <param name="argumentResult">The argument results.</param>
    /// <returns>The URI.</returns>
    public static Uri Parse(ArgumentResult argumentResult) => ParsingHelpers.ReturnSingle(ParseAll(argumentResult.Tokens), () => new InvalidOperationException(Properties.Resources.MultipleUris), () => new InvalidOperationException(Properties.Resources.NoUris));

    /// <summary>
    /// Parses the URI from the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The URI.</returns>
    public static Uri Parse(Token token) => Parse(token.Value);

    /// <summary>
    /// Parses the URI from the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The URI.</returns>
    public static Uri Parse(string? value) => ParsingHelpers.ReturnSingle(ParseAllCore(value), () => new InvalidOperationException(Properties.Resources.MultipleUris), () => new InvalidOperationException(Properties.Resources.NoUris));

    private static IEnumerable<Uri> ParseAllCore(string? value)
    {
        return value switch
        {
            { } v when Uri.IsWellFormedUriString(v, UriKind.Absolute) => Create(new Uri(v)),
            { } v when File.Exists(v) => Create(CreateFileUri(v)),
            { } v when Directory.Exists(v) => Directory.EnumerateFiles(v, "*.*").Select(CreateFileUri),
            { } v when Directory.Exists(GetDirectory(v)) => Directory.EnumerateFiles(GetDirectory(v), Path.GetFileName(v)).Select(CreateFileUri),
            _ => [],
        };

        static string GetDirectory(string value)
        {
            return Path.GetDirectoryName(value) ?? Directory.GetCurrentDirectory();
        }

        static IEnumerable<T> Create<T>(T value)
        {
            yield return value;
        }
    }

    private static Uri CreateFileUri(string path)
    {
        var builder = new UriBuilder
        {
            Scheme = Uri.UriSchemeFile,
            Path = GetFullPath(path),
            Host = string.Empty,
        };

        return builder.Uri;

        static string GetFullPath(string path)
        {
            // root the path here
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            return Path.IsPathFullyQualified(path)
                ? path
                : Path.GetFullPath(path);
#else
            return Path.GetFullPath(path);
#endif
        }
    }
}