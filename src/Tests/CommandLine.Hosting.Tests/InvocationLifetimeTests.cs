// -----------------------------------------------------------------------
// <copyright file="InvocationLifetimeTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class InvocationLifetimeTests
{
    [Test]
    public async Task WithCancellation()
    {
        CancellationTokenSource cancellationTokenSource = new();
        RootCommand command = [];
        command.SetAction(async (_, cancellationToken) =>
        {
            await cancellationTokenSource.CancelAsync();

            // wait for the cancellation token source to be cancelled.
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, CancellationToken.None);
            }
        });

        _ = command.UseHost();

        _ = await command.Parse([]).InvokeAsync(cancellationToken: cancellationTokenSource.Token);

        _ = await Assert.That(cancellationTokenSource.IsCancellationRequested).IsTrue();
    }

    [Test]
    public async Task DoubleDispose()
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();
        _ = builder.ConfigureServices((_, services) => services.AddInvocationLifetime());

        IHost host = await builder.StartAsync();

        IHostLifetime? lifetime = host.Services.GetService<IHostLifetime>();

        IDisposable? disposable = await Assert.That(lifetime).IsAssignableTo<IDisposable>();
        _ = await Assert.That(() => disposable?.Dispose()).ThrowsNothing();
        _ = await Assert.That(() => disposable?.Dispose()).ThrowsNothing();

        await host.StopAsync();
    }
}