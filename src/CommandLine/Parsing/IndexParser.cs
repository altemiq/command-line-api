// -----------------------------------------------------------------------
// <copyright file="IndexParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="Index" /> parsers for <see cref="CliArgument{T}.CustomParser"/> or <see cref="CliOption{T}.CustomParser"/>.
/// </summary>
public static class IndexParser
{
    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Index Parse(ArgumentResult argumentResult) => Parse(argumentResult.Tokens[^1]);

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The parsed value.</returns>
    public static Index Parse(CliToken token) => Parse(token.Value);

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index Parse(string value) => Parse(value.AsSpan());

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseAll(ArgumentResult argumentResult) => ParseAll(argumentResult.Tokens);

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="tokens">The tokens to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseAll(IEnumerable<CliToken> tokens) => ParseAll(tokens.Select(token => token.Value));

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="values">The strings to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseAll(IEnumerable<string> values) => values.Select(Parse).ToArray();

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="value">The span to parse.</param>
    /// <returns>The parsed value.</returns>
    internal static Index Parse(ReadOnlySpan<char> value)
    {
        var fromEnd = value[0] is '^';
        return new Index(int.Parse(fromEnd ? value[1..] : value, provider: Globalization.CultureInfo.InvariantCulture), fromEnd);
    }
}
#endif