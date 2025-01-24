// -----------------------------------------------------------------------
// <copyright file="ArgumentValidation.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Provides extension methods for <see cref="Argument" />.
/// </summary>
public static class ArgumentValidation
{
    /// <summary>
    /// Validates that the <see cref="Uri"/> has the correct <see cref="Uri.Scheme"/> specified.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="scheme">The requied scheme.</param>
    /// <returns>The input argument.</returns>
    public static Argument<Uri> AcceptScheme(this Argument<Uri> argument, string scheme)
    {
        argument.Validators.Add(result =>
        {
            foreach (var url in result.Tokens.Select(t => t.Value))
            {
                if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)
                    && !string.Equals(uri.Scheme, scheme, StringComparison.OrdinalIgnoreCase))
                {
                    result.AddError(LocalizationResources.InvalidScheme(url, scheme));
                }
            }
        });

        return argument;
    }

    /// <summary>
    /// Validates that the <see cref="Uri"/> has the correct <see cref="Uri.Scheme"/> specified.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="schemes">The requied schemes.</param>
    /// <returns>The input argument.</returns>
    public static Argument<Uri> AcceptSchemes(this Argument<Uri> argument, params string[] schemes)
    {
        argument.Validators.Add(result =>
        {
            foreach (var url in result.Tokens.Select(t => t.Value))
            {
                if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)
                    && !schemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase))
                {
                    result.AddError(LocalizationResources.InvalidSchemes(url, schemes));
                }
            }
        });

        return argument;
    }

    /// <summary>
    /// Validates that the <see cref="Uri"/> has its <see cref="Uri.Scheme"/> set to <see cref="Uri.UriSchemeHttp"/>.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The input argument.</returns>
    public static Argument<Uri> AcceptHttp(this Argument<Uri> argument) => AcceptScheme(argument, Uri.UriSchemeHttp);

    /// <summary>
    /// Validates that the <see cref="Uri"/> has its <see cref="Uri.Scheme"/> set to <see cref="Uri.UriSchemeHttps"/>.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The input argument.</returns>
    public static Argument<Uri> AcceptHttps(this Argument<Uri> argument) => AcceptScheme(argument, Uri.UriSchemeHttps);

    /// <summary>
    /// Validates that the <see cref="Uri"/> has its <see cref="Uri.Scheme"/> set to <see cref="Uri.UriSchemeHttp"/> or <see cref="Uri.UriSchemeHttps"/>.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The input argument.</returns>
    public static Argument<Uri> AcceptHttpOrHttps(this Argument<Uri> argument) => AcceptSchemes(argument, Uri.UriSchemeHttp, Uri.UriSchemeHttps);

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing file.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static Argument<FileInfo> AcceptMissingOnly(this Argument<FileInfo> argument)
    {
        argument.Validators.Add(FileOrDirectoryDoesNotExist<FileInfo>);
        return argument;
    }

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing directory.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static Argument<DirectoryInfo> AcceptMissingOnly(this Argument<DirectoryInfo> argument)
    {
        argument.Validators.Add(FileOrDirectoryDoesNotExist<DirectoryInfo>);
        return argument;
    }

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing directory.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static Argument<FileSystemInfo> AcceptMissingOnly(this Argument<FileSystemInfo> argument)
    {
        argument.Validators.Add(FileOrDirectoryDoesNotExist<FileSystemInfo>);
        return argument;
    }

    /// <summary>
    /// Configures an argument to accept only values corresponding to missing files or directories.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FileSystemInfo"/>.</typeparam>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static Argument<T> AcceptMissingOnly<T>(this Argument<T> argument)
        where T : IEnumerable<FileSystemInfo>
    {
        if (typeof(IEnumerable<FileInfo>).IsAssignableFrom(typeof(T)))
        {
            argument.Validators.Add(FileOrDirectoryDoesNotExist<FileInfo>);
        }
        else if (typeof(IEnumerable<DirectoryInfo>).IsAssignableFrom(typeof(T)))
        {
            argument.Validators.Add(FileOrDirectoryDoesNotExist<DirectoryInfo>);
        }
        else
        {
            argument.Validators.Add(FileOrDirectoryDoesNotExist<FileSystemInfo>);
        }

        return argument;
    }

    private static void FileOrDirectoryDoesNotExist<T>(Parsing.ArgumentResult result)
        where T : FileSystemInfo
    {
        // both FileInfo and DirectoryInfo are sealed so following checks are enough
        var checkFile = typeof(T) != typeof(DirectoryInfo);
        var checkDirectory = typeof(T) != typeof(FileInfo);

        for (var i = 0; i < result.Tokens.Count; i++)
        {
            if (CheckToken(result.Tokens[i], checkFile, checkDirectory) is { } errorMessage)
            {
                result.AddError(errorMessage);
            }
        }

        static string? CheckToken(Parsing.Token token, bool checkFile, bool checkDirectory)
        {
            return CheckTokenValue(token.Value, checkFile, checkDirectory);

            static string? CheckTokenValue(string value, bool checkFile, bool checkDirectory)
            {
                if (checkFile)
                {
                    if (checkDirectory)
                    {
#if NET7_0_OR_GREATER
                        if (Path.Exists(token.Value))
#else
                        if (Directory.Exists(value) || File.Exists(value))
#endif
                        {
                            return LocalizationResources.FileOrDirectoryExists(value);
                        }
                    }
                    else if (File.Exists(value))
                    {
                        return LocalizationResources.FileExists(value);
                    }
                }
                else if (checkDirectory && Directory.Exists(value))
                {
                    return LocalizationResources.DirectoryExists(value);
                }

                return default;
            }
        }
    }
}