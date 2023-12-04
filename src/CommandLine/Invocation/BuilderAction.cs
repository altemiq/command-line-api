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
    private static readonly Dictionary<(CliCommand, Type, Type), Configurer> Configures = [];

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
        InstanceAction.SetHandlers(command, Create);

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

    private sealed class Configurer
    {
        private readonly List<Action<ParseResult?, object?>> actions = [];

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
}