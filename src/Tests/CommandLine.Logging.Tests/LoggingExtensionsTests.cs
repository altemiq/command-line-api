﻿// -----------------------------------------------------------------------
// <copyright file="LoggingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;
using TUnit.Assertions.AssertConditions.Throws;

public class LoggingExtensionsTests
{
    private readonly Option<VerbosityOptions> verbosityOption = new VerbosityOption();

    [Test]
    public async Task AddLogging()
    {
        CommandLineConfiguration configuration = new(new RootCommand());
        _ = configuration.AddLogging((parseResult, builder) =>
        {
            if (parseResult?.Configuration is { } parseResultConfiguration)
            {
                _ = builder.AddCommandLineConfiguration(parseResultConfiguration);
            }
        });

        ParseResult parseResult = configuration.Parse(string.Empty);
        _ = await Assert.That(parseResult.CreateLogger("Test")).IsNotNull();
    }

    [Test]
    public async Task GetLoggerFromSubCommand()
    {
        var command = new Command("command");
        var rootCommand = new RootCommand { command };
        var configuration = new CommandLineConfiguration(rootCommand);
        bool configureCalled = false;
        configuration.AddLogging(_ => configureCalled = true);

        var parseResult = configuration.Parse("command");
        _ = await Assert.That(parseResult.CreateLogger("Test")).IsNotNull();
        _ = await Assert.That(configureCalled).IsTrue();
    }

    [Test]
    public async Task AddLoggingMultipleTimes()
    {
        const int Total = 10;
        int count = default;
        CommandLineConfiguration configuration = new(new RootCommand());

        _ = await Task.WhenAll(Enumerable.Range(0, Total).Select(_ => Task.Run(() => configuration.AddLogging(_ => Interlocked.Increment(ref count)))));

        // force getting the logger
        _ = configuration.Parse(string.Empty).GetLoggerFactory();

        _ = await Assert.That(count).IsGreaterThan(1);
    }

    [Test]
    public async Task FailGetLogger()
    {
        _ = await Assert.That(() => new CommandLineConfiguration(new RootCommand()).Parse(string.Empty).GetLoggerFactory()).Throws<InvalidOperationException>();
    }

    [Test]
    [Arguments(VerbosityOptions.q, LogLevel.Error)]
    [Arguments(VerbosityOptions.quiet, LogLevel.Error)]
    [Arguments(VerbosityOptions.m, LogLevel.Warning)]
    [Arguments(VerbosityOptions.minimal, LogLevel.Warning)]
    [Arguments(VerbosityOptions.n, LogLevel.Information)]
    [Arguments(VerbosityOptions.normal, LogLevel.Information)]
    [Arguments(VerbosityOptions.d, LogLevel.Debug)]
    [Arguments(VerbosityOptions.detailed, LogLevel.Debug)]
    [Arguments(VerbosityOptions.diag, LogLevel.Trace)]
    [Arguments(VerbosityOptions.diagnostic, LogLevel.Trace)]
    public async Task GetLogLevel(VerbosityOptions verbosity, LogLevel level)
    {
        _ = await Assert.That(new CommandLineConfiguration(new RootCommand { verbosityOption }).Parse($"{verbosityOption.Name} {verbosity}").GetLogLevel()).IsEqualTo(level);
    }
}