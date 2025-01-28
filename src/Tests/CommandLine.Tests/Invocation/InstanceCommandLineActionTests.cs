// -----------------------------------------------------------------------
// <copyright file="InstanceActionTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

public class InstanceCommandLineActionTests
{
    [Test]
    [Arguments(Action.None)]
    [Arguments(Action.Synchronous)]
    [Arguments(Action.Asynchronous)]
    public async Task Instance(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);

        InstanceCommandLineAction.SetHandlers(command, parseResult => new object());

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = await Assert.That(InstanceCommandLineAction.GetInstance<object>(command)).IsNotNull();
    }

    [Test]
    [Arguments(Action.None)]
    [Arguments(Action.Synchronous)]
    [Arguments(Action.Asynchronous)]
    public async Task InstanceWithSynchronousBeforeAfter(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);
        bool after = false;
        bool before = false;

        InstanceCommandLineAction.SetHandlers(command, _ => new object(), (_, _) => before = true, (_, _) => after = true);

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = await Assert.That(InstanceCommandLineAction.GetInstance<object>(command)).IsNotNull();
        _ = await Assert.That(before).IsTrue();
        _ = await Assert.That(after).IsTrue();
    }

    [Test]
    [Arguments(Action.None)]
    [Arguments(Action.Synchronous)]
    [Arguments(Action.Asynchronous)]
    public async Task InstanceWithAsynchronousBeforeAfter(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);
        bool after = false;
        bool before = false;

        InstanceCommandLineAction.SetHandlers(command, _ => new object(), (_, _, _) => Task.FromResult(before = true), (_, _, _) => Task.FromResult(after = true));

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = await Assert.That(InstanceCommandLineAction.GetInstance<object>(command)).IsNotNull();
        _ = await Assert.That(before).IsTrue();
        _ = await Assert.That(after).IsTrue();
    }
}