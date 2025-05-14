// -----------------------------------------------------------------------
// <copyright file="FileInfoParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="FileInfo" /> parsers for <see cref="Argument{T}.CustomParser"/> or <see cref="Option{T}.CustomParser"/>.
/// </summary>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static class FileInfoParser
{
    /// <summary>
    /// Parses the file information from the argument results.
    /// </summary>
    /// <param name="argumentResult">The argument results.</param>
    /// <returns>The file information.</returns>
    public static FileInfo[] ParseAll(ArgumentResult argumentResult) => ParseAll(argumentResult.Tokens);

    /// <summary>
    /// Parses the file information from the tokens.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>The file information.</returns>
    public static FileInfo[] ParseAll(IEnumerable<Token> tokens) => ParseAll(tokens.Select(token => token.Value));

    /// <summary>
    /// Parses the file information from the values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The file information.</returns>
    public static FileInfo[] ParseAll(IEnumerable<string?> values) => [.. values.SelectMany(ParseAllCore)];

    /// <summary>
    /// Parses the file information from the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The file information.</returns>
    public static FileInfo[] ParseAll(Token token) => ParseAll(token.Value);

    /// <summary>
    /// Parses the file information from the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The file information.</returns>
    public static FileInfo[] ParseAll(string? value) => [.. ParseAllCore(value)];

    /// <summary>
    /// Parses the file information from the argument results.
    /// </summary>
    /// <param name="argumentResult">The argument results.</param>
    /// <returns>The file information.</returns>
    public static FileInfo Parse(ArgumentResult argumentResult) => ParsingHelpers.ReturnSingle(ParseAll(argumentResult.Tokens), () => new InvalidOperationException(Properties.Resources.MultipleFiles), () => new FileNotFoundException(Properties.Resources.NoFiles));

    /// <summary>
    /// Parses the file information from the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The file information.</returns>
    public static FileInfo Parse(Token token) => Parse(token.Value);

    /// <summary>
    /// Parses the file information from the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The file information.</returns>
    public static FileInfo Parse(string? value) => ParsingHelpers.ReturnSingle(ParseAllCore(value), () => new InvalidOperationException(Properties.Resources.MultipleFiles), () => new FileNotFoundException(Properties.Resources.NoFiles));

    /// <summary>
    /// Expands the path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The expanded path.</returns>
    [return: Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(path))]
    internal static string? ExpandPath(string? path)
    {
        if (path is null)
        {
            return path;
        }

        path = Environment.ExpandEnvironmentVariables(path);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        return path.StartsWith('~') ? string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path[1..]) : path;
#else
        return path.StartsWith("~", StringComparison.Ordinal) ? string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path.Substring(1)) : path;
#endif
    }

    private static IEnumerable<FileInfo> ParseAllCore(string? value)
    {
        return ExpandPath(value) switch
        {
            { Length: 0 } => [],
            { } v when File.Exists(v) => Create(new FileInfo(v)),
            { } v when Directory.Exists(v) => CreateFromDirectory(v, "*.*"),
            { } v when GetParentDirectoryIfExists(v) is { } directory => CreateFromDirectory(directory, Path.GetFileName(v)),
            _ => [],
        };

        static string? GetParentDirectoryIfExists(string v)
        {
            if (Path.GetDirectoryName(v) is { } directory && Directory.Exists(directory))
            {
                return directory;
            }

            return default;
        }

        static IEnumerable<T> Create<T>(T value)
        {
            yield return value;
        }

        static IEnumerable<FileInfo> CreateFromDirectory(string directory, string searchPattern)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.EnumerateFiles(searchPattern);
        }
    }
}