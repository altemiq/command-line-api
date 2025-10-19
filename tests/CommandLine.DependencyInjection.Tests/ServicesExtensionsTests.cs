// -----------------------------------------------------------------------
// <copyright file="ServicesExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public class ServicesExtensionsTests
{
    [Test]
    public async Task GetServices()
    {
        IServiceProvider? serviceProvider = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        _ = rootCommand.UseServices(static _ => { });

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(serviceProvider).IsNotNull();
    }

    [Test]
    public async Task GetService()
    {
        IDisposable? disposable = default;
        RootCommand rootCommand = [];
        rootCommand.SetAction(result =>
        {
            if (result.GetServices() is { } serviceProvider)
            {
                disposable = (IDisposable?)serviceProvider.GetService(typeof(IDisposable));
            }
        });

        _ = rootCommand.UseServices(static configure => configure.AddSingleton<IDisposable, Disposable>());

        _ = await rootCommand.Parse([]).InvokeAsync();
        _ = await Assert.That(disposable).IsNotNull();

    }

    private sealed class Disposable : IDisposable
    {
        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}