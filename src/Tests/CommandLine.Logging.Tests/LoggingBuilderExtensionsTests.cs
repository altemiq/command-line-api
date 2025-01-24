// -----------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;
using System.CommandLine;

public class LoggingBuilderExtensionsTests
{
    [Fact]
    public void CreateLogger()
    {
        _ = CreateLoggerFactory().CreateLogger("Program").Should().NotBeNull();
    }

    [Theory]
    [InlineData(LogLevel.Information, LogLevel.Information, 4)]
    [InlineData(LogLevel.Warning, LogLevel.Information, 0)]
    [InlineData(LogLevel.Information, LogLevel.Warning, 4)]
    [InlineData(LogLevel.Debug, LogLevel.Trace, 0)]
    [InlineData(LogLevel.Trace, LogLevel.Debug, 4)]
    [InlineData(LogLevel.None, LogLevel.Warning, 0)]
    public void LogValue(LogLevel minLevel, LogLevel level, int expectedLength)
    {
        StringWriter writer = new();
        CommandLineConfiguration configuration = new(new RootCommand()) { Output = writer };
        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddCommandLineConfiguration(configuration).SetMinimumLevel(minLevel));

        ILogger logger = factory.CreateLogger("Test");
        logger.Log(level, "Test");

        if (expectedLength > 0)
        {
            expectedLength += Environment.NewLine.Length;
        }

        _ = writer.GetStringBuilder().Length.Should().Be(expectedLength);
    }

    private static ILoggerFactory CreateLoggerFactory(CommandLineConfiguration? configuration = default)
    {
        configuration ??= new CommandLineConfiguration(new RootCommand());
        return LoggerFactory.Create(builder => builder.AddCommandLineConfiguration(configuration));
    }
}