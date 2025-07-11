﻿// -----------------------------------------------------------------------
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
        _ = configuration.UseHost(configureHost: static (_, builder) => builder.ConfigureServices((_, services) => services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value)));

        _ = await configuration.InvokeAsync([]);
        _ = await Assert.That(host).IsAssignableTo<Microsoft.Extensions.Hosting.IHost>().And
            .Satisfies(
                h => h.Services.GetService<Microsoft.Extensions.Hosting.IHostLifetime>(),
                hostLifeTime => hostLifeTime.IsNotNull().And
                    .IsTypeOf<InvocationLifetime>().And
                    .Satisfies(
                        invocationLifetime => invocationLifetime.Options.SuppressStatusMessages,
                        suppressStatusMessages => suppressStatusMessages.IsEqualTo(Value)).And
                    .IsAssignableTo<Microsoft.Extensions.Hosting.IHostLifetime?>());
    }

    [Test]
    public async Task GetHostWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseHost();

        _ = await configuration.InvokeAsync("[config:Key1=Value1] [config:Key2]");
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

        _ = await configuration.InvokeAsync([]);
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    [MatrixDataSource]
    public async Task GetServices([Matrix(true, false)] bool withArgs)
    {
        IServiceProvider? serviceProvider = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = withArgs
            ? configuration.UseServices(args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : configuration.UseServices();

        _ = await configuration.InvokeAsync([]);
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
        _ => { });

        _ = configuration.UseConfiguration(() =>
        {
            count++;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null);
        },
        _ => { });

        _ = await configuration.InvokeAsync([]);
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
                if (command?.Action is { } action)
                {
                    config = action.GetConfiguration();
                }

                return string.Empty;
            }
        };

        CommandLineConfiguration configuration = new(new RootCommand { argument });
        _ = configuration.UseConfiguration();

        _ = await configuration.InvokeAsync([]);
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

        _ = await configuration.InvokeAsync(string.Empty);
        _ = await Assert.That(config).IsNotNull();
    }

    public static class EnsureStartAndStopAreCalled
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
                _ = configuration.UseHost(_ => hostBuilder);

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
                root.SetAction(_ => { });
                CommandLineConfiguration configuration = new(root);
                _ = configuration.UseHost(_ => hostBuilder);

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
                root.SetAction((_, _) => Task.CompletedTask);
                CommandLineConfiguration configuration = new(root);
                _ = configuration.UseHost(_ => hostBuilder);

                _ = await configuration.InvokeAsync(string.Empty);

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }
        }
    }
}