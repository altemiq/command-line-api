// -----------------------------------------------------------------------
// <copyright file="ServicesExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public class ServicesExtensionsTests
{
    [Fact]
    public void GetServices()
    {
        IServiceProvider? serviceProvider = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result => serviceProvider = result.GetServices());

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices(configure => { });

        _ = configuration.Invoke(Array.Empty<string>());
        _ = serviceProvider.Should().NotBeNull();
    }

    [Fact]
    public void GetService()
    {
        IDisposable? disposable = default;
        var rootCommand = new CliRootCommand();
        rootCommand.SetAction(result =>
        {
            if (result.GetServices() is { } serviceProvider)
            {
                disposable = (IDisposable?)serviceProvider.GetService(typeof(IDisposable));
            }
        });

        var configuration = new CliConfiguration(rootCommand);
        _ = configuration.UseServices(configure => configure.AddSingleton<IDisposable, Disposable>());

        _ = configuration.Invoke(Array.Empty<string>());
        _ = disposable.Should().NotBeNull();

    }

    private sealed class Disposable : IDisposable
    {
        void IDisposable.Dispose() => throw new NotImplementedException();
    }
}