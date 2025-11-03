// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.ApplicationHost.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public partial class HostingExtensionsTests
{
#if NET7_0_OR_GREATER
    [Test]
    public async Task GetHostFromApplicationBuilder()
    {
        const bool Value = true;

        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        _ = rootCommand.UseApplicationHost(configureHost: static (_, configure) => configure.Services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value));

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(host?.Services.GetService<Microsoft.Extensions.Hosting.IHostLifetime>())
            .IsTypeOf<InvocationLifetime>().And
            .Member(
                static invocationLifetime => invocationLifetime.Options.SuppressStatusMessages,
                static suppressStatusMessages => suppressStatusMessages.IsEqualTo(Value));
    }

    [Test]
    public async Task GetHostFromApplicationBuilderWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        _ = rootCommand.UseApplicationHost();

        _ = await rootCommand.Parse("[config:Key1=Value1] [config:Key2]").InvokeAsync();
        _ = await Assert.That(host).IsNotNull();

        ConfigurationManager? configurationManager = await Assert.That(host?.Services.GetService(typeof(IConfiguration))).IsTypeOf<ConfigurationManager>();

        _ = await Assert.That(configurationManager!.GetValue<string>("Key1")).IsEqualTo("Value1");
        _ = await Assert.That(configurationManager!.GetValue<string>("Key2")).IsNull();
    }

    [Test]
    public async Task GetHostApplicationLifetime()
    {
        Microsoft.Extensions.Hosting.IHostApplicationLifetime? hostApplicationLifetime = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => hostApplicationLifetime = parseResult.GetServices()?.GetService(typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime)) as Microsoft.Extensions.Hosting.IHostApplicationLifetime);

        _ = rootCommand.UseApplicationHost();

        _ = await rootCommand.Parse([]).InvokeAsync();

        _ = await Assert.That(hostApplicationLifetime).IsNotNull();
    }
#endif
}