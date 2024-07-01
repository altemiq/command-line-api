// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.ApplicationHost.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;

public partial class HostingExtensionsTests
{
#if NET7_0_OR_GREATER
    [Fact]
    public void GetHostFromApplicationBuilder()
    {
        const bool Value = true;

        Microsoft.Extensions.Hosting.IHost? host = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseApplicationHost(configureHost: (parseResult, configure) => configure.Services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value));

        _ = configuration.Invoke([]);
        _ = host.Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHost>()
            .Which.Services.GetService(typeof(Microsoft.Extensions.Hosting.IHostLifetime)).Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHostLifetime>()
            .Which.Should().BeOfType<InvocationLifetime>()
            .Which.Options.SuppressStatusMessages.Should().Be(Value);
    }

    [Fact]
    public void GetHostFromApplicationBuilderWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke("[config:Key1=Value1] [config:Key2]");
        _ = host.Should().NotBeNull();

        var configurationManager = host?.Services.GetService(typeof(IConfiguration))
            .Should().BeOfType<ConfigurationManager>().Which;

        configurationManager!.GetValue<string>("Key1").Should().Be("Value1");
        configurationManager!.GetValue<string>("Key2").Should().Be(null);
    }

    [Fact]
    public void GetHostApplicationLifetime()
    {
        Microsoft.Extensions.Hosting.IHostApplicationLifetime? hostApplicationLifetime = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(parseResult => hostApplicationLifetime = parseResult.GetServices()?.GetService(typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime)) as Microsoft.Extensions.Hosting.IHostApplicationLifetime);

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke(string.Empty);

        hostApplicationLifetime.Should().NotBeNull();
    }
#endif
}