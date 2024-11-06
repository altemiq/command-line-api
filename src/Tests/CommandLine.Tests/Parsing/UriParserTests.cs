// -----------------------------------------------------------------------
// <copyright file="UriParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

public class UriParserTests
{
    [Fact]
    public void CreateUri()
    {
        _ = UriParser.CreateUri(new FileInfo(typeof(UriParserTests).Assembly.Location)).Should().NotBeNull();
    }

    [Fact]
    public void CreateFileInfo()
    {
        UriBuilder builder = new()
        {
            Scheme = Uri.UriSchemeFile,
            Path = typeof(UriParserTests).Assembly.Location,
            Host = string.Empty,
        };

        _ = UriParser.TryCreateFileInfo(builder.Uri, out FileInfo? fileInfo).Should().BeTrue();
        _ = fileInfo!.FullName.Should().Be(typeof(UriParserTests).Assembly.Location);
    }

    [Fact]
    public void ParseFromDirectory()
    {
        _ = UriParser.ParseAll(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location)).Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ParseFromGlob()
    {
        _ = UriParser.ParseAll(Path.Join(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location), "*.*")).Should().HaveCountGreaterThan(1);
    }
}