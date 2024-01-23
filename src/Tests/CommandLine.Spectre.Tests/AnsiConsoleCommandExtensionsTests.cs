// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleCommandExtensionsTests
{
    [Fact]
    public void AddFigetToRoot()
    {
        var console = new TestConsole();
        var command = new CliRootCommand();
        _ = command.AddFiglet("value", Color.Blue, console);

        var configuration = new CliConfiguration(command);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigletTooEarly()
    {
        var command = new CliCommand(nameof(AddFigletTooEarly));
        _ = command.Invoking(c => c.AddFiglet("value", Color.Blue)).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddFigetToSubCommand()
    {
        var console = new TestConsole();
        var command = new CliCommand(nameof(AddFigetToSubCommand));

        var configuration = new CliConfiguration(new CliRootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse($"{nameof(AddFigetToSubCommand)} --help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }

    [Fact]
    public void AddFigetToSubCommandAndInvokeRoot()
    {
        var console = new TestConsole();
        var command = new CliCommand(nameof(AddFigetToSubCommandAndInvokeRoot));

        var configuration = new CliConfiguration(new CliRootCommand { command });
        _ = command.AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().BeEmpty();
    }
}