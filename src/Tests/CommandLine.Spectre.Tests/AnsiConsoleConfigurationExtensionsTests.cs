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
        TestConsole console = new();

        CommandLineConfiguration configuration = new CommandLineConfiguration(new RootCommand()).AddFiglet("value", Color.Blue, console);

        _ = configuration.Parse("--help").Invoke();

        _ = console.Lines.Skip(1).Should().NotBeEmpty();
    }
}