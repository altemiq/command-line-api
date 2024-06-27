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

    private static IEnumerable<T> Create<T>(T item)
    {
        yield return item;
    }
}