// -----------------------------------------------------------------------
// <copyright file="DelegateAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// The delegate action.
/// </summary>
public static class DelegateAction
{
    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="action">The delegate.</param>
    public static void SetHandlers(CliCommand command, Action<ParseResult> action)
    {
        if (command.Action is AsynchronousCliAction asyncAction)
        {
            command.Action = new Handlers.Action.DelegateNestedAsynchronousCliAction(action, asyncAction);
        }
        else if (command.Action is SynchronousCliAction syncAction)
        {
            command.Action = new Handlers.Action.DelegateNestedSynchronousCliAction(action, syncAction);
        }

        command.Action ??= new Handlers.Action.DelegateSynchronousCliAction(action);

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, action);
        }
    }

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="func">The delegate.</param>
    public static void SetHandlers(CliCommand command, Func<ParseResult, Task> func)
    {
        if (command.Action is AsynchronousCliAction asyncAction)
        {
            command.Action = new Handlers.Func.DelegateNestedAsynchronousCliAction(func, asyncAction);
        }

        command.Action ??= new Handlers.Func.DelegateAsynchronousCliAction(func);

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, func);
        }
    }

    private static class Handlers
    {
        public static class Action
        {
            public sealed class DelegateSynchronousCliAction(Action<ParseResult> action) : SynchronousCliAction
            {
                public override int Invoke(ParseResult parseResult)
                {
                    action(parseResult);
                    return 0;
                }
            }

            public sealed class DelegateNestedAsynchronousCliAction(Action<ParseResult> action, AsynchronousCliAction cliAction) : Invocation.NestedAsynchronousCliAction(cliAction)
            {
                public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
                {
                    action(parseResult);
                    return base.InvokeAsync(parseResult, cancellationToken);
                }
            }

            public sealed class DelegateNestedSynchronousCliAction(Action<ParseResult> action, SynchronousCliAction cliAction) : NestedSynchronousCliAction(cliAction)
            {
                public override int Invoke(ParseResult parseResult)
                {
                    action(parseResult);
                    return base.Invoke(parseResult);
                }
            }
        }

        public static class Func
        {
            public sealed class DelegateAsynchronousCliAction(Func<ParseResult, Task> func) : AsynchronousCliAction
            {
                public async override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
                {
                    await func(parseResult).ConfigureAwait(false);
                    return 0;
                }
            }

            public sealed class DelegateNestedAsynchronousCliAction(Func<ParseResult, Task> func, AsynchronousCliAction cliAction) : NestedAsynchronousCliAction(cliAction)
            {
                public async override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
                {
                    await func(parseResult).ConfigureAwait(false);
                    return await base.InvokeAsync(parseResult, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}