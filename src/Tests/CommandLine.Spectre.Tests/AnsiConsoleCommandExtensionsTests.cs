// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleCommandExtensionsTests
{
    [Fact]
    public void AddFigletToRoot()
    {
        TestConsole console = new();
        RootCommand command = [];
        _ = command.AddFiglet("value", Color.Blue, console);

        CommandLineConfiguration configuration = new(command);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigletTooEarly()
    {
        Command command = new(nameof(AddFigletTooEarly));
        _ = command.Invoking(c => c.AddFiglet("value", Color.Blue)).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddFigletToSubCommand()
    {
        TestConsole console = new();
        Command command = new(nameof(AddFigletToSubCommand));

        CommandLineConfiguration configuration = new(new RootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse($"{nameof(AddFigletToSubCommand)} --help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigletToSubCommandAndInvokeRoot()
    {
        TestConsole console = new();
        Command command = new(nameof(AddFigletToSubCommandAndInvokeRoot));

        CommandLineConfiguration configuration = new(new RootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().BeEmpty();
    }
}