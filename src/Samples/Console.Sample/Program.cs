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
var progressRateOption = new Option<int>("--progress-rate") { Aliases = { "-p" }, Description = "The progress rate in milliseconds", DefaultValueFactory = _ => 100 };
var root = new RootCommand { verbosityOption, progressRateOption };
root.SetAction(parseResult =>
{
    var exception = new InvalidOperationException("This is an exception!");
    var logger = parseResult.CreateLogger<Program>();
    LogTrace(logger, nameof(parseResult), null!);
    LogDebug(logger, nameof(parseResult), null!);
    LogInformation(logger, nameof(parseResult), null!);
    LogWarning(logger, nameof(parseResult), null!);
    LogError(logger, nameof(parseResult), exception);

    var host = parseResult.GetHost()!;
    logger = host.Services.GetRequiredService<ILogger<Program>>();
    LogTrace(logger, nameof(host), null!);
    LogDebug(logger, nameof(host), null!);
    LogInformation(logger, nameof(host), null!);
    LogWarning(logger, nameof(host), null!);
    LogError(logger, nameof(host), exception);

    // do some tasks
    var ansiConsoleProgress = AnsiConsoleProgress.Create<(string Name, double Percentage)>(Spectre.Console.AnsiConsole.Console, static x => new AnsiConsoleProgressItem(x.Name, x.Percentage), new AnsiConsoleProgressOptions { UpdateRate = TimeSpan.FromMilliseconds(50) });
    IProgress<(string Name, double Percentage)> progress = ansiConsoleProgress;

    var progressRate = parseResult.GetValueOrPrompt(progressRateOption, "Enter the progress rate");

    const string UnknownLengthTask = "Unknown Length Task";
    const string KnownLengthTask = "Known Length Task";

    progress.Report(new(UnknownLengthTask, double.PositiveInfinity));
    progress.Report(new(KnownLengthTask, 0));

    for (var i = 1; i <= 100; i++)
    {
        Thread.Sleep(progressRate);
        progress.Report(new(KnownLengthTask, i));
    }

    progress.Report((UnknownLengthTask, double.NaN));

    while (!ansiConsoleProgress.IsComplete)
    {
        Thread.Sleep(100);
    }
});

var configuration = new CommandLineConfiguration(root);
configuration
#if NET8_0_OR_GREATER
    .UseApplicationHost((parseResult, builder) =>
    {
        if (parseResult is { Configuration: { } parseResultConfiguration })
        {
            builder.Logging.AddCommandLineConfiguration(parseResultConfiguration);
        }
    })
#else
    .UseHost(static (parseResult, builder) => builder.ConfigureLogging(loggingBuilder =>
    {
        if (parseResult is { Configuration: { } configuration })
        {
            loggingBuilder.AddCommandLineConfiguration(configuration);
        }
    }))
#endif
    .AddLogging(static (parseResult, configure) =>
    {
        if (parseResult is { Configuration: { } configuration })
        {
            configure.AddCommandLineConfiguration(configuration);
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