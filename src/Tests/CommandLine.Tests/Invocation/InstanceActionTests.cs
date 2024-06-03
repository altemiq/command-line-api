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
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);

        InstanceAction.SetHandlers(command, parseResult => new object());

        var configuration = new CliConfiguration(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceAction.GetInstance<object>(command).Should().NotBeNull();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void InstanceWithSynchronousBeforeAfter(Action action)
    {
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);
        var after = false;
        var before = false;

        InstanceAction.SetHandlers(command, _ => new object(), (_, _) => before = true, (_, _) => after = true);

        var configuration = new CliConfiguration(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceAction.GetInstance<object>(command).Should().NotBeNull();
        _ = before.Should().BeTrue();
        _ = after.Should().BeTrue();
    }

    [Theory]
    [InlineData(Action.None)]
    [InlineData(Action.Synchronous)]
    [InlineData(Action.Asynchronous)]
    public void InstanceWithAsynchronousBeforeAfter(Action action)
    {
        var command = CliCommandExtensions.SetAction(new CliCommand("command") { new CliCommand("subcommand") }, action);
        var after = false;
        var before = false;

        InstanceAction.SetHandlers(command, _ => new object(), (_, _, _) => Task.FromResult(before = true), (_, _, _) => Task.FromResult(after = true));

        var configuration = new CliConfiguration(command);
        _ = configuration.Invoke(string.Empty);

        _ = InstanceAction.GetInstance<object>(command).Should().NotBeNull();
        _ = before.Should().BeTrue();
        _ = after.Should().BeTrue();
    }
}