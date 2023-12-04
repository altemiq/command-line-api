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
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => config = result.GetConfiguration());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(Array.Empty<string>());
        _ = config.Should().NotBeNull();
    }

    [Fact]
    public void GetServices()
    {
        IServiceProvider? serviceProvider = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices();

        _ = configuration.Invoke(Array.Empty<string>());
        _ = serviceProvider.Should().NotBeNull();
    }

    [Fact]
    public void GetConfigurationAndServices()
    {
        IServiceProvider? serviceProvider = default;
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result =>
        {
            serviceProvider = result.GetServices();
            config = result.GetConfiguration();
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
        _ = serviceProvider.Should().NotBeNull();
        _ = config.Should().NotBeNull();
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromArgument()
    {
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        var argument = new CliArgument<string>("ARG")
        {
            DefaultValueFactory = argumentResult =>
            {
                var command = argumentResult.GetCommand();
                if (command is not null && command.Action is { } action)
                {
                    config = action.GetConfiguration();
                }

                return string.Empty;
            }
        };

        var configuration = new CliConfiguration(new CliRootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(Array.Empty<string>());
        _ = config.Should().NotBeNull();
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromOption()
    {
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        var argument = new CliOption<string>("--option")
        {
            DefaultValueFactory = argumentResult =>
            {
                if (argumentResult.GetCommand() is { } command)
                {
                    config = command.GetConfiguration();
                }

                return string.Empty;
            },
            Recursive = true,
        };

        var configuration = new CliConfiguration(new CliRootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(string.Empty);
        _ = config.Should().NotBeNull();
    }

    [Theory]
    [InlineData("first")]
    [InlineData("second")]
    public void UseConfigurationInHelp(string name)
    {
        Microsoft.Extensions.Configuration.IConfiguration? config = default;
        CliCommand? command = default;

        var argument = new CliOption<string>("--option")
        {
            DefaultValueFactory = _ => default!,
            Recursive = true,
        };

        var firstSubCommand = new CliCommand("first") { argument };
        firstSubCommand.SetAction(_ => { });
        var secondSubCommand = new CliCommand("second") { argument };
        secondSubCommand.SetAction(_ => { });

        var rootCommand = new CliRootCommand
        {
            firstSubCommand,
            secondSubCommand,
        };

        _ = argument.CustomizeHelp(helpContext =>
        {
            command = helpContext.Command;
            config = command.GetConfiguration();
            return default;
        });

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke($"{name} --help");

        _ = command.Should().NotBeNull().And.Subject.As<CliCommand>().Name.Should().Be(name);
        _ = config.Should().NotBeNull();
    }
}