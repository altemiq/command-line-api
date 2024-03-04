// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleConfigurationExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleConfigurationExtensionsTests
{
    [Fact]
    public void AddFiglet()
    {
        var console = new TestConsole();

        var configuration = new CliConfiguration(new CliRootCommand()).AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }
}