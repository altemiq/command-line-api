// -----------------------------------------------------------------------
// <copyright file="CliOptions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Extensions;

/// <summary>
/// The <see cref="CliOption"/> values.
/// </summary>
public static class CliOptions
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    private static readonly string[] Separator = [".."];

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Range ParseRange(Parsing.ArgumentResult argumentResult) => ParseRange(argumentResult.Tokens[^1]);

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The parsed value.</returns>
    public static Range ParseRange(Parsing.CliToken token) => ParseRange(token.Value);

    /// <summary>
    /// Parses the range.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Range ParseRange(string value)
    {
        // split on the '..'
        var split = value.Split(Separator, StringSplitOptions.None);

        // do the start and end
        var startString = split[0];
        var endString = split[1];

        var startIndex = string.IsNullOrEmpty(startString)
            ? Index.Start
            : ParseIndex(startString);

        var endIndex = string.IsNullOrEmpty(endString)
            ? Index.End
            : ParseIndex(endString);

        return new Range(startIndex, endIndex);
    }

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseRanges(Parsing.ArgumentResult argumentResult) => ParseRanges(argumentResult.Tokens);

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseRanges(IEnumerable<Parsing.CliToken> tokens) => ParseRanges(tokens.Select(token => token.Value));

    /// <summary>
    /// Parses the ranges.
    /// </summary>
    /// <param name="values">The strings to parse.</param>
    /// <returns>The parsed values.</returns>
    public static Range[] ParseRanges(IEnumerable<string> values) => values.Select(ParseRange).ToArray();

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Index ParseIndex(Parsing.ArgumentResult argumentResult) => ParseIndex(argumentResult.Tokens[^1]);

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The parsed value.</returns>
    public static Index ParseIndex(Parsing.CliToken token) => ParseIndex(token.Value);

    /// <summary>
    /// Parses the index.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index ParseIndex(string value)
    {
        var fromEnd = value.StartsWith('^');
        return new Index(int.Parse(fromEnd ? value[1..] : value, System.Globalization.CultureInfo.InvariantCulture), fromEnd);
    }

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseIndexes(Parsing.ArgumentResult argumentResult) => ParseIndexes(argumentResult.Tokens);

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="tokens">The tokens to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseIndexes(IEnumerable<Parsing.CliToken> tokens) => ParseIndexes(tokens.Select(token => token.Value));

    /// <summary>
    /// Parses the indexes.
    /// </summary>
    /// <param name="values">The strings to parse.</param>
    /// <returns>The parsed value.</returns>
    public static Index[] ParseIndexes(IEnumerable<string> values) => values.Select(ParseIndex).ToArray();
#endif

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] ParseGlobbing(Parsing.ArgumentResult argumentResult, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo = default) => ParseGlobbing(argumentResult.Tokens, directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] ParseGlobbing(IEnumerable<Parsing.CliToken> tokens, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo = default) => ParseGlobbing(tokens.Select(token => token.Value), directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="globs">The globs.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] ParseGlobbing(IEnumerable<string> globs, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo = default)
    {
        return Process(globs, directoryInfo).SelectMany(results => results.Select(file => new FileInfo(file))).ToArray();

        static IEnumerable<IEnumerable<string>> Process(IEnumerable<string> tokens, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo)
        {
            var patternBuilder = new Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns.PatternBuilder(StringComparison.OrdinalIgnoreCase);
            var normalisedTokens = tokens.Select(NormalizedName).ToArray();
            var rooted = normalisedTokens.Where(Path.IsPathRooted).ToArray();
            foreach (var root in rooted)
            {
                yield return GetRooted(root, directoryInfo, patternBuilder);
            }

            yield return GetFiles(directoryInfo ?? GetDirectoryInfo(Environment.CurrentDirectory), patternBuilder, normalisedTokens.Except(rooted, StringComparer.Ordinal).ToArray());

            static IEnumerable<string> GetRooted(string root, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo, Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns.PatternBuilder patternBuilder)
            {
                // find the first '*' element
                var starIndex =
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                    root.IndexOf('*', StringComparison.Ordinal);
#else
                    root.IndexOf('*');
#endif
                if (starIndex < 0)
                {
                    yield return root;
                    yield break;
                }

                // trace back to the directory
                string pattern;
                string directoryPath;
                var directorySeparatorIndex = root.LastIndexOf(Path.AltDirectorySeparatorChar, starIndex);
                if (directorySeparatorIndex < 0)
                {
                    // there is no root below this
                    pattern = root;
                    directoryPath = string.Empty;
                }
                else
                {
#if NETSTANDARD2_0
                    directoryPath = root.Substring(0, directorySeparatorIndex);
                    pattern = root.Substring(directorySeparatorIndex + 1);
#else
                    directoryPath = root[..directorySeparatorIndex];
                    pattern = root[(directorySeparatorIndex + 1)..];
#endif
                }

                if (directoryInfo is null || !string.Equals(NormalizedName(directoryInfo.FullName), directoryPath, StringComparison.Ordinal))
                {
                    directoryInfo = GetDirectoryInfo(directoryPath);
                }

                foreach (var match in GetFiles(directoryInfo!, patternBuilder, pattern))
                {
                    yield return match;
                }
            }

            static IEnumerable<string> GetFiles(Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directoryInfo, Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns.PatternBuilder patternBuilder, params string[] includePatterns)
            {
                if (includePatterns.Length == 0)
                {
                    yield break;
                }

                var context = new Microsoft.Extensions.FileSystemGlobbing.Internal.MatcherContext(
                    includePatterns.Select(includePattern => patternBuilder.Build(includePattern)),
                    [],
                    directoryInfo,
                    patternBuilder.ComparisonType);
                var result = context.Execute();
                var directoryName = directoryInfo.FullName;
                foreach (var match in result.Files)
                {
                    yield return Path.GetFullPath(Path.Combine(directoryName, match.Path));
                }
            }

            static Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase GetDirectoryInfo(string directoryPath)
            {
                return new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(directoryPath));
            }
        }

        static string NormalizedName(string name)
        {
            return name.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}