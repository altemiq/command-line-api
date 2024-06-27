// -----------------------------------------------------------------------
// <copyright file="CliOptionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.FileSystemGlobbing.Parsing;

public class FileSystemGlobbingParserTests
{
    [Fact]
    public void FileSystemGlobbing()
    {
        var rootDir = Path.Join(Path.GetPathRoot(Environment.CurrentDirectory), "Files to Search");

        var first = Path.Join(rootDir, "first", "first.txt");
        var second = Path.Join(rootDir, "second.txt");
        var third = Path.Join(rootDir, "deep", "deep", "path", "third.txt");
        var forth = Path.Join(rootDir, "forth.doc");

        var directoryInfo = new Microsoft.Extensions.FileSystemGlobbing.InMemoryDirectoryInfo(rootDir, [first, second, third, forth]);

        var argument = new CliArgument<FileInfo[]>("FILES") { CustomParser = argumentResult => CommandLine.Parsing.FileSystemGlobbingParser.Parse(argumentResult, directoryInfo) };
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);
        var parseResult = configuration.Parse("\"" + Path.Combine(rootDir, "**", "*.txt") + "\"");

        parseResult.GetValue(argument).Should().NotBeNull()
            .And.Subject.Select(x => x.FullName).Should()
                .Contain(first).And
                .Contain(second).And
                .Contain(third).And
                .NotContain(forth);
    }
}