// -----------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using System.CommandLine;
using Microsoft.Extensions.Logging;

public class LoggingBuilderExtensionsTests
{
    [Fact]
    public void CreateLogger() => CreateLoggerImpl("Program").Should().NotBeNull();

    [Theory]
    [InlineData(LogLevel.Information, LogLevel.Information, 6)]
    [InlineData(LogLevel.Warning, LogLevel.Information, 0)]
    [InlineData(LogLevel.Information, LogLevel.Warning, 6)]
    [InlineData(LogLevel.Debug, LogLevel.Trace, 0)]
    [InlineData(LogLevel.Trace, LogLevel.Debug, 6)]
    [InlineData(LogLevel.None, LogLevel.Warning, 0)]
    public void LogValue(LogLevel minLevel, LogLevel level, int expectedLength)
    {
        var writer = new StringWriter();
        var configuration = new CliConfiguration(new CliRootCommand()) { Output = writer };
        var factory = LoggerFactory.Create(builder => builder.AddCliConfiguration(configuration).SetMinimumLevel(minLevel));

        var logger = factory.CreateLogger("Test");
        logger.Log(level, "Test");

        _ = writer.GetStringBuilder().Length.Should().Be(expectedLength);
    }

    private static ILoggerFactory CreateLoggerFactory(CliConfiguration? configuration = default)
    {
        configuration ??= new CliConfiguration(new CliRootCommand());
        return LoggerFactory.Create(builder => builder.AddCliConfiguration(configuration));
    }

    private static ILogger CreateLoggerImpl(string name, CliConfiguration? configuration = default) => CreateLoggerFactory(configuration).CreateLogger(name);
}
