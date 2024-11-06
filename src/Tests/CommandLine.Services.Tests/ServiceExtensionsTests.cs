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
        IServiceProvider? serviceProvider = default;
        CliRootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        CliConfiguration configuration = new(rootCommand);
        _ = configuration.UseServices(services => { });

        _ = configuration.Invoke([]);
        _ = serviceProvider.Should().NotBeNull();

    }
}