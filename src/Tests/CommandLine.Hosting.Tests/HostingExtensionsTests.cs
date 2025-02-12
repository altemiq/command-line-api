// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

public partial class HostingExtensionsTests
{
    [Test]
    public async Task GetHost()
    {
        const bool Value = true;

        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseHost(configureHost: (_, builder) => builder.ConfigureServices((_, services) => services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value)));

        _ = configuration.Invoke([]);
        _ = await Assert.That(host).IsAssignableTo<Microsoft.Extensions.Hosting.IHost>().And
            .Satisfies(
                host => host.Services.GetService<Microsoft.Extensions.Hosting.IHostLifetime>(),
                hostLifeTime =>
                {
                    TUnit.Assertions.AssertionBuilders.InvokableValueAssertionBuilder<Microsoft.Extensions.Hosting.IHostLifetime?> assertionBuilder = hostLifeTime.IsNotNull();
                    _ = assertionBuilder.And
                        .IsTypeOf<InvocationLifetime>().And
                        .Satisfies(
                        invocationLifetime => invocationLifetime.Options.SuppressStatusMessages, 
                        suppressStatusMessages => suppressStatusMessages.IsEqualTo(Value));
                    return assertionBuilder;
                });
    }

    [Test]
    public async Task GetHostWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseHost();

        _ = configuration.Invoke("[config:Key1=Value1] [config:Key2]");
        _ = await Assert.That(host).IsNotNull();

        ConfigurationRoot? configurationRoot = await Assert.That(host?.Services.GetService(typeof(IConfiguration))).IsTypeOf<ConfigurationRoot>();

        _ = await Assert.That(configurationRoot!.GetValue<string>("Key1")).IsEqualTo("Value1");
        _ = await Assert.That(configurationRoot!.GetValue<string>("Key2")).IsEqualTo(null);
    }

    [Test]
    public async Task GetConfiguration()
    {
        IConfiguration? config = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => config = result.GetConfiguration());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke([]);
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task GetServices(bool withArgs)
    {
        IServiceProvider? serviceProvider = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = withArgs
            ? configuration.UseServices(args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : configuration.UseServices();

        _ = configuration.Invoke([]);
        _ = await Assert.That(serviceProvider).IsNotNull();
    }

    [Test]
    public async Task GetConfigurationAndServices()
    {
        IServiceProvider? serviceProvider = default;
        IConfiguration? config = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result =>
        {
            serviceProvider = result.GetServices();
            config = result.GetConfiguration();
        });

        CommandLineConfiguration configuration = new(rootCommand);
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
        _ = await Assert.That(count).IsEqualTo(1);
        _ = await Assert.That(serviceProvider).IsNotNull();
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    public async Task UseConfigurationInDefaultValueFromArgument()
    {
        IConfiguration? config = default;
        Argument<string> argument = new("ARG")
        {
            DefaultValueFactory = argumentResult =>
            {
                Command? command = argumentResult.GetCommand();
                if (command is not null && command.Action is { } action)
                {
                    config = action.GetConfiguration();
                }

                return string.Empty;
            }
        };

        CommandLineConfiguration configuration = new(new RootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke([]);
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    public async Task UseConfigurationInDefaultValueFromOption()
    {
        IConfiguration? config = default;
        Option<string> argument = new("--option")
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

        CommandLineConfiguration configuration = new(new RootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(string.Empty);
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    [Arguments("first", true)]
    [Arguments("second", true)]
    [Arguments("first", false)]
    [Arguments("second", false)]
    public async Task UseConfigurationInHelp(string name, bool withArgs)
    {
        IConfiguration? config = default;
        Command? command = default;

        Option<string> argument = new("--option")
        {
            DefaultValueFactory = _ => default!,
            Recursive = true,
        };

        Command firstSubCommand = new("first") { argument };
        firstSubCommand.SetAction(_ => { });
        Command secondSubCommand = new("second") { argument };
        secondSubCommand.SetAction(_ => { });

        RootCommand rootCommand =
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

        CommandLineConfiguration configuration = new(rootCommand);
        _ = withArgs
            ? configuration.UseConfiguration((args) => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : configuration.UseConfiguration();

        _ = configuration.Invoke($"{name} --help");

        _ = await Assert.That(command).IsTypeOf<Command>().And.Satisfies(command => command.Name, commandName => commandName.IsEqualTo(name)!);
        _ = await Assert.That(config).IsNotNull();
    }

    public partial class EnsureStartAndStopAreCalled
    {
        public class Host
        {
            [Test]
            public async Task WithNoAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                RootCommand root = [];
                CommandLineConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Test]
            public async Task WithSynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                RootCommand root = [];
                root.SetAction(parseResult => { });
                CommandLineConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Test]
            public async Task WithAsynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                RootCommand root = [];
                root.SetAction((parseResult, cancellationToken) => Task.CompletedTask);
                CommandLineConfiguration configuration = new(root);
                _ = configuration.UseHost(args => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }
        }
    }
}