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
    [Fact]
    public void AddLogging()
    {
        var configuration = new CliConfiguration(new CliRootCommand());
        _ = configuration.AddLogging((parseResult, builder) =>
        {
            if (parseResult?.Configuration is { } configuration)
            {
                _ = builder.AddCliConfiguration(configuration);
            }
        });

        var parseResult = configuration.Parse(string.Empty);
        _ = parseResult.CreateLogger("Test").Should().NotBeNull();
    }

    [Fact]
    public async Task AddLoggingMultipleTimes()
    {
        const int Total = 10;
        int count = default;
        var configuration = new CliConfiguration(new CliRootCommand());

        _ = await Task.WhenAll(Enumerable.Range(0, Total).Select(_ => Task.Run(() => configuration.AddLogging(builder => Interlocked.Increment(ref count)))));

        // force getting the logger
        _ = configuration.Parse(string.Empty).GetLoggerFactory();

        _ = count.Should().Be(Total);
    }

    [Fact]
    public void FailGetLogger() => new CliConfiguration(new CliRootCommand()).Parse(string.Empty).Invoking(pr => pr.GetLoggerFactory()).Should().Throw<InvalidOperationException>();

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
    public void GetLogLevel(VerbosityOptions verbosity, LogLevel level) => new CliConfiguration(new CliRootCommand { CliOptions.VerbosityOption }).Parse($"{CliOptions.VerbosityOption.Name} {verbosity}").GetLogLevel().Should().Be(level);
}