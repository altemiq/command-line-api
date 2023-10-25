// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

public class HostingExtensionsTests
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

    [Fact]
    public void GetServices()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => result.GetServices().Should().NotBeNull());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices();

        _ = configuration.Invoke(Array.Empty<string>());
    }

    [Fact]
    public void GetConfigurationAndServices()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result =>
        {
            _ = result.GetServices().Should().NotBeNull();
            _ = result.GetConfiguration().Should().NotBeNull();
        });

        var configuration = new CliConfiguration(rootCommand);
        var count = 0;
        _ = configuration.UseServices(() =>
        {
            count++;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null);
        },
        configure => { });

        _ = configuration.UseConfiguration(() =>
        {
            count++;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null);
        },
        configure => { });

        _ = configuration.Invoke(Array.Empty<string>());
        _ = count.Should().Be(1);
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromArgument()
    {
        var argument = new CliArgument<string>("ARG")
        {
            DefaultValueFactory = argumentResult =>
            {
                var command = argumentResult.GetCommand();
                if (command is not null && command.Action is { } action)
                {
                    config = action.GetConfiguration();
                }

                config.Should().NotBeNull();
                return string.Empty;
            }
        };
        var rootCommand = new CliRootCommand
        {
            argument,
        };

        rootCommand.SetAction(_ => { });

        var configuration = new CliConfiguration(rootCommand);
        configuration.UseConfiguration();

        _ = configuration.Invoke(Array.Empty<string>());
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromOption()
    {
        var argument = new CliOption<string>("--option")
        {
            DefaultValueFactory = argumentResult =>
            {
                Microsoft.Extensions.Configuration.IConfiguration? config = default;
                if (argumentResult.GetCommand() is { } command)
                {
                    config = command.GetConfiguration();
                }

                config.Should().NotBeNull();
                return string.Empty;
            },
            Recursive = true,
        };

        var rootCommand = new CliRootCommand
        {
            argument,
        };

        rootCommand.SetAction(_ => { });

        var configuration = new CliConfiguration(rootCommand);
        configuration.UseConfiguration();

        _ = configuration.Invoke("--help");
    }
}
