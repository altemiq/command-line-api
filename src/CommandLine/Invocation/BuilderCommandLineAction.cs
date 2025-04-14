// -----------------------------------------------------------------------
// <copyright file="BuilderCommandLineAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// The builder action.
/// </summary>
public static class BuilderCommandLineAction
{
    private static readonly Collections.Concurrent.ConcurrentDictionary<(Command, Type, Type), Configurer> Configures = [];

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createInstance">The instance.</param>
    /// <param name="configure">The action to configure the builder.</param>
    public static void SetHandlers<TBuilder, TInstance>(Command command, Func<TBuilder, TInstance> createInstance, Action<ParseResult?, TBuilder> configure)
        where TBuilder : new() => SetHandlers(command, static _ => new TBuilder(), createInstance, configure);

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createBuilder">The function to create the builder.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="configure">The action to configure the builder.</param>
    public static void SetHandlers<TBuilder, TInstance>(Command command, Func<ParseResult?, TBuilder> createBuilder, Func<TBuilder, TInstance> buildInstance, Action<ParseResult?, TBuilder> configure) => SetHandlers(command, createBuilder, buildInstance, configure, create => InstanceCommandLineAction.SetHandlers(command, create));

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createBuilder">The function to create the builder.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public static void SetHandlers<TBuilder, TInstance>(Command command, Func<ParseResult?, TBuilder> createBuilder, Func<TBuilder, TInstance> buildInstance, Action<ParseResult?, TBuilder> configure, Action<ParseResult, TInstance> beforeInvoke, Action<ParseResult, TInstance> afterInvoke) => SetHandlers(command, createBuilder, buildInstance, configure, create => InstanceCommandLineAction.SetHandlers(command, create, beforeInvoke, afterInvoke));

    /// <summary>
    /// Sets the handlers.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder.</typeparam>
    /// <typeparam name="TInstance">The type of instance.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="createBuilder">The function to create the builder.</param>
    /// <param name="buildInstance">The build the instance.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public static void SetHandlers<TBuilder, TInstance>(Command command, Func<ParseResult?, TBuilder> createBuilder, Func<TBuilder, TInstance> buildInstance, Action<ParseResult?, TBuilder> configure, Func<ParseResult, TInstance, CancellationToken, Task> beforeInvoke, Func<ParseResult, TInstance, CancellationToken, Task> afterInvoke) => SetHandlers(command, createBuilder, buildInstance, configure, create => InstanceCommandLineAction.SetHandlers(command, create, beforeInvoke, afterInvoke));

    private static void SetHandlers<TBuilder, TInstance>(Command command, Func<ParseResult?, TBuilder> createBuilder, Func<TBuilder, TInstance> buildInstance, Action<ParseResult?, TBuilder> configure, Action<Func<ParseResult?, TInstance>> setHandler)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
        _ = Configures.AddOrUpdate(
            (command, typeof(TBuilder), typeof(TInstance)),
            (_, state) =>
            {
                var configurer = new Configurer();
                configurer.Add(state.Configure);
                state.SetHandler(Create);
                return configurer;
            },
            (_, configurer, state) =>
            {
                configurer.Add(state.Configure);
                return configurer;
            },
            (Configure: configure, SetHandler: setHandler));
#else
        _ = Configures.AddOrUpdate(
            (command, typeof(TBuilder), typeof(TInstance)),
            _ =>
            {
                var configurer = new Configurer();
                configurer.Add(configure);
                setHandler(Create);
                return configurer;
            },
            (_, configurer) =>
            {
                configurer.Add(configure);
                return configurer;
            });
#endif

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

        public void Add<T>(Action<ParseResult?, T> action) => this.actions.Add((parseResult, obj) =>
        {
            if (obj is T t)
            {
                action(parseResult, t);
            }
        });

        public void Configure(ParseResult? parseResult, object? obj)
        {
            foreach (var action in this.actions)
            {
                action(parseResult, obj);
            }
        }
    }
}