// -----------------------------------------------------------------------
// <copyright file="FileInfoParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

public class FileInfoParserTests
{
    [Test]
    public async Task ParseFile()
    {
        _ = await Assert.That(FileInfoParser.Parse(typeof(FileInfoParserTests).Assembly.Location)).IsNotNull();
    }

    [Test]
    public async Task ParseDirectory()
    {
        _ = await Assert.That(FileInfoParser.ParseAll(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location))).HasCount().GreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task ParsePattern()
    {
        _ = await Assert.That(FileInfoParser.ParseAll(Path.Combine(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location)!, "*.dll"))).HasCount().GreaterThanOrEqualTo(2);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task ParseEmpty(string? path)
    {
        _ = await Assert.That(FileInfoParser.ParseAll(path)).IsEmpty();
    }

    [Test]
    public async Task ParseMultiple()
    {
        _ = await Assert.That(() => FileInfoParser.Parse(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location))).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ParseNone()
    {
        _ = await Assert.That(() => FileInfoParser.Parse(Path.ChangeExtension(typeof(FileInfoParserTests).Assembly.Location, ".bad"))).Throws<FileNotFoundException>();
    }

    [Test]
    public async Task ExpandTilde()
    {
        _ = await Assert.That(() => FileInfoParser.ExpandPath("~")).IsEqualTo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }

    [Test]
    public async Task ExpandHome()
    {
        _ = await Assert.That(() => FileInfoParser.ExpandPath("%HOME%")).IsEqualTo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }


}