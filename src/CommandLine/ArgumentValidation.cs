// -----------------------------------------------------------------------
// <copyright file="ArgumentValidation.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Provides extension methods for <see cref="CliArgument" />.
/// </summary>
public static class ArgumentValidation
{
    /// <summary>
    /// Validates that the <see cref="Uri"/> has the correct <see cref="Uri.Scheme"/> specified.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="scheme">The requied scheme.</param>
    /// <returns>The input argument.</returns>
    public static CliArgument<Uri> AcceptScheme(this CliArgument<Uri> argument, string scheme)
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
    public static CliArgument<Uri> AcceptSchemes(this CliArgument<Uri> argument, params string[] schemes)
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
    public static CliArgument<Uri> AcceptHttp(this CliArgument<Uri> argument) => AcceptScheme(argument, Uri.UriSchemeHttp);

    /// <summary>
    /// Validates that the <see cref="Uri"/> has its <see cref="Uri.Scheme"/> set to <see cref="Uri.UriSchemeHttps"/>.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The input argument.</returns>
    public static CliArgument<Uri> AcceptHttps(this CliArgument<Uri> argument) => AcceptScheme(argument, Uri.UriSchemeHttps);

    /// <summary>
    /// Validates that the <see cref="Uri"/> has its <see cref="Uri.Scheme"/> set to <see cref="Uri.UriSchemeHttp"/> or <see cref="Uri.UriSchemeHttps"/>.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The input argument.</returns>
    public static CliArgument<Uri> AcceptHttpOrHttps(this CliArgument<Uri> argument) => AcceptSchemes(argument, Uri.UriSchemeHttp, Uri.UriSchemeHttps);

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing file.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static CliArgument<FileInfo> AcceptMissingOnly(this CliArgument<FileInfo> argument)
    {
        argument.Validators.Add(FileOrDirectoryDoesNotExist<FileInfo>);
        return argument;
    }

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing directory.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static CliArgument<DirectoryInfo> AcceptMissingOnly(this CliArgument<DirectoryInfo> argument)
    {
        argument.Validators.Add(FileOrDirectoryDoesNotExist<DirectoryInfo>);
        return argument;
    }

    /// <summary>
    /// Configures an argument to accept only values corresponding to a missing directory.
    /// </summary>
    /// <param name="argument">The argument to configure.</param>
    /// <returns>The configured argument.</returns>
    public static CliArgument<FileSystemInfo> AcceptMissingOnly(this CliArgument<FileSystemInfo> argument)
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
    public static CliArgument<T> AcceptMissingOnly<T>(this CliArgument<T> argument)
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
        bool checkFile = typeof(T) != typeof(DirectoryInfo);
        bool checkDirectory = typeof(T) != typeof(FileInfo);

        for (var i = 0; i < result.Tokens.Count; i++)
        {
            var token = result.Tokens[i];

            if (checkFile && checkDirectory)
            {
#if NET7_0_OR_GREATER
                if (Path.Exists(token.Value))
#else
                if (Directory.Exists(token.Value) || File.Exists(token.Value))
#endif
                {
                    result.AddError(LocalizationResources.FileOrDirectoryExists(token.Value));
                }
            }
            else if (checkDirectory && Directory.Exists(token.Value))
            {
                result.AddError(LocalizationResources.DirectoryExists(token.Value));
            }
            else if (checkFile && File.Exists(token.Value))
            {
                result.AddError(LocalizationResources.FileExists(token.Value));
            }
        }
    }
}