// -----------------------------------------------------------------------
// <copyright file="DelegateActionTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;


public class DelegateActionTests
{
    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void SynchronousDelegate(Action action)
    {
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);

        bool value = default;
        DelegateAction.SetHandlers(command, parseResult => value = true);

        var configuration = new CliConfiguration(command);
        _ = configuration.Invoke(string.Empty);

        _ = value.Should().BeTrue();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Asynchronous)]
    public async Task AsynchronousDelegate(Action action)
    {
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);

        bool value = default;
        DelegateAction.SetHandlers(command, (parseResult, cancellationToken) =>
        {
            value = true;
            return Task.CompletedTask;
        });

        var configuration = new CliConfiguration(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = value.Should().BeTrue();
    }

    [Theory]
    [InlineData(Action.None, true)]
    [InlineData(Action.Synchronous, true)]
    [InlineData(Action.Asynchronous, true)]
    [InlineData(Action.None, false)]
    [InlineData(Action.Synchronous, false)]
    [InlineData(Action.Asynchronous, false)]
    public async Task BothDelegates(Action action, bool preferSynchronous)
    {
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);

        bool value = default;
        DelegateAction.SetHandlers(command, _ => value = true, (parseResult, cancellationToken) =>
        {
            value = true;
            return Task.CompletedTask;
        }, preferSynchronous);

        var configuration = new CliConfiguration(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = value.Should().BeTrue();
    }
}