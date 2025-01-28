// -----------------------------------------------------------------------
// <copyright file="UriParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

public class UriParserTests
{
    [Test]
    public async Task CreateUri()
    {
        _ = await Assert.That(UriParser.CreateUri(new FileInfo(typeof(UriParserTests).Assembly.Location))).IsNotNull();
    }

    [Test]
    public async Task CreateFileInfo()
    {
        UriBuilder builder = new()
        {
            Scheme = Uri.UriSchemeFile,
            Path = typeof(UriParserTests).Assembly.Location,
            Host = string.Empty,
        };

        _ = await Assert.That(UriParser.TryCreateFileInfo(builder.Uri, out FileInfo? fileInfo)).IsTrue();
        _ = await Assert.That(fileInfo!.FullName).IsEqualTo(typeof(UriParserTests).Assembly.Location);
    }

    [Test]
    public async Task ParseFromDirectory()
    {
        _ = await Assert.That(UriParser.ParseAll(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location))).HasCount().GreaterThan(1);
    }

    [Test]
    public async Task ParseFromGlob()
    {
        _ = await Assert.That(UriParser.ParseAll(Path.Join(Path.GetDirectoryName(typeof(UriParserTests).Assembly.Location), "*.*"))).HasCount().GreaterThan(1);
    }
}