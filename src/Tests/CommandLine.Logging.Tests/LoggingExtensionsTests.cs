// -----------------------------------------------------------------------
// <copyright file="LoggingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class LoggingExtensionsTests
{
    private readonly CliOption<VerbosityOptions> verbosityOption = new VerbosityOption();

    [Fact]
    public void AddLogging()
    {
        CliConfiguration configuration = new(new CliRootCommand());
        _ = configuration.AddLogging((parseResult, builder) =>
        {
            if (parseResult?.Configuration is { } configuration)
            {
                _ = builder.AddCliConfiguration(configuration);
            }
        });

        ParseResult parseResult = configuration.Parse(string.Empty);
        _ = parseResult.CreateLogger("Test").Should().NotBeNull();
    }

    [Fact]
    public async Task AddLoggingMultipleTimes()
    {
        const int Total = 10;
        int count = default;
        CliConfiguration configuration = new(new CliRootCommand());

        _ = await Task.WhenAll(Enumerable.Range(0, Total).Select(_ => Task.Run(() => configuration.AddLogging(builder => Interlocked.Increment(ref count)))));

        // force getting the logger
        _ = configuration.Parse(string.Empty).GetLoggerFactory();

        _ = count.Should().Be(Total);
    }

    [Fact]
    public void FailGetLogger()
    {
        _ = new CliConfiguration(new CliRootCommand()).Parse(string.Empty).Invoking(pr => pr.GetLoggerFactory()).Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(VerbosityOptions.q, LogLevel.Error)]
    [InlineData(VerbosityOptions.quiet, LogLevel.Error)]
    [InlineData(VerbosityOptions.m, LogLevel.Warning)]
    [InlineData(VerbosityOptions.minimal, LogLevel.Warning)]
    [InlineData(VerbosityOptions.n, LogLevel.Information)]
    [InlineData(VerbosityOptions.normal, LogLevel.Information)]
    [InlineData(VerbosityOptions.d, LogLevel.Debug)]
    [InlineData(VerbosityOptions.detailed, LogLevel.Debug)]
    [InlineData(VerbosityOptions.diag, LogLevel.Trace)]
    [InlineData(VerbosityOptions.diagnostic, LogLevel.Trace)]
    public void GetLogLevel(VerbosityOptions verbosity, LogLevel level)
    {
        _ = new CliConfiguration(new CliRootCommand { verbosityOption }).Parse($"{verbosityOption.Name} {verbosity}").GetLogLevel().Should().Be(level);
    }
}