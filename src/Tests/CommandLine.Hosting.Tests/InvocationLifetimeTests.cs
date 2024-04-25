// -----------------------------------------------------------------------
// <copyright file="InvocationLifetimeTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;

public class InvocationLifetimeTests
{
    [Fact]
    public async Task WithCancellation()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var command = new CliRootCommand();
        command.SetAction(async (_, cancellationToken) =>
        {
            cancellationTokenSource.Cancel();

            // wait for the cancellation token source to be cancelled.
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        });

        var configuration = new CliConfiguration(command);
        configuration.UseHost();

        await configuration.InvokeAsync(string.Empty, cancellationTokenSource.Token);

        cancellationTokenSource.IsCancellationRequested.Should().BeTrue();
    }
}
