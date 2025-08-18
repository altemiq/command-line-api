// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleCommandExtensionsTests
{
    [Test]
    public async Task AddFigletToRoot()
    {
        TestConsole console = new();
        RootCommand command = [];
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = await command.Parse("--help").InvokeAsync();

        _ = await Assert.That(console.Lines.Skip(1)).IsNotEmpty();
    }

    [Test]
    public async Task AddFigletTooEarly()
    {
        Command command = new(nameof(AddFigletTooEarly));
        _ = await Assert.That(() => command.AddFiglet("value", Color.Blue)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddFigletToSubCommand()
    {
        TestConsole console = new();
        Command command = new(nameof(AddFigletToSubCommand));

        _ = command.AddFiglet("value", Color.Blue, console);

        _ = await command.Parse($"{nameof(AddFigletToSubCommand)} --help").InvokeAsync();

        _ = await Assert.That(console.Lines.Skip(1)).IsNotEmpty();
    }

    [Test]
    public async Task AddFigletToSubCommandAndInvokeRoot()
    {
        TestConsole console = new();
        Command command = new(nameof(AddFigletToSubCommandAndInvokeRoot));

        RootCommand rootCommand = [command];
        var helpOption = rootCommand.Options.OfType<Help.HelpOption>().Single();
        helpOption.Action = new NullAction();
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = await rootCommand.Parse("--help").InvokeAsync();

        _ = await Assert.That(console.Lines.Skip(1)).IsEmpty();
    }

    private sealed class NullAction : Invocation.SynchronousCommandLineAction
    {
        public override int Invoke(ParseResult parseResult) => default;
    }
}