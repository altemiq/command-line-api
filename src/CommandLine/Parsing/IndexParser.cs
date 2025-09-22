// -----------------------------------------------------------------------
// <copyright file="IndexParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

#pragma warning disable CS0419
/// <summary>
/// <see cref="Index" /> parsers for <see cref="Argument{T}.CustomParser"/> or <see cref="Option{T}.CustomParser"/>.
/// </summary>
#pragma warning restore CS0419
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
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
    public static Index Parse(Token token) => Parse(token.Value);

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
    public static Index[] ParseAll(IEnumerable<Token> tokens) => ParseAll(tokens.Select(static token => token.Value));

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="values">The strings to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseAll(IEnumerable<string> values) => [.. values.Select(Parse)];

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="value">The span to parse.</param>
    /// <returns>The parsed value.</returns>
    internal static Index Parse(ReadOnlySpan<char> value)
    {
        var fromEnd = value[0] is '^';
        return new Index(int.Parse(GetValue(fromEnd, value), provider: Globalization.CultureInfo.InvariantCulture), fromEnd);

#if NETSTANDARD2_1_OR_GREATER
        static ReadOnlySpan<char> GetValue(bool fromEnd, ReadOnlySpan<char> value)
        {
            return fromEnd ? value[1..] : value;
        }
#else
        static string GetValue(bool fromEnd, ReadOnlySpan<char> value)
        {
            return fromEnd ? value[1..].ToString() : value.ToString();
        }
#endif
    }
}