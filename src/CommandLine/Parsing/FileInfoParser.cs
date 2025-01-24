// -----------------------------------------------------------------------
// <copyright file="FileInfoParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="FileInfo" /> parsers for <see cref="Argument{T}.CustomParser"/> or <see cref="Option{T}.CustomParser"/>.
/// </summary>
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
    public static FileInfo[] ParseAll(IEnumerable<string?> values) => values.SelectMany(ParseAllCore).ToArray();

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
    public static FileInfo[] ParseAll(string? value) => ParseAllCore(value).ToArray();

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

    private static IEnumerable<FileInfo> ParseAllCore(string? value)
    {
        return value switch
        {
            { Length: 0 } => [],
            { } v when File.Exists(v) => Create(new FileInfo(v)),
            { } v when Directory.Exists(v) => CreateFromDirectory(v, "*.*"),
            { } v when Directory.Exists(GetDirectory(v)) => CreateFromDirectory(GetDirectory(v), Path.GetFileName(v)),
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

        static IEnumerable<FileInfo> CreateFromDirectory(string directory, string searchPattern)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.EnumerateFiles(searchPattern);
        }
    }
}