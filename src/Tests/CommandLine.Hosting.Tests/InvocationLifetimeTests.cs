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
    [Fact]
    public async Task WithCancellation()
    {
        CancellationTokenSource cancellationTokenSource = new();
        RootCommand command = [];
        command.SetAction(async (_, cancellationToken) =>
        {
            cancellationTokenSource.Cancel();

            // wait for the cancellation token source to be cancelled.
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, CancellationToken.None);
            }
        });

        CommandLineConfiguration configuration = new(command);
        _ = configuration.UseHost();

        _ = await configuration.InvokeAsync(string.Empty, cancellationTokenSource.Token);

        _ = cancellationTokenSource.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public async Task DoubleDispose()
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();
        _ = builder.ConfigureServices((_, services) => services.AddScoped<IHostLifetime, InvocationLifetime>());

        IHost host = await builder.StartAsync();

        IHostLifetime? lifetime = host.Services.GetService<IHostLifetime>();

        _ = lifetime.Should()
            .BeAssignableTo<IDisposable>()
            .Which.Invoking(d => d.Dispose()).Should().NotThrow()
            .And.Subject.Should().NotThrow();

        await host.StopAsync();
    }
}