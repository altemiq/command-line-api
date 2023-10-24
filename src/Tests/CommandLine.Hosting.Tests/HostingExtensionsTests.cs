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
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => result.GetConfiguration().Should().NotBeNull());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseConfiguration();

        _ = configuration.Invoke(Array.Empty<string>());
    }

    [Fact]
    public void GetServices()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => result.GetServices().Should().NotBeNull());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices();

        _ = configuration.Invoke(Array.Empty<string>());
    }

    [Fact]
    public void GetConfigurationAndServices()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result =>
        {
            _ = result.GetServices().Should().NotBeNull();
            _ = result.GetConfiguration().Should().NotBeNull();
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
    }
}
