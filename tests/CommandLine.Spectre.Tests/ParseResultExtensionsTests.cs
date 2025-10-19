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
            .HasMember(c => c.Profile.Out.Writer).IsSameReferenceAs(Console.Out);
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
            .HasMember(c => c.Profile.Out).IsNotNull().And
            .HasMember(x => x.Writer).IsSameReferenceAs(Console.Out);
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
            .HasMember(c => c.Profile.Out).IsNotNull().And
            .HasMember(x => x.Writer as StreamWriter).IsNotNull().And
            .HasMember(x => x!.BaseStream as FileStream).IsNotNull().And
            .HasMember(x => x!.Name).IsEqualTo(temp);
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
            .HasMember(c => c.Profile.Out).IsNotNull().And
            .HasMember(x => x.Writer as StreamWriter).IsNotNull().And
            .HasMember(x => x!.BaseStream).IsSameReferenceAs(memoryStream);

    }
}