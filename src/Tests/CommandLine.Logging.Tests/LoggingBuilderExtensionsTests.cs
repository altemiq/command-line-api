// -----------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;

public class LoggingBuilderExtensionsTests
{
    [Test]
    public async Task CreateLogger()
    {
        _ = await Assert.That(CreateLoggerFactory().CreateLogger("Program")).IsNotNull();
    }

    [Test]
    [Arguments(LogLevel.Information, LogLevel.Information, 4)]
    [Arguments(LogLevel.Warning, LogLevel.Information, 0)]
    [Arguments(LogLevel.Information, LogLevel.Warning, 4)]
    [Arguments(LogLevel.Debug, LogLevel.Trace, 0)]
    [Arguments(LogLevel.Trace, LogLevel.Debug, 4)]
    [Arguments(LogLevel.None, LogLevel.Warning, 0)]
    public async Task LogValue(LogLevel minLevel, LogLevel level, int expectedLength)
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

        _ = await Assert.That(writer.GetStringBuilder().Length).IsEqualTo(expectedLength);
    }

    private static ILoggerFactory CreateLoggerFactory(CommandLineConfiguration? configuration = default)
    {
        configuration ??= new CommandLineConfiguration(new RootCommand());
        return LoggerFactory.Create(builder => builder.AddCommandLineConfiguration(configuration));
    }
}