// -----------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;

public class LoggingExtensions
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
}
