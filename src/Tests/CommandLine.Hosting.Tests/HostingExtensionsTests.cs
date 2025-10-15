// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using TUnit.Assertions.Core;

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

        _ = rootCommand.UseHost(configureHost: static (_, builder) => builder.ConfigureServices((_, services) => services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value)));

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(host)
            .IsNotNull().And
            .HasMember(static host => host.Services.GetService<Microsoft.Extensions.Hosting.IHostLifetime>())
            .IsTypeOf<InvocationLifetime, Microsoft.Extensions.Hosting.IHostLifetime?>().And
            .HasMember(static invocationLifetime => invocationLifetime.Options.SuppressStatusMessages).IsEqualTo(Value);
    }

    [Test]
    public async Task GetHostWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        _ = rootCommand.UseHost();

        _ = await rootCommand.Parse("[config:Key1=Value1] [config:Key2]").InvokeAsync();
        _ = await Assert.That(host).IsNotNull();

        ConfigurationRoot? configurationRoot = await Assert.That(host?.Services.GetService(typeof(IConfiguration))).IsTypeOf<ConfigurationRoot>();

        _ = await Assert.That(configurationRoot!.GetValue<string>("Key1")).IsEqualTo("Value1");
        _ = await Assert.That(configurationRoot!.GetValue<string>("Key2")).IsNull();
    }

    [Test]
    public async Task GetConfiguration()
    {
        IConfiguration? config = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => config = result.GetConfiguration());

        _ = rootCommand.UseConfiguration();

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(config).IsNotNull();
    }

    [Test]
    [MatrixDataSource]
    public async Task GetServices([Matrix(true, false)] bool withArgs)
    {
        IServiceProvider? serviceProvider = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        _ = withArgs
            ? rootCommand.UseServices(args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args))
            : rootCommand.UseServices();

        _ = await rootCommand.Parse([]).InvokeAsync();
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


        int count = 0;
        _ = rootCommand.UseServices(() =>
        {
            count++;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null);
        },
        _ => { });

        _ = rootCommand.UseConfiguration(() =>
        {
            count++;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null);
        },
        _ => { });

        _ = await rootCommand.Parse([]).InvokeAsync();
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

        RootCommand rootCommand = [argument];
        _ = rootCommand.UseConfiguration();

        _ = await rootCommand.Parse([]).InvokeAsync();
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

        RootCommand rootCommand = [argument];
        _ = rootCommand.UseConfiguration();

        _ = await rootCommand.Parse([]).InvokeAsync();
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

                RootCommand rootCommand = [];
                _ = rootCommand.UseHost(_ => hostBuilder);

                _ = await rootCommand.Parse([]).InvokeAsync();

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Test]
            public async Task WithSynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                RootCommand rootCommand = [];
                rootCommand.SetAction(_ => { });
                _ = rootCommand.UseHost(_ => hostBuilder);

                _ = await rootCommand.Parse([]).InvokeAsync();

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }

            [Test]
            public async Task WithAsynchronousAction()
            {
                Microsoft.Extensions.Hosting.IHost host = Substitute.For<Microsoft.Extensions.Hosting.IHost>();
                Microsoft.Extensions.Hosting.IHostBuilder hostBuilder = Substitute.For<Microsoft.Extensions.Hosting.IHostBuilder>();
                _ = hostBuilder.Build().Returns(host);

                RootCommand rootCommand = [];
                rootCommand.SetAction((_, _) => Task.CompletedTask);
                _ = rootCommand.UseHost(_ => hostBuilder);

                _ = await rootCommand.Parse([]).InvokeAsync();

                await host.Received().StartAsync(Arg.Any<CancellationToken>());
                await host.Received().StopAsync(Arg.Any<CancellationToken>());
            }
        }
    }
}