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
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => config = result.GetConfiguration());

        ParseResult? parseResult = default;
        Microsoft.Extensions.Configuration.IConfigurationBuilder? builder = default;
        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseConfiguration((parseResult_, builder_) =>
        {
            parseResult = parseResult_;
            builder = builder_;
        });

        _ = configuration.Invoke(Array.Empty<string>());
        _ = config.Should().NotBeNull();
        parseResult.Should().NotBeNull();
        builder.Should().NotBeNull();
    }
}
