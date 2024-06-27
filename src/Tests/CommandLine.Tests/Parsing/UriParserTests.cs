// -----------------------------------------------------------------------
// <copyright file="UriParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

using System.Security.Cryptography.X509Certificates;

public class UriParserTests
{
    [Fact]
    public void CreateUri() => UriParser.CreateUri(new FileInfo(typeof(UriParserTests).Assembly.Location)).Should().NotBeNull();

    [Fact]
    public void CreateFileInfo()
    {
        var builder = new UriBuilder
        {
            Scheme = Uri.UriSchemeFile,
            Path = typeof(UriParserTests).Assembly.Location,
            Host = string.Empty,
        };

        UriParser.TryCreateFileInfo(builder.Uri, out var fileInfo).Should().BeTrue();
        fileInfo!.FullName.Should().Be(typeof(UriParserTests).Assembly.Location);
    }

    [Fact]
    public void ParseFromDirectory() => UriParser.ParseAll(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location)).Should().HaveCountGreaterThan(1);

    [Fact]
    public void ParseFromGlob() => UriParser.ParseAll(Path.Join(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location), "*.*")).Should().HaveCountGreaterThan(1);
}
