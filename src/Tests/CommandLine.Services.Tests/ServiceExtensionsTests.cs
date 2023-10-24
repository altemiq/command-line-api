// -----------------------------------------------------------------------
// <copyright file="ServiceExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Services;

public class ServiceExtensionsTests
{
    [Fact]
    public void GetServices()
    {
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => result.GetServices().Should().NotBeNull());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices(services => { });

        _ = configuration.Invoke(Array.Empty<string>());

    }
}
