// -----------------------------------------------------------------------
// <copyright file="FileSystemGlobbingParser.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// <see cref="Microsoft.Extensions.FileSystemGlobbing" /> parsers for <see cref="CliArgument{T}.CustomParser"/> or <see cref="CliOption{T}.CustomParser"/>.
/// </summary>
public static class FileSystemGlobbingParser
{
    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] Parse(ArgumentResult argumentResult) => Parse(argumentResult, default);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] Parse(IEnumerable<CliToken> tokens) => Parse(tokens, default);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] Parse(CliToken token) => Parse(token, default);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="glob">The glob.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] Parse(string glob) => Parse(glob, default);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="globs">The globs.</param>
    /// <returns>The files matched by the globbing.</returns>
    public static FileInfo[] Parse(IEnumerable<string> globs) => Parse(globs, default);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="argumentResult">The argument result.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    internal static FileInfo[] Parse(ArgumentResult argumentResult, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo) => Parse(argumentResult.Tokens, directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    internal static FileInfo[] Parse(IEnumerable<CliToken> tokens, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo) => Parse(tokens.Select(token => token.Value), directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    internal static FileInfo[] Parse(CliToken token, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo) => Parse(token.Value, directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="glob">The glob.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    internal static FileInfo[] Parse(string glob, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo) => Parse(Create(glob), directoryInfo);

    /// <summary>
    /// Parses the file system globbing.
    /// </summary>
    /// <param name="globs">The globs.</param>
    /// <param name="directoryInfo">The directory information.</param>
    /// <returns>The files matched by the globbing.</returns>
    internal static FileInfo[] Parse(IEnumerable<string> globs, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo)
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
                return OrdinalIndex(root, '*') switch
                {
                    < 0 => Create(root),
                    var starIndex => GetFilesFromDirectory(root, starIndex, directoryInfo, patternBuilder),
                };

                static int OrdinalIndex(string text, char value)
                {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                    return text.IndexOf(value, StringComparison.Ordinal);
#else
                    return text.IndexOf(value);
#endif
                }

                static IEnumerable<string> GetFilesFromDirectory(string root, int starIndex, Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? directoryInfo, Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns.PatternBuilder patternBuilder)
                {
                    // trace back to the directory
                    var (directoryPath, pattern) = GetDirectoryAndPattern(root, starIndex);

                    if (directoryInfo is null || !string.Equals(NormalizedName(directoryInfo.FullName), directoryPath, StringComparison.Ordinal))
                    {
                        directoryInfo = GetDirectoryInfo(directoryPath);
                    }

                    foreach (var match in GetFiles(directoryInfo!, patternBuilder, pattern))
                    {
                        yield return match;
                    }

                    static (string DirectoryPath, string Pattern) GetDirectoryAndPattern(string root, int startIndex)
                    {
                        var directorySeparatorIndex = root.LastIndexOf(Path.AltDirectorySeparatorChar, startIndex);
                        if (directorySeparatorIndex < 0)
                        {
                            // there is no root below this
                            return (string.Empty, root);
                        }
                        else
                        {
#if NETSTANDARD2_0
                        return (root.Substring(0, directorySeparatorIndex), root.Substring(directorySeparatorIndex + 1));
#else
                            return (root[..directorySeparatorIndex], root[(directorySeparatorIndex + 1)..]);
#endif
                        }
                    }
                }
            }

            static IEnumerable<string> GetFiles(Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directoryInfo, Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns.PatternBuilder patternBuilder, params string[] includePatterns)
            {
                if (includePatterns.Length is 0)
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

    private static IEnumerable<T> Create<T>(T item)
    {
        yield return item;
    }
}