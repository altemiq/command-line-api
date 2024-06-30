// -----------------------------------------------------------------------
// <copyright file="ArgumentValidationTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class ArgumentValidationTests
{
    [Fact]
    public void FileExists() => AcceptMissingOnly(new CliArgument<FileInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);

    [Fact]
    public void FilesExists() => AcceptMissingOnly(new CliArgument<FileInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);

    [Fact]
    public void DirectoryExists() => AcceptMissingOnly(new CliArgument<DirectoryInfo>("DIRECTORY").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);

    [Fact]
    public void DirectoriesExists() => AcceptMissingOnly(new CliArgument<DirectoryInfo[]>("DIRECTORIES").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);

    [Fact]
    public void FileSystemInfoExists() => AcceptMissingOnly(new CliArgument<FileSystemInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);

    [Fact]
    public void FileSystemInfosExists() => AcceptMissingOnly(new CliArgument<FileSystemInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);

    [Fact]
    public void CorrectUriScheme()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttps();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriScheme()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttp();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CorrectUriSchemes()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttpOrHttps();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriSchemes()
    {
        var argument = new CliArgument<Uri>("URI").AcceptSchemes(Uri.UriSchemeGopher, Uri.UriSchemeSsh);
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    private static void AcceptMissingOnly<T>(CliArgument<T> argument, string args)
    {
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse(args);
        _ = results.Errors.Should().NotBeEmpty();
    }
}