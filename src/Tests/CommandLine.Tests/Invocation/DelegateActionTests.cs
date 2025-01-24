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
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, parseResult => value = true);

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = value.Should().BeTrue();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Asynchronous)]
    public async Task AsynchronousDelegate(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, (parseResult, cancellationToken) =>
        {
            value = true;
            return Task.CompletedTask;
        });

        CommandLineConfiguration configuration = new(command);
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
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, _ => value = true, (parseResult, cancellationToken) =>
        {
            value = true;
            return Task.CompletedTask;
        }, preferSynchronous);

        CommandLineConfiguration configuration = new(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = value.Should().BeTrue();
    }
}