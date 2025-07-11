﻿// -----------------------------------------------------------------------
// <copyright file="RangeParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="Range" /> parsers for <see cref="Argument{T}.CustomParser"/> or <see cref="Option{T}.CustomParser"/>.
/// </summary>
public static class RangeParser
{
    private const string Separator = "..";

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Range Parse(ArgumentResult argumentResult) => Parse(argumentResult.Tokens[^1]);

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The parsed value.</returns>
    public static Range Parse(Token token) => Parse(token.Value);

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Range Parse(string value)
    {
        // split on the '..'
        ReadOnlySpan<char> span = value;
        var index = span.IndexOf(Separator, StringComparison.Ordinal);

        // do the start and end
        var startSpan = span[..index];
        var endSpan = span[(index + Separator.Length)..];

        var startIndex = startSpan.Length == 0
            ? Index.Start
            : IndexParser.Parse(startSpan);

        var endIndex = endSpan.Length is 0
            ? Index.End
            : IndexParser.Parse(endSpan);

        return new Range(startIndex, endIndex);
    }

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseAll(ArgumentResult argumentResult) => ParseAll(argumentResult.Tokens);

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseAll(IEnumerable<Token> tokens) => ParseAll(tokens.Select(static token => token.Value));

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="values">The strings to parse.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseAll(IEnumerable<string> values) => [.. values.Select(Parse)];
}