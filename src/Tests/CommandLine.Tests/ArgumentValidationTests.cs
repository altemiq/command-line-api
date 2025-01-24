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
        AcceptMissingOnly(new Argument<FileInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void FilesExists()
    {
        AcceptMissingOnly(new Argument<FileInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void DirectoryExists()
    {
        AcceptMissingOnly(new Argument<DirectoryInfo>("DIRECTORY").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);
    }

    [Fact]
    public void DirectoriesExists()
    {
        AcceptMissingOnly(new Argument<DirectoryInfo[]>("DIRECTORIES").AcceptMissingOnly(), Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);
    }

    [Fact]
    public void FileSystemInfoExists()
    {
        AcceptMissingOnly(new Argument<FileSystemInfo>("FILE").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void FileSystemInfosExists()
    {
        AcceptMissingOnly(new Argument<FileSystemInfo[]>("FILES").AcceptMissingOnly(), typeof(ArgumentValidationTests).Assembly.Location);
    }

    [Fact]
    public void CorrectUriScheme()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttps();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriScheme()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttp();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CorrectUriSchemes()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttpOrHttps();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriSchemes()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptSchemes(Uri.UriSchemeGopher, Uri.UriSchemeSsh);
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = results.Errors.Should().NotBeEmpty();
    }

    private static void AcceptMissingOnly<T>(Argument<T> argument, string args)
    {
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse(args);
        _ = results.Errors.Should().NotBeEmpty();
    }
}