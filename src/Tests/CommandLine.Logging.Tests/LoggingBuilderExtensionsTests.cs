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
        RootCommand rootCommand = [];
        var parseResult = rootCommand.Parse([]);
        parseResult.InvocationConfiguration.Output = writer;

        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddCommandLineConfiguration(parseResult.InvocationConfiguration).SetMinimumLevel(minLevel));

        ILogger logger = factory.CreateLogger("Test");
        logger.Log(level, "Test");

        if (expectedLength > 0)
        {
            expectedLength += Environment.NewLine.Length;
        }

        _ = await Assert.That(writer.GetStringBuilder().Length).IsEqualTo(expectedLength);
    }

    private static ILoggerFactory CreateLoggerFactory(RootCommand? rootCommand = default)
    {
        rootCommand ??= [];
        var parseResult = rootCommand.Parse([]);
        return LoggerFactory.Create(builder => builder.AddCommandLineConfiguration(parseResult.InvocationConfiguration));
    }
}