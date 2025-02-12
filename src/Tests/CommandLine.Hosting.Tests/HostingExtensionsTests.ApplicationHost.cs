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

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseApplicationHost(configureHost: (parseResult, configure) => configure.Services.ConfigureInvocationLifetime(opts => opts.SuppressStatusMessages = Value));

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
    public async Task GetHostFromApplicationBuilderWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke("[config:Key1=Value1] [config:Key2]");
        _ = await Assert.That(host).IsNotNull();

        ConfigurationManager? configurationManager = await Assert.That(host?.Services.GetService(typeof(IConfiguration))).IsTypeOf<ConfigurationManager>();

        _ = await Assert.That(configurationManager!.GetValue<string>("Key1")).IsEqualTo("Value1");
        _ = await Assert.That(configurationManager!.GetValue<string>("Key2")).IsEqualTo(null);
    }

    [Test]
    public async Task GetHostApplicationLifetime()
    {
        Microsoft.Extensions.Hosting.IHostApplicationLifetime? hostApplicationLifetime = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(parseResult => hostApplicationLifetime = parseResult.GetServices()?.GetService(typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime)) as Microsoft.Extensions.Hosting.IHostApplicationLifetime);

        CommandLineConfiguration configuration = new(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke(string.Empty);

        _ = await Assert.That(hostApplicationLifetime).IsNotNull();
    }
#endif
}