// -----------------------------------------------------------------------
// <copyright file="FileInfoParserTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

public class FileInfoParserTests
{
    [Fact]
    public void ParseFile() => FileInfoParser.Parse(typeof(FileInfoParserTests).Assembly.Location).Should().NotBeNull();

    [Fact]
    public void ParseDirectory() => FileInfoParser.ParseAll(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location)).Should().HaveCountGreaterThan(1);

    [Fact]
    public void ParsePattern() => FileInfoParser.ParseAll(Path.Combine(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location)!, "*.dll")).Should().HaveCountGreaterThan(1);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ParseEmpty(string? path) => FileInfoParser.ParseAll(path).Should().BeEmpty();

    [Fact]
    public void ParseMultiple()
    {
        Action action = () => FileInfoParser.Parse(Path.GetDirectoryName(typeof(FileInfoParserTests).Assembly.Location)).Should();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ParseNone()
    {
        Action action = () => FileInfoParser.Parse(Path.ChangeExtension(typeof(FileInfoParserTests).Assembly.Location, ".bad"));
        action.Should().Throw<FileNotFoundException>();
    }
}
