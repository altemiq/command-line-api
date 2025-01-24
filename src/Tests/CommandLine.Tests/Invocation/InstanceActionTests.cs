// -----------------------------------------------------------------------
// <copyright file="InstanceActionTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

public class InstanceActionTests
{
    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void Instance(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);

        InstanceCommandLineAction.SetHandlers(command, parseResult => new object());

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceCommandLineAction.GetInstance<object>(command).Should().NotBeNull();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void InstanceWithSynchronousBeforeAfter(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);
        bool after = false;
        bool before = false;

        InstanceCommandLineAction.SetHandlers(command, _ => new object(), (_, _) => before = true, (_, _) => after = true);

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceCommandLineAction.GetInstance<object>(command).Should().NotBeNull();
        _ = before.Should().BeTrue();
        _ = after.Should().BeTrue();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void InstanceWithAsynchronousBeforeAfter(Action action)
    {
        Command command = CommandExtensions.SetAction(new Command("command") { new Command("subcommand") }, action);
        bool after = false;
        bool before = false;

        InstanceCommandLineAction.SetHandlers(command, _ => new object(), (_, _, _) => Task.FromResult(before = true), (_, _, _) => Task.FromResult(after = true));

        CommandLineConfiguration configuration = new(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceCommandLineAction.GetInstance<object>(command).Should().NotBeNull();
        _ = before.Should().BeTrue();
        _ = after.Should().BeTrue();
    }
}