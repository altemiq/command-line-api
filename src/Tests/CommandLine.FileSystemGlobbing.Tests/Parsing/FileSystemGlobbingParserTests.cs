// -----------------------------------------------------------------------
// <copyright file="OptionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.FileSystemGlobbing.Parsing;

public class FileSystemGlobbingParserTests
{
    [Test]
    public async Task FileSystemGlobbing()
    {
        string rootDir = Path.Join(Path.GetPathRoot(Environment.CurrentDirectory), "Files to Search");

        string first = Path.Join(rootDir, "first", "first.txt");
        string second = Path.Join(rootDir, "second.txt");
        string third = Path.Join(rootDir, "deep", "deep", "path", "third.txt");
        string forth = Path.Join(rootDir, "forth.doc");

        Microsoft.Extensions.FileSystemGlobbing.InMemoryDirectoryInfo directoryInfo = new(rootDir, [first, second, third, forth]);

        Argument<FileInfo[]> argument = new("FILES") { CustomParser = argumentResult => CommandLine.Parsing.FileSystemGlobbingParser.Parse(argumentResult, directoryInfo) };
        RootCommand root = [argument];
        CommandLineConfiguration configuration = new(root);
        ParseResult parseResult = configuration.Parse("\"" + Path.Combine(rootDir, "**", "*.txt") + "\"");

        _ = await Assert.That(parseResult.GetValue(argument)).IsNotNull().And
            .ContainsOnly(x => x.FullName.Equals(first) || x.FullName.Equals(second) || x.FullName.Equals(third));
    }
}