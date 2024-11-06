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
        CliRootCommand command = [];
        _ = command.AddFiglet("value", Color.Blue, console);

        CliConfiguration configuration = new(command);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigletTooEarly()
    {
        CliCommand command = new(nameof(AddFigletTooEarly));
        _ = command.Invoking(c => c.AddFiglet("value", Color.Blue)).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddFigletToSubCommand()
    {
        TestConsole console = new();
        CliCommand command = new(nameof(AddFigletToSubCommand));

        CliConfiguration configuration = new(new CliRootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse($"{nameof(AddFigletToSubCommand)} --help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigletToSubCommandAndInvokeRoot()
    {
        TestConsole console = new();
        CliCommand command = new(nameof(AddFigletToSubCommandAndInvokeRoot));

        CliConfiguration configuration = new(new CliRootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().BeEmpty();
    }
}