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
        var configuration = new CommandLineConfiguration(command);
        var parseResult = configuration.Parse(string.Empty);
        await Assert.That(parseResult.CreateConsole())
            .IsNotNull().And
            .Satisfies(c => c.Profile.Out.Writer, @out => @out.IsSameReferenceAs(Console.Out));
    }

    [Test]
    public async Task CreateWithOutputOption()
    {
        const string Option = "--output";
        var option = new Option<FileInfo>(Option);
        var command = new RootCommand { option };
        var configuration = new CommandLineConfiguration(command);
        var parseResult = configuration.Parse(string.Empty);

        await Assert.That(parseResult.CreateConsole(option))
            .IsNotNull().And
            .Satisfies(
                c => c.Profile.Out,
                @out => @out
                    .IsAssignableTo<IAnsiConsoleOutput>().And
                    .Satisfies(
                        x => x.Writer,
                        writer => writer.IsSameReferenceAs(Console.Out)).And
                    .IsAssignableTo<IAnsiConsoleOutput?>());
    }

    [Test]
    public async Task CreateWithOutputOptionAndValue()
    {
        const string Option = "--output";
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var option = new Option<FileInfo>(Option);
        var command = new RootCommand { option };
        var configuration = new CommandLineConfiguration(command);
        var parseResult = configuration.Parse($"{Option} \"{temp}\"");

        await Assert.That(parseResult.CreateConsole(option))
            .IsNotNull().And
            .Satisfies(
                c => c.Profile.Out,
                @out => @out
                    .IsAssignableTo<IAnsiConsoleOutput>().And
                    .Satisfies(
                        x => x.Writer,
                        writer => writer
                            .IsTypeOf<StreamWriter>().And
                            .Satisfies(
                                x => x.BaseStream,
                                baseStream => baseStream
                                    .IsTypeOf<FileStream>().And
                                    .Satisfies(x => x.Name, name => name.IsEqualTo(temp).And.IsTypeOf<string?>()).And
                                    .IsAssignableTo<Stream?>()).And
                        .IsAssignableTo<TextWriter?>()).And
                    .IsAssignableTo<IAnsiConsoleOutput?>());
    }

    [Test]
    public async Task CreateWithNonStandardOutput()
    {
        var command = new RootCommand();
        using var memoryStream = new MemoryStream();
        var configuration = new CommandLineConfiguration(command) { Output = new StreamWriter(memoryStream) };
        var parseResult = configuration.Parse(string.Empty);

        await Assert.That(parseResult.CreateConsole())
            .IsNotNull().And
            .Satisfies(
                c => c.Profile.Capabilities.Ansi,
                ansi => ansi.IsFalse());
    }
}
