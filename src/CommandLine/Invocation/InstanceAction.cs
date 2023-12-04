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
        T? Get();
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
        command.Action = command.Action switch
        {
            AsynchronousCliAction asyncAction => new InstanceNestedAsynchronousAction<TInstance>(buildInstance, asyncAction),
            SynchronousCliAction syncAction => new InstanceNestedSynchronousAction<TInstance>(buildInstance, syncAction),
            null => new InstanceSynchronousAction<TInstance>(buildInstance),
            var a => a,
        };

        foreach (var subCommand in command.Subcommands)
        {
            SetHandlers(subCommand, buildInstance);
        }
    }

    /// <summary>
    /// Gets the instance from the parse result.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(ParseResult parseResult) => GetInstanceFromAction<TInstance>(parseResult.CommandResult.Command.Action);

    /// <summary>
    /// Gets the instance from the command.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(CliCommand command) => GetInstanceFromAction<TInstance>(command.Action);

    /// <summary>
    /// Gets the instance from the action.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>The instance.</returns>
    public static TInstance? GetInstance<TInstance>(CliAction action) => GetInstanceFromAction<TInstance>(action);

    private static TInstance? GetInstanceFromAction<TInstance>(CliAction? action) => action switch
    {
        IInstance<TInstance> instance => instance.Get(),
        INestedAction nested => GetInstanceFromAction<TInstance>(nested.Action),
        _ => default,
    };

    private sealed class InstanceSynchronousAction<TInstance>(Func<ParseResult, TInstance> create) : SynchronousCliAction, IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override int Invoke(ParseResult parseResult)
        {
            this.EnsureInstance(parseResult);
            return 0;
        }

        public TInstance? Get()
        {
            this.EnsureInstance(default);
            return this.instance;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }

    private sealed class InstanceNestedAsynchronousAction<TInstance>(Func<ParseResult, TInstance> create, AsynchronousCliAction action) : NestedAsynchronousCliAction(action), IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            this.EnsureInstance(parseResult);
            return base.InvokeAsync(parseResult, cancellationToken);
        }

        public TInstance? Get()
        {
            this.EnsureInstance(default);
            return this.instance;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }

    private sealed class InstanceNestedSynchronousAction<TInstance>(Func<ParseResult, TInstance> create, SynchronousCliAction actualAction) : NestedSynchronousCliAction(actualAction), IInstance<TInstance>
    {
        private readonly Func<ParseResult, TInstance> create = create;
        private TInstance? instance;

        public override int Invoke(ParseResult parseResult)
        {
            this.EnsureInstance(parseResult);
            return base.Invoke(parseResult);
        }

        public TInstance? Get()
        {
            this.EnsureInstance(default);
            return this.instance;
        }

        private void EnsureInstance(ParseResult? parseResult) => this.instance ??= this.create(parseResult!);
    }
}