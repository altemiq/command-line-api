// -----------------------------------------------------------------------
// <copyright file="DelegateActionTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;


public class DelegateCommandLineActionTests
{
    [Test]
    [Arguments(Action.None)]
    [Arguments(Action.Synchronous)]
    [Arguments(Action.Asynchronous)]
    public async Task SynchronousDelegate(Action action)
    {
        Command command = new Command("command") { new Command("subcommand") }.SetAction(action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, _ => value = true);

        CommandLineConfiguration configuration = new(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = await Assert.That(value).IsTrue();
    }

    [Test]
    [Arguments(Action.None)]
    [Arguments(Action.Asynchronous)]
    public async Task AsynchronousDelegate(Action action)
    {
        Command command = new Command("command") { new Command("subcommand") }.SetAction(action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, (_, _) =>
        {
            value = true;
            return Task.CompletedTask;
        });

        CommandLineConfiguration configuration = new(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = await Assert.That(value).IsTrue();
    }

    [Test]
    [MatrixDataSource]
    public async Task BothDelegates(
        [Matrix(Action.None, Action.Synchronous, Action.Asynchronous)] Action action,
        [Matrix(true, false)] bool preferSynchronous)
    {
        Command command = new Command("command") { new Command("subcommand") }.SetAction(action);

        bool value = default;
        DelegateCommandLineAction.SetHandlers(command, _ => value = true, (_, _) =>
        {
            value = true;
            return Task.CompletedTask;
        }, preferSynchronous);

        CommandLineConfiguration configuration = new(command);
        _ = await configuration.InvokeAsync(string.Empty);

        _ = await Assert.That(value).IsTrue();
    }
}