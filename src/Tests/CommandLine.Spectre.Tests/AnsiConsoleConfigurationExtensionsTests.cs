// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleConfigurationExtensionsTests
{
    [Fact]
    public void AddFiget()
    {
        var console = new TestConsole();

        var configuration = new CliConfiguration(new CliRootCommand()).AddFiglet("value", Color.Blue, console);

        configuration.Parse("--help").Invoke();

        console.Lines.Skip(1).Should().NotBeEmpty();
    }
}
