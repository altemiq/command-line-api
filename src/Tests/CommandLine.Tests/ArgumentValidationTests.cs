// -----------------------------------------------------------------------
// <copyright file="ArgumentValidationTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ArgumentValidationTests
{
    [Fact]
    public void FileExists()
    {
        var argument = new CliArgument<FileInfo>("FILE").AcceptMissingOnly();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse(typeof(ArgumentValidationTests).Assembly.Location);
        results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void DirectoryExists()
    {
        var argument = new CliArgument<DirectoryInfo>("DIRECTORY").AcceptMissingOnly();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse(Path.GetDirectoryName(typeof(ArgumentValidationTests).Assembly.Location) ?? string.Empty);
        results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CorrectUriScheme()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttps();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriScheme()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttp();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        results.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CorrectUriSchemes()
    {
        var argument = new CliArgument<Uri>("URI").AcceptHttpOrHttps();
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        results.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IncorrectUriSchemes()
    {
        var argument = new CliArgument<Uri>("URI").AcceptSchemes(Uri.UriSchemeGopher, Uri.UriSchemeSsh);
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);

        var results = configuration.Parse("https://www.google.com");
        results.Errors.Should().NotBeEmpty();
    }
}
