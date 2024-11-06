// -----------------------------------------------------------------------
// <copyright file="ArgumentValidationTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class ArgumentValidationTests
{
    [Fact]
    public void FileExists()
    {
        AcceptMissingOnly(new CliArgument<FileInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void FilesExists()
    {
        AcceptMissingOnly(new CliArgument<FileInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void DirectoryExists()
    {
        AcceptMissingOnly(new CliArgument<DirectoryInfo>("DIRECTORY").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);
    }

    [Fact]
    public void DirectoriesExists()
    {
        AcceptMissingOnly(new CliArgument<DirectoryInfo[]>("DIRECTORIES").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);
    }

    [Fact]
    public void FileSystemInfoExists()
    {
        AcceptMissingOnly(new CliArgument<FileSystemInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void FileSystemInfosExists()
    {
        AcceptMissingOnly(new CliArgument<FileSystemInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void CorrectUriScheme()
    {
        CliArgument<Uri> argument = new CliArgument<Uri>("URI").AcceptHttps();
        CliRootCommand root = [argument];
        CliConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriScheme()
    {
        CliArgument<Uri> argument = new CliArgument<Uri>("URI").AcceptHttp();
        CliRootCommand root = [argument];
        CliConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CorrectUriSchemes()
    {
        CliArgument<Uri> argument = new CliArgument<Uri>("URI").AcceptHttpOrHttps();
        CliRootCommand root = [argument];
        CliConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriSchemes()
    {
        CliArgument<Uri> argument = new CliArgument<Uri>("URI").AcceptSchemes(Uri.UriSchemeGopher, Uri.UriSchemeSsh);
        CliRootCommand root = [argument];
        CliConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    private static void AcceptMissingOnly<T>(CliArgument<T> argument, string args)
    {
        CliRootCommand root = [argument];
        CliConfiguration configuration = new(root);

        ParseResult results = configuration.Parse(args);
        _ = results.Errors.Should().NotBeEmpty();
    }
}