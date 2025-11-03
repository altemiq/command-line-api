// -----------------------------------------------------------------------
// <copyright file="ParseResultExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class ParseResultExtensionsTests
{
    [Test]
    public async Task CreateConsoleWithNoParseResult()
    {
        ParseResult parseResult = default!;
        await Assert.That(parseResult.CreateConsole()).IsSameReferenceAs(global::Spectre.Console.AnsiConsole.Console);
    }

    [Test]
    public async Task CreateDefaultConsole()
    {
        var command = new RootCommand();
        var parseResult = command.Parse(string.Empty);
        await Assert.That(parseResult.CreateConsole())
            .IsNotNull().And
            .Member(static c => c.Profile.Out.Writer, static writer => writer.IsSameReferenceAs(Console.Out));
    }

    [Test]
    public async Task CreateWithOutputOption()
    {
        const string Option = "--output";
        var option = new Option<FileInfo>(Option);
        var command = new RootCommand { option };
        var parseResult = command.Parse(string.Empty);

        await Assert.That(parseResult.CreateConsole(option))
            .IsNotNull().And
            .Member(
                static c => c.Profile.Out,
                static output => output.Member(static x => x.Writer, writer => writer.IsSameReferenceAs(Console.Out)));
    }

    [Test]
    public async Task CreateWithOutputOptionAndValue()
    {
        const string Option = "--output";
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var option = new Option<FileInfo>(Option);
        var command = new RootCommand { option };
        var parseResult = command.Parse($"{Option} \"{temp}\"");

        await Assert.That(parseResult.CreateConsole(option))
            .IsNotNull().And
            .Member(
                static c => ((FileStream)((StreamWriter)c.Profile.Out.Writer).BaseStream).Name,
                name => name.IsEqualTo(temp));
    }

    [Test]
    public async Task CreateWithNonStandardOutput()
    {
        var command = new RootCommand();
        var memoryStream = new MemoryStream();
        var parseResult = command.Parse(string.Empty);
        parseResult.InvocationConfiguration.Output = new StreamWriter(memoryStream);

        await Assert.That(parseResult.CreateConsole())
            .IsNotNull().And
            .Member(
                static c => ((StreamWriter)c.Profile.Out.Writer).BaseStream,
                stream => stream.IsSameReferenceAs(memoryStream));
    }
}