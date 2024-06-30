// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var verbosityOption = new VerbosityOption();
var root = new CliRootCommand { verbosityOption };
root.SetAction(parseResult =>
{
    var logger = parseResult.CreateLogger<Program>();
    logger.LogTrace("parseResult: Logging Trace");
    logger.LogDebug("parseResult: Logging Debug");
    logger.LogInformation("parseResult: Logging Information");
    logger.LogWarning("parseResult: Logging Warning");
    logger.LogError("parseResult: Logging Error");

    var host = parseResult.GetHost()!;
    logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogTrace("host: Logging Trace");
    logger.LogDebug("host: Logging Debug");
    logger.LogInformation("host: Logging Information");
    logger.LogWarning("host: Logging Warning");
    logger.LogError("host: Logging Error");

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

await configuration.InvokeAsync($"{verbosityOption.Name} {nameof(VerbosityOptions.detailed)}").ConfigureAwait(true);