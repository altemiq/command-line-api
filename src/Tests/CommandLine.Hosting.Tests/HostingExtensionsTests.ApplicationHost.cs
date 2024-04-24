// -----------------------------------------------------------------------
// <copyright file="HostingExtensionsTests.ApplicationHost.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
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
        Microsoft.Extensions.Hosting.IHost? host = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke(Array.Empty<string>());
        _ = host.Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHost>()
            .Which.Services.GetService(typeof(Microsoft.Extensions.Hosting.IHostLifetime)).Should().BeAssignableTo<Microsoft.Extensions.Hosting.IHostLifetime>()
            .Which.Should().BeOfType<InvocationLifetime>();
    }

    [Fact]
    public void GetHostFromApplicationBuilderWithDirectives()
    {
        Microsoft.Extensions.Hosting.IHost? host = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(parseResult => host = parseResult.GetHost());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseApplicationHost();

        _ = configuration.Invoke("[config:Key1=Value1]");
        _ = host.Should().NotBeNull();

        host?.Services.GetService(typeof(IConfiguration))
            .Should().BeOfType<ConfigurationManager>()
            .Which.GetValue<string>("Key1").Should().Be("Value1");
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
