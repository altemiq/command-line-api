// -----------------------------------------------------------------------
// <copyright file="ServiceExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Services;

public class ServiceExtensionsTests
{
    [Test]
    public async Task GetServices()
    {
        IServiceProvider? serviceProvider = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        _ = rootCommand.UseServices(_ => { });

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(serviceProvider).IsNotNull();

    }
}