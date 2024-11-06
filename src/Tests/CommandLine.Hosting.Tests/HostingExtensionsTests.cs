// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;
using NSubstitute;

public partial class HostingExtensionsTests
{
    [Fact]
    public void GetHost()
    {
        const bool Value = true;

        Microsoft.Extensions.Hosting.IHost? host = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CliConfiguration configuration = new(rootCommand);
        _ = configuration.UseHost(configureHost: (_, builder) => builder.ConfigureServices((_, services) => services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value)));

        _ = configuration.Invoke([]);
        _ = host.Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHost>()
            .Which.Services.GetService(typeof(Microsoft.Extensions.Hosting.IHostLifetime)).Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHostLifetime>()
            .Which.Should().BeOfType<InvocationLifetime>()
            .Which.Options.SuppressStatusMessages.Should().Be(Value);
    }

    [Fact]
    public void GetHostWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CliConfiguration configuration = new(rootCommand);
        _ = configuration.UseHost();

        _ = configuration.Invoke("[config:Key1=Value1] [config:Key2]");
        _ = host.Should().NotBeNull();

        ConfigurationRoot? configurationRoot = host?.Services.GetService(typeof(IConfiguration))
            .Should().BeOfType<ConfigurationRoot>().Which;

        _ = configurationRoot!.GetValue<string>("Key1").Should().Be("Value1");
        _ = configurationRoot!.GetValue<string>("Key2").Should().Be(null);
    }

    [Fact]
    public void GetConfiguration()
    {
        IConfiguration? config = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(result => config = result.GetConfiguration());

        CliConfiguration configuration = new(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke([]);
        _ = config.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetServices(bool withArgs)
    {
        IServiceProvider? serviceProvider = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        CliConfiguration configuration = new(rootCommand);
        _ = withArgs
            ? configuration.UseServices(args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : configuration.UseServices();

        _ = configuration.Invoke([]);
        _ = serviceProvider.Should().NotBeNull();
    }

    [Fact]
    public void GetConfigurationAndServices()
    {
        IServiceProvider? serviceProvider = default;
        IConfiguration? config = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(result =>
        {
            serviceProvider = result.GetServices();
            config = result.GetConfiguration();
        });

        CliConfiguration configuration = new(rootCommand);
        int count = 0;
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

        _ = configuration.Invoke([]);
        _ = count.Should().Be(1);
        _ = serviceProvider.Should().NotBeNull();
        _ = config.Should().NotBeNull();
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromArgument()
    {
        IConfiguration? config = default;
        CliArgument<string> argument = new("ARG")
        {
            DefaultValueFactory = argumentResult =>
            {
                CliCommand? command = argumentResult.GetCommand();
                if (command is not null && command.Action is { } action)
                {
                    config = action.GetConfiguration();
                }

                return string.Empty;
            }
        };

        CliConfiguration configuration = new(new CliRootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke([]);
        _ = config.Should().NotBeNull();
    }

    [Fact]
    public void UseConfigurationInDefaultValueFromOption()
    {
        IConfiguration? config = default;
        CliOption<string> argument = new("--option")
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

        CliConfiguration configuration = new(new CliRootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(string.Empty);
        _ = config.Should().NotBeNull();
    }

    [Theory]
    [InlineData("first", true)]
    [InlineData("second", true)]
    [InlineData("first", false)]
    [InlineData("second", false)]
    public void UseConfigurationInHelp(string name, bool withArgs)
    {
        IConfiguration? config = default;
        CliCommand? command = default;

        CliOption<string> argument = new("--option")
        {
            DefaultValueFactory = _ => default!,
            Recursive = true,
        };

        CliCommand firstSubCommand = new("first") { argument };
        firstSubCommand.SetAction(_ => { });
        CliCommand secondSubCommand = new("second") { argument };
        secondSubCommand.SetAction(_ => { });

        CliRootCommand rootCommand =
        [
            firstSubCommand,
            secondSubCommand,
        ];

        _ = argument.CustomizeHelp(helpContext =>
        {
            command = helpContext.Command;
            config = command.GetConfiguration();
            return default;
        });

        CliConfiguration configuration = new(rootCommand);
        _ = withArgs
            ? configuration.UseConfiguration((args) => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : configuration.UseConfiguration();

        _ = configuration.Invoke($"{name} --help");

        _ = command.Should().NotBeNull().And.Subject.As<CliCommand>().Name.Should().Be(name);
        _ = config.Should().NotBeNull();
    }

    public partial class EnsureStartAndStopAreCalled
    {
        public class Host
        {
            [Fact]
            public async Task WithNoAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                CliRootCommand root = [];
                CliConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task WithSynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                CliRootCommand root = [];
                root.SetAction(parseResult => { });
                CliConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task WithAsynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                CliRootCommand root = [];
                root.SetAction((parseResult, cancellationToken) => Task.CompletedTask);
                CliConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }
        }
    }
}