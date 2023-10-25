// -----------------------------------------------------------------------
// <copyright file="BuilderAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// The builder action.
/// </summary>
public static class BuilderAction
{
    private static readonly IDictionary<(CliCommand, Type, Type), Configurer> Configures = new Dictionary<(CliCommand, Type, Type), Configurer>();

#pragma warning disable SA1600 // Elements should be documented
    private interface IInstance<out TInstance>
    {
        TInstance? Get();
    }
#pragma warning restore SA1600 // Elements should be documented

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createInstance">The instance.</param>
    /// <param name="configure">The action to confure the builder.</param>
    public static void SetHandlers<TBuilder, TInstance>(CliCommand command, Func<TBuilder, TInstance> createInstance, Action<ParseResult?, TBuilder> configure)
        where TBuilder : new() => SetHandlers(command, _ => new TBuilder(), createInstance, configure);

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createBuilder">The function to create the builder.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="configure">The action to confure the builder.</param>
    public static void SetHandlers<TBuilder, TInstance>(CliCommand command, Func<ParseResult?, TBuilder> createBuilder, Func<TBuilder, TInstance> buildInstance, Action<ParseResult?, TBuilder> configure)
    {
        // see if the handler already exists
        if (Configures.TryGetValue((command, typeof(TBuilder), typeof(TInstance)), out var configurer))
        {
            configurer.Add(configure);
            return;
        }

        configurer = new Configurer();
        configurer.Add(configure);
        Configures.Add((command, typeof(TBuilder), typeof(TInstance)), configurer);
        SetHandlersImpl(command, Create);

        static void SetHandlersImpl(CliCommand command, Func<ParseResult, TInstance> create)
        {
            if (command.Action is AsynchronousCliAction asyncAction)
            {
                command.Action = new BuilderNestedAsynchronousAction<TInstance>(create, asyncAction);
            }
            else if (command.Action is SynchronousCliAction syncAction)
            {
                command.Action = new BuilderNestedSynchronousAction<TInstance>(create, syncAction);
            }

            command.Action ??= new BuilderSynchronousAction<TInstance>(create);

            foreach (var subCommand in command.Subcommands)
            {
                SetHandlersImpl(subCommand, create);
            }
        }

        TInstance Create(ParseResult? parseResult)
        {
            var builder = createBuilder(parseResult);
            if (Configures.TryGetValue((command, typeof(TBuilder), typeof(TInstance)), out var configurer))
            {
                configurer.Configure(parseResult, builder);
            }

            return buildInstance(builder);
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

    private sealed class Configurer
    {
        private readonly IList<Action<ParseResult?, object?>> actions = new List<Action<ParseResult?, object?>>();

        public void Add<T>(Action<ParseResult?, T> action) => this.Add((parseResult, obj) =>
        {
            if (obj is T t)
            {
                action(parseResult, t);
            }
        });

        public void Add(Action<ParseResult?, object?> action) => this.actions.Add(action);

        public void Configure(ParseResult? parseResult, object? obj)
        {
            foreach (var action in this.actions)
            {
                action(parseResult, obj);
            }
        }
    }

    private sealed class BuilderSynchronousAction<TInstance>(Func<ParseResult, TInstance> create) : SynchronousCliAction, IInstance<TInstance>
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

    private sealed class BuilderNestedAsynchronousAction<TInstance>(Func<ParseResult, TInstance> create, AsynchronousCliAction action) : NestedAsynchronousCliAction(action), IInstance<TInstance>
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

    private sealed class BuilderNestedSynchronousAction<TInstance>(Func<ParseResult, TInstance> create, SynchronousCliAction actualAction) : NestedSynchronousCliAction(actualAction), IInstance<TInstance>
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