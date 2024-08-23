// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
#if !NET8_0_OR_GREATER
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.Logging;

var verbosityOption = new VerbosityOption();
var root = new CliRootCommand { verbosityOption };
root.SetAction(parseResult =>
{
    var exception = new InvalidOperationException("This is an exception!");
    var logger = parseResult.CreateLogger<Program>();
    LogTrace(logger, "parseResult", null!);
    LogDebug(logger, "parseResult", null!);
    LogInformation(logger, "parseResult", null!);
    LogWarning(logger, "parseResult", null!);
    LogError(logger, "parseResult", exception);

    var host = parseResult.GetHost()!;
    logger = host.Services.GetRequiredService<ILogger<Program>>();
    LogTrace(logger, "host", null!);
    LogDebug(logger, "host", null!);
    LogInformation(logger, "host", null!);
    LogWarning(logger, "host", null!);
    LogError(logger, "host", exception);

    // do some tasks
    var ansiConsoleProgress = AnsiConsoleProgress.Create<(string Name, double Percentage)>(Spectre.Console.AnsiConsole.Console, x => new AnsiConsoleProgressItem(x.Name, x.Percentage), new AnsiConsoleProgressOptions { UpdateRate = TimeSpan.FromMilliseconds(50) });
    IProgress<(string Name, double Percentage)> progress = ansiConsoleProgress;

    progress.Report(new("Unknown Length Task", double.PositiveInfinity));
    progress.Report(new("Known Length Task", 0));

    for (var i = 1; i <= 100; i++)
    {
        Thread.Sleep(100);
        progress.Report(new("Known Length Task", i));
    }

    progress.Report(("Unknown Length Task", double.NaN));

    while (!ansiConsoleProgress.IsComplete)
    {
        Thread.Sleep(100);
    }
});

var configuration = new CliConfiguration(root);
configuration
#if NET8_0_OR_GREATER
    .UseApplicationHost((parseResult, builder) =>
    {
        if (parseResult is { Configuration: { } configuration })
        {
            builder.Logging.AddCliConfiguration(configuration);
        }
    })
#else
    .UseHost((parseResult, builder) => builder.ConfigureLogging(loggingBuilder =>
    {
        if (parseResult is { Configuration: { } configuration })
        {
            loggingBuilder.AddCliConfiguration(configuration);
        }
    }))
#endif
    .AddLogging((parseResult, configure) =>
    {
        if (parseResult is { Configuration: { } configuration })
        {
            configure.AddCliConfiguration(configuration);
        }
    });

await configuration.InvokeAsync(args).ConfigureAwait(true);

/// <content>
/// The program class.
/// </content>
internal sealed partial class Program
{
#if NET6_0_OR_GREATER
    private Program()
    {
    }

    [LoggerMessage(1, LogLevel.Trace, $"{{Source}}: Logging {nameof(LogLevel.Trace)}")]
    private static partial void LogTrace(ILogger logger, string source, Exception exception);

    [LoggerMessage(2, LogLevel.Debug, $"{{Source}}: Logging {nameof(LogLevel.Debug)}")]
    private static partial void LogDebug(ILogger logger, string source, Exception exception);

    [LoggerMessage(3, LogLevel.Information, $"{{Source}}: Logging {nameof(LogLevel.Information)}")]
    private static partial void LogInformation(ILogger logger, string source, Exception exception);

    [LoggerMessage(4, LogLevel.Warning, $"{{Source}}: Logging {nameof(LogLevel.Warning)}")]
    private static partial void LogWarning(ILogger logger, string source, Exception exception);

    [LoggerMessage(5, LogLevel.Error, $"{{Source}}: Logging {nameof(LogLevel.Error)}")]
    private static partial void LogError(ILogger logger, string source, Exception exception);
#else
    private static readonly Action<ILogger, string, Exception> LogTrace = LoggerMessage.Define<string>(LogLevel.Trace, 1, $"{{Source}}: Logging {nameof(LogLevel.Trace)}");
    private static readonly Action<ILogger, string, Exception> LogDebug = LoggerMessage.Define<string>(LogLevel.Debug, 2, $"{{Source}}: Logging {nameof(LogLevel.Debug)}");
    private static readonly Action<ILogger, string, Exception> LogInformation = LoggerMessage.Define<string>(LogLevel.Information, 3, $"{{Source}}: Logging {nameof(LogLevel.Information)}");
    private static readonly Action<ILogger, string, Exception> LogWarning = LoggerMessage.Define<string>(LogLevel.Warning, 4, $"{{Source}}: Logging {nameof(LogLevel.Warning)}");
    private static readonly Action<ILogger, string, Exception> LogError = LoggerMessage.Define<string>(LogLevel.Error, 5, $"{{Source}}: Logging {nameof(LogLevel.Error)}");

    private Program()
    {
    }
#endif
}