// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

using TUnit.Assertions.AssertConditions.Throws;

public class AnsiConsoleCommandExtensionsTests
{
    [Test]
    public async Task AddFigletToRoot()
    {
        TestConsole console = new();
        RootCommand command = [];
        _ = command.AddFiglet("value", Color.Blue, console);

        CommandLineConfiguration configuration = new(command);

        _ = await configuration.Parse("--help").InvokeAsync();

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

        CommandLineConfiguration configuration = new(new RootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = await configuration.Parse($"{nameof(AddFigletToSubCommand)} --help").InvokeAsync();

        _ = await Assert.That(console.Lines.Skip(1)).IsNotEmpty();
    }

    [Test]
    public async Task AddFigletToSubCommandAndInvokeRoot()
    {
        TestConsole console = new();
        Command command = new(nameof(AddFigletToSubCommandAndInvokeRoot));

        CommandLineConfiguration configuration = new(new RootCommand { command});
        var helpOption = configuration.RootCommand.Options.OfType<Help.HelpOption>().Single();
        helpOption.Action = new NullAction();
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = await configuration.Parse("--help").InvokeAsync();

        _ = await Assert.That(console.Lines.Skip(1)).IsEmpty();
    }

    private sealed class NullAction : Invocation.SynchronousCommandLineAction
    {
        public override int Invoke(ParseResult parseResult) => default;
    }
}