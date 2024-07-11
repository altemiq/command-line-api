// -----------------------------------------------------------------------
// <copyright file="InstanceAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// The instance action.
/// </summary>
public static class InstanceAction
{
#pragma warning disable SA1600 // Elements should be documented
    private interface IInstance<out T>
    {
        T Get(ParseResult? parseResult);
    }
#pragma warning restore SA1600 // Elements should be documented

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="buildInstance">The build the instance.</param>
    public static void SetHandlers<TInstance>(CliCommand command, Func<ParseResult, TInstance> buildInstance)
    {
        SetHandlersCore(command, buildInstance);

        static void SetHandlersCore(CliCommand command, Func<ParseResult, TInstance> buildInstance)
        {
            command.Action = command.Action switch
            {
                AsynchronousCliAction asyncAction => new InstanceNestedAsynchronousCliAction<TInstance>(buildInstance, asyncAction, Empty<TInstance>.Task, Empty<TInstance>.Task),
                SynchronousCliAction syncAction => new InstanceNestedSynchronousCliAction<TInstance>(buildInstance, syncAction, Empty<TInstance>.Action, Empty<TInstance>.Action),
                null => new InstanceSynchronousCliAction<TInstance>(buildInstance, Empty<TInstance>.Action, Empty<TInstance>.Action),
                var a => a,
            };

            foreach (var subCommand in command.Subcommands)
            {
                SetHandlersCore(subCommand, buildInstance);
            }
        }
    }

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public static void SetHandlers<TInstance>(CliCommand command, Func<ParseResult, TInstance> buildInstance, Action<ParseResult, TInstance> beforeInvoke, Action<ParseResult, TInstance> afterInvoke)
    {
        SetHandlersCore(command, buildInstance, beforeInvoke, afterInvoke);

        static void SetHandlersCore(CliCommand command, Func<ParseResult, TInstance> buildInstance, Action<ParseResult, TInstance> beforeInvoke, Action<ParseResult, TInstance> afterInvoke)
        {
            command.Action = command.Action switch
            {
                AsynchronousCliAction asyncAction => new InstanceNestedAsynchronousCliAction<TInstance>(buildInstance, asyncAction, InvokeAsync(beforeInvoke), InvokeAsync(afterInvoke)),
                SynchronousCliAction syncAction => new InstanceNestedSynchronousCliAction<TInstance>(buildInstance, syncAction, beforeInvoke, afterInvoke),
                null => new InstanceSynchronousCliAction<TInstance>(buildInstance, beforeInvoke, afterInvoke),
                var a => a,
            };

            foreach (var subCommand in command.Subcommands)
            {
                SetHandlersCore(subCommand, buildInstance, beforeInvoke, afterInvoke);
            }

            static Func<ParseResult, TInstance, CancellationToken, Task> InvokeAsync(Action<ParseResult, TInstance> action)
            {
                return new Func<ParseResult, TInstance, CancellationToken, Task>((parseResult, instance, _) =>
                {
                    action(parseResult, instance);
                    return Task.CompletedTask;
                });
            }
        }
    }

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public static void SetHandlers<TInstance>(CliCommand command, Func<ParseResult, TInstance> buildInstance, Func<ParseResult, TInstance, CancellationToken, Task> beforeInvoke, Func<ParseResult, TInstance, CancellationToken, Task> afterInvoke)
    {
        SetHandlersCore(command, buildInstance, beforeInvoke, afterInvoke);

        static void SetHandlersCore(CliCommand command, Func<ParseResult, TInstance> buildInstance, Func<ParseResult, TInstance, CancellationToken, Task> beforeInvoke, Func<ParseResult, TInstance, CancellationToken, Task> afterInvoke)
        {
            command.Action = command.Action switch
            {
                AsynchronousCliAction asyncAction => new InstanceNestedAsynchronousCliAction<TInstance>(buildInstance, asyncAction, beforeInvoke, afterInvoke),
                SynchronousCliAction syncAction => new InstanceNestedSynchronousCliAction<TInstance>(buildInstance, syncAction, Invoke(beforeInvoke), Invoke(afterInvoke)),
                null => new InstanceAsynchronousCliAction<TInstance>(buildInstance, beforeInvoke, afterInvoke),
                var a => a,
            };

            foreach (var subCommand in command.Subcommands)
            {
                SetHandlersCore(subCommand, buildInstance, beforeInvoke, afterInvoke);
            }

            static Action<ParseResult, TInstance> Invoke(Func<ParseResult, TInstance, CancellationToken, Task> func)
            {
                return new Action<ParseResult, TInstance>((parseResult, instance) => func(parseResult, instance, CancellationToken.None).GetAwaiter().GetResult());
            }
        }
    }

