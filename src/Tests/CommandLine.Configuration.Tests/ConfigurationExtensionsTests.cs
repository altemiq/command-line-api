// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Configuration;

public class ConfigurationExtensionsTests
{
    [Fact]
    public void GetConfiguration()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => result.GetConfiguration().Should().NotBeNull());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(Array.Empty<string>());
    }
}
