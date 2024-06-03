// -----------------------------------------------------------------------
// <copyright file="InvocationLifetimeTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;
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
                await Task.Delay(1000, CancellationToken.None);
            }
        });

        var configuration = new CliConfiguration(command);
        _ = configuration.UseHost();

        _ = await configuration.InvokeAsync(string.Empty, cancellationTokenSource.Token);

        _ = cancellationTokenSource.IsCancellationRequested.Should().BeTrue();
    }
}