    /// <summary>
    /// Gets the instance from the parse result.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(ParseResult parseResult) => GetInstanceFromAction<TInstance>(parseResult, parseResult.CommandResult.Command.Action);

    /// <summary>
    /// Gets the instance from the command.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(CliCommand command) => GetInstanceFromAction<TInstance>(default, command.Action);

    /// <summary>
    /// Gets the instance from the action.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(CliAction action) => GetInstanceFromAction<TInstance>(default, action);

    private static TInstance? GetInstanceFromAction<TInstance>(ParseResult? parseResult, CliAction? action) => action switch
    {
        IInstance<TInstance> instance => instance.Get(parseResult),
        INestedAction nested => GetInstanceFromAction<TInstance>(parseResult, nested.Action),
        _ => default,
    };

    private static TInstance ThrowIfNull<TInstance>(TInstance? instance) => instance ?? throw new InvalidOperationException();

    private static class Empty<T>
    {
        public static Func<ParseResult, T, CancellationToken, Task> Task => (_, _, _) => Threading.Tasks.Task.CompletedTask;

        public static Action<ParseResult, T> Action => (_, _) => { };
    }

    private sealed class InstanceSynchronousCliAction<TInstance>(Func<ParseResult, TInstance> create, Action<ParseResult, TInstance> beforeInvoke, Action<ParseResult, TInstance> afterInvoke) : SynchronousCliAction, IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override int Invoke(ParseResult parseResult)
        {
            this.EnsureInstance(parseResult);
            beforeInvoke(parseResult, ThrowIfNull(this.instance));
            afterInvoke(parseResult, ThrowIfNull(this.instance));
            return 0;
        }

        public TInstance Get(ParseResult? parseResult)
        {
            this.EnsureInstance(parseResult);
            return this.instance!;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }

    private sealed class InstanceAsynchronousCliAction<TInstance>(Func<ParseResult, TInstance> create, Func<ParseResult, TInstance, CancellationToken, Task> beforeInvoke, Func<ParseResult, TInstance, CancellationToken, Task> afterInvoke) : AsynchronousCliAction, IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            this.EnsureInstance(parseResult);
            await beforeInvoke(parseResult, ThrowIfNull(this.instance), cancellationToken).ConfigureAwait(false);
            await afterInvoke(parseResult, ThrowIfNull(this.instance), cancellationToken).ConfigureAwait(false);
            return 0;
        }

        public TInstance Get(ParseResult? parseResult)
        {
            this.EnsureInstance(parseResult);
            return this.instance!;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }

    private sealed class InstanceNestedAsynchronousCliAction<TInstance>(Func<ParseResult, TInstance> create, AsynchronousCliAction action, Func<ParseResult, TInstance, CancellationToken, Task> beforeInvoke, Func<ParseResult, TInstance, CancellationToken, Task> afterInvoke)
        : NestedAsynchronousCliAction(
            action,
            (action, parseResult, cancellationToken) => beforeInvoke(parseResult, ThrowIfNull(GetInstanceFromAction<TInstance>(parseResult, action)), cancellationToken),
            (action, parseResult, cancellationToken) => afterInvoke(parseResult, ThrowIfNull(GetInstanceFromAction<TInstance>(parseResult, action)), cancellationToken)), IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            this.EnsureInstance(parseResult);
            return base.InvokeAsync(parseResult, cancellationToken);
        }

        public TInstance Get(ParseResult? parseResult)
        {
            this.EnsureInstance(parseResult);
            return this.instance!;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }

    private sealed class InstanceNestedSynchronousCliAction<TInstance>(Func<ParseResult, TInstance> create, SynchronousCliAction actualAction, Action<ParseResult, TInstance> beforeInvoke, Action<ParseResult, TInstance> afterInvoke)
        : NestedSynchronousCliAction(
            actualAction,
            (action, parseResult) => beforeInvoke(parseResult, ThrowIfNull(GetInstanceFromAction<TInstance>(parseResult, action))),
            (action, parseResult) => afterInvoke(parseResult, ThrowIfNull(GetInstanceFromAction<TInstance>(parseResult, action)))), IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override int Invoke(ParseResult parseResult)
        {
            this.EnsureInstance(parseResult);
            return base.Invoke(parseResult);
        }

        public TInstance Get(ParseResult? parseResult)
        {
            this.EnsureInstance(parseResult);
            return this.instance!;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }
}