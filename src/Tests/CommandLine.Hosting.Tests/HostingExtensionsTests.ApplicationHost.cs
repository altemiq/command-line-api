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
        await Assert.That(host).IsAssignableTo<Microsoft.Extensions.Hosting.IHost>().And
            .Satisfies(
                h => h.Services.GetService(typeof(Microsoft.Extensions.Hosting.IHostLifetime)),
                hl => hl.IsAssignableTo<Microsoft.Extensions.Hosting.IHostLifetime>().And
                    .IsTypeOf<InvocationLifetime>().And
                    .Satisfies(
                        o => o.Options.SuppressStatusMessages,
                        suppressStatusMessages => suppressStatusMessages.IsEqualTo(Value)).And
                    .IsNotNull<object?>());
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
        await Assert.That( host).IsNotNull();

        var configurationManager = await Assert.That(host?.Services.GetService(typeof(IConfiguration))).IsTypeOf<ConfigurationManager>();

        await Assert.That(configurationManager!.GetValue<string>("Key1")).IsEqualTo("Value1");
        await Assert.That(configurationManager!.GetValue<string>("Key2")).IsEqualTo(null);
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

        await Assert.That(hostApplicationLifetime).IsNotNull();
    }
#endif
}