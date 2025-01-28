// -----------------------------------------------------------------------
// <copyright file="ArgumentValidationTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class ArgumentValidationTests
{
    [Test]
    public async Task FileExists()
    {
        await AcceptMissingOnly(new Argument<FileInfo>("FILE").AcceptMissingOnly(), typeof(TestAttribute).Assembly.Location);
    }

    [Test]
    public async Task FilesExists()
    {
        await AcceptMissingOnly(new Argument<FileInfo[]>("FILES").AcceptMissingOnly(), typeof(TestAttribute).Assembly.Location);
    }

    [Test]
    public async Task DirectoryExists()
    {
        await AcceptMissingOnly(new Argument<DirectoryInfo>("DIRECTORY").AcceptMissingOnly(), Path.GetDirectoryName(typeof(TestAttribute).Assembly.Location) ?? string.Empty);
    }

    [Test]
    public async Task DirectoriesExists()
    {
        await AcceptMissingOnly(new Argument<DirectoryInfo[]>("DIRECTORIES").AcceptMissingOnly(), Path.GetDirectoryName(typeof(TestAttribute).Assembly.Location) ?? string.Empty);
    }

    [Test]
    public async Task FileSystemInfoExists()
    {
        await AcceptMissingOnly(new Argument<FileSystemInfo>("FILE").AcceptMissingOnly(), typeof(TestAttribute).Assembly.Location);
    }

    [Test]
    public async Task FileSystemInfosExists()
    {
        await AcceptMissingOnly(new Argument<FileSystemInfo[]>("FILES").AcceptMissingOnly(), typeof(TestAttribute).Assembly.Location);
    }

    [Test]
    public async Task CorrectUriScheme()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttps();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = await Assert.That(results.Errors).IsEmpty();
    }

    [Test]
    public async Task IncorrectUriScheme()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttp();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = await Assert.That(results.Errors).IsNotEmpty();
    }

    [Test]
    public async Task CorrectUriSchemes()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptHttpOrHttps();
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = await Assert.That(results.Errors).IsEmpty();
    }

    [Test]
    public async Task IncorrectUriSchemes()
    {
        Argument<Uri> argument = new Argument<Uri>("URI").AcceptSchemes(Uri.UriSchemeGopher, Uri.UriSchemeSsh);
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse("https://www.google.com");
        _ = await Assert.That(results.Errors).IsNotEmpty();
    }

    private static async Task AcceptMissingOnly<T>(Argument<T> argument, string args)
    {
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);

        ParseResult results = configuration.Parse(args);
        _ = await Assert.That(results.Errors).IsNotEmpty();
    }
}