// -----------------------------------------------------------------------
// <copyright file="DelegateCommandLineAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// The delegate action.
/// </summary>
public static class DelegateCommandLineAction
{
    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="action">The delegate.</param>
    /// <param name="func">The asynchronous delegate.</param>
    /// <param name="preferSynchronous">Set to <see langword="true"/> to prefer <paramref name="action"/> when no action is present.</param>
    public static void SetHandlers(
        Command command,
        Action<ParseResult> action,
        Func<ParseResult, CancellationToken, Task> func,
        bool preferSynchronous = false)
    {
        command.Action = command.Action switch
        {
            AsynchronousCommandLineAction asyncAction => new Handlers.DelegateNestedAsynchronousCommandLineAction(func, asyncAction),
            SynchronousCommandLineAction syncAction => new Handlers.DelegateNestedSynchronousCommandLineAction(action, syncAction),
            null when preferSynchronous => new Handlers.DelegateSynchronousCommandLineAction(action),
            null => new Handlers.DelegateAsynchronousCommandLineAction(func),
            var a => a,
        };

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, action);
        }
    }

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="action">The delegate.</param>
    public static void SetHandlers(Command command, Action<ParseResult> action)
    {
        command.Action = command.Action switch
        {
            AsynchronousCommandLineAction asyncAction => new Handlers.DelegateNestedAsynchronousCommandLineAction((parseResult, cancellationToken) => Task.Run(() => action(parseResult), cancellationToken), asyncAction),
            SynchronousCommandLineAction syncAction => new Handlers.DelegateNestedSynchronousCommandLineAction(action, syncAction),
            null => new Handlers.DelegateSynchronousCommandLineAction(action),
            var a => a,
        };

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
    public static void SetHandlers(Command command, Func<ParseResult, CancellationToken, Task> func)
    {
        command.Action = command.Action switch
        {
            AsynchronousCommandLineAction asyncAction => new Handlers.DelegateNestedAsynchronousCommandLineAction(func, asyncAction),
            null => new Handlers.DelegateAsynchronousCommandLineAction(func),
            var a => a,
        };

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, func);
        }
    }

    private static class Handlers
    {
        public sealed class DelegateSynchronousCommandLineAction(Action<ParseResult> action) : SynchronousCommandLineAction
        {
            public override int Invoke(ParseResult parseResult)
            {
                action(parseResult);
                return 0;
            }
        }

        public sealed class DelegateAsynchronousCommandLineAction(Func<ParseResult, CancellationToken, Task> func) : AsynchronousCommandLineAction
        {
            public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
            {
                await func(parseResult, cancellationToken).ConfigureAwait(false);
                return 0;
            }
        }

        public sealed class DelegateNestedAsynchronousCommandLineAction(Func<ParseResult, CancellationToken, Task> func, AsynchronousCommandLineAction action) : NestedAsynchronousCommandLineAction(action)
        {
            public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
            {
                await func(parseResult, cancellationToken).ConfigureAwait(false);
                return await base.InvokeAsync(parseResult, cancellationToken).ConfigureAwait(false);
            }
        }

        public sealed class DelegateNestedSynchronousCommandLineAction(Action<ParseResult> action, SynchronousCommandLineAction commandLineAction) : NestedSynchronousCommandLineAction(commandLineAction)
        {
            public override int Invoke(ParseResult parseResult)
            {
                action(parseResult);
                return base.Invoke(parseResult);
            }
        }
    }
}