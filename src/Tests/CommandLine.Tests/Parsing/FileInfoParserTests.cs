// -----------------------------------------------------------------------
// <copyright file="FileInfoParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

using TUnit.Assertions.AssertConditions.Throws;

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
        _ = await Assert.That(FileInfoParser.ParseAll(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location))).HasCount().GreaterThan(1);
    }

    [Test]
    public async Task ParsePattern()
    {
        _ = await Assert.That(FileInfoParser.ParseAll(Path.Combine(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location)!, "*.dll"))).HasCount().GreaterThan(1);
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
        await Assert.That(() => FileInfoParser.Parse(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location))).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ParseNone()
    {
        await Assert.That(() => FileInfoParser.Parse(Path.ChangeExtension(typeof(FileInfoParserTests).Assembly.Location, ".bad"))).Throws<FileNotFoundException>();
    }
}