// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Configuration;

public class ConfigurationExtensionsTests
{
    [Test]
    public async Task GetConfiguration()
    {
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => config = result.GetConfiguration());

        ParseResult? parseResult = default;
        Microsoft.Extensions.Configuration.IConfigurationBuilder? builder = default;
        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseConfiguration((parseResult_, builder_) =>
        {
            parseResult = parseResult_;
            builder = builder_;
        });

        _ = configuration.Invoke([]);
        _ = await Assert.That(config).IsNotNull();
        _ = await Assert.That(parseResult).IsNotNull();
        _ = await Assert.That(builder).IsNotNull();
    }
}