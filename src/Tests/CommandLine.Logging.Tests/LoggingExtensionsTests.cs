// -----------------------------------------------------------------------
// <copyright file="LoggingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;

public class LoggingExtensionsTests
{
    private readonly Option<VerbosityOptions> verbosityOption = new VerbosityOption();

    [Test]
    public async Task AddLogging()
    {
        RootCommand configuration = [];
        _ = configuration.AddLogging((parseResult, builder) =>
        {
            if (parseResult?.InvocationConfiguration is { } invocationConfiguration)
            {
                _ = builder.AddCommandLineConfiguration(invocationConfiguration);
            }
        });

        ParseResult parseResult = configuration.Parse(string.Empty);
        _ = await Assert.That(parseResult.CreateLogger("Test")).IsNotNull();
    }

    [Test]
    public async Task GetLoggerFromSubCommand()
    {
        const string CommandName = "command";
        var rootCommand = new RootCommand { new Command(CommandName) };
        bool configureCalled = false;
        rootCommand.AddLogging(_ => configureCalled = true);

        var parseResult = rootCommand.Parse(CommandName);
        _ = await Assert.That(parseResult.CreateLogger("Test")).IsNotNull();
        _ = await Assert.That(configureCalled).IsTrue();
    }

    [Test]
    public async Task AddLoggingMultipleTimes()
    {
        const int Total = 10;
        int count = default;
        RootCommand command = [];

        _ = await Task.WhenAll(Enumerable.Range(0, Total).Select(_ => Task.Run(() => command.AddLogging(_ => Interlocked.Increment(ref count)))));

        // force getting the logger
        _ = command.Parse([]).GetLoggerFactory();

        _ = await Assert.That(count).IsGreaterThan(1);
    }

    [Test]
    public async Task FailGetLogger()
    {
        _ = await Assert.That(() => new RootCommand().Parse([]).GetLoggerFactory()).Throws<InvalidOperationException>();
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
        _ = await Assert.That(new RootCommand { verbosityOption }.Parse($"{verbosityOption.Name} {verbosity}").GetLogLevel()).IsEqualTo(level);
    }
}