// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var root = new CliRootCommand { CliOptions.VerbosityOption };
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
});

var configuration = new CliConfiguration(root);
configuration
    .UseApplicationHost((parseResult, builder) =>
    {
        if (parseResult is not null)
        {
            builder.Logging.AddCliConfiguration(parseResult.Configuration);
        }
    })
    .AddLogging((parseResult, configure) =>
    {
        if (parseResult is not null)
        {
            configure.AddCliConfiguration(parseResult.Configuration);
        }
    });

await configuration.InvokeAsync($"{CliOptions.VerbosityOption.Name} {nameof(VerbosityOptions.detailed)}").ConfigureAwait(true);