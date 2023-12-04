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
    /// <param name="func">The asynchronous delegate.</param>
    /// <param name="preferSynchronous">Set to <see langword="true"/> to prefer <paramref name="action"/> when no action is present.</param>
    public static void SetHandlers(
        CliCommand command,
        Action<ParseResult> action,
        Func<ParseResult, CancellationToken, Task> func,
        bool preferSynchronous = false)
    {
        command.Action = command.Action switch
        {
            AsynchronousCliAction asyncAction => new Handlers.DelegateNestedAsynchronousCliAction(func, asyncAction),
            SynchronousCliAction syncAction => new Handlers.DelegateNestedSynchronousCliAction(action, syncAction),
            null when preferSynchronous => new Handlers.DelegateSynchronousCliAction(action),
            null => new Handlers.DelegateAsynchronousCliAction(func),
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
    public static void SetHandlers(CliCommand command, Action<ParseResult> action)
    {
        command.Action = command.Action switch
        {
            AsynchronousCliAction asyncAction => new Handlers.DelegateNestedAsynchronousCliAction((parseResult, cancellationToken) => Task.Run(() => action(parseResult), cancellationToken), asyncAction),
            SynchronousCliAction syncAction => new Handlers.DelegateNestedSynchronousCliAction(action, syncAction),
            null => new Handlers.DelegateSynchronousCliAction(action),
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
    public static void SetHandlers(CliCommand command, Func<ParseResult, CancellationToken, Task> func)
    {
        command.Action = command.Action switch
        {
            AsynchronousCliAction asyncAction => new Handlers.DelegateNestedAsynchronousCliAction(func, asyncAction),
            null => new Handlers.DelegateAsynchronousCliAction(func),
            var a => a,
        };

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, func);
        }
    }

    private static class Handlers
    {
        public sealed class DelegateSynchronousCliAction(Action<ParseResult> action) : SynchronousCliAction
        {
            public override int Invoke(ParseResult parseResult)
            {
                action(parseResult);
                return 0;
            }
        }

        public sealed class DelegateAsynchronousCliAction(Func<ParseResult, CancellationToken, Task> func) : AsynchronousCliAction
        {
            public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
            {
                await func(parseResult, cancellationToken).ConfigureAwait(false);
                return 0;
            }
        }

        public sealed class DelegateNestedAsynchronousCliAction(Func<ParseResult, CancellationToken, Task> func, AsynchronousCliAction cliAction) : NestedAsynchronousCliAction(cliAction)
        {
            public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
            {
                await func(parseResult, cancellationToken).ConfigureAwait(false);
                return await base.InvokeAsync(parseResult, cancellationToken).ConfigureAwait(false);
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
}