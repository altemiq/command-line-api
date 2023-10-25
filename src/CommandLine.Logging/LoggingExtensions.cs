// -----------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="Microsoft.Extensions.Logging"/> extensions.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging to the <see cref="CliConfiguration"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <returns>The input configuration.</returns>
    public static T AddLogging<T>(this T configuration, Action<ILoggingBuilder> configure)
        where T : CliConfiguration => configuration.AddLogging((_, builder) => configure(builder));

    /// <summary>
    /// Adds logging to the <see cref="CliConfiguration"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <returns>The input configuration.</returns>
    public static T AddLogging<T>(this T configuration, Action<ParseResult?, ILoggingBuilder> configure)
        where T : CliConfiguration
    {
        LoggerAction.SetHandlers(configuration.RootCommand, configure);
        return configuration;
    }

    /// <inheritdoc cref="ILoggerFactory.CreateLogger(string)" />
    public static ILogger CreateLogger(this ParseResult parseResult, string categoryName) => LoggerAction.GetLoggerFactory(parseResult)!.CreateLogger(categoryName);

    /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, Type)" />
    public static ILogger CreateLogger(this ParseResult parseResult, Type type) => LoggerAction.GetLoggerFactory(parseResult)!.CreateLogger(type);

    /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)" />
    public static ILogger<T> CreateLogger<T>(this ParseResult parseResult) => LoggerAction.GetLoggerFactory(parseResult)!.CreateLogger<T>();

    /// <summary>
    /// Gets the log level.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The log level.</returns>
    public static LogLevel GetLogLevel(this ParseResult parseResult) => parseResult.GetValue(CliOptions.VerbosityOption).GetLogLevel();

    private static LogLevel GetLogLevel(this VerbosityOptions options) => options switch
    {
        VerbosityOptions.q or VerbosityOptions.quiet => LogLevel.Error,
        VerbosityOptions.m or VerbosityOptions.minimal => LogLevel.Warning,
        VerbosityOptions.n or VerbosityOptions.normal => LogLevel.Information,
        VerbosityOptions.d or VerbosityOptions.detailed => LogLevel.Debug,
        VerbosityOptions.diag or VerbosityOptions.diagnostic => LogLevel.Trace,
        _ => throw new ArgumentOutOfRangeException(nameof(options)),
    };

    private static class LoggerAction
    {
        private static readonly IDictionary<CliCommand, Configurer> Configures = new Dictionary<CliCommand, Configurer>();

        public static void SetHandlers(CliCommand command, Action<ParseResult?, ILoggingBuilder> configure)
        {
            // see if the handler already exists
            if (Configures.TryGetValue(command, out var configurer))
            {
                configurer.Add(configure);
                return;
            }

            configurer = new Configurer();
            configurer.Add(configure);
            Configures.Add(command, configurer);
            Invocation.InstanceAction.SetHandlers(command, Create);

            ILoggerFactory Create(ParseResult? parseResult)
            {
                var serviceCollection = new ServiceCollection();
                _ = serviceCollection.AddLogging(builder =>
                {
                    if (parseResult is not null)
                    {
                        _ = builder.SetMinimumLevel(parseResult.GetLogLevel());
                    }

                    if (Configures.TryGetValue(command, out var configurer))
                    {
                        configurer.Configure(parseResult, builder);
                    }
                });

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return new DisposingLoggerFactory(loggerFactory, serviceProvider);
            }
        }

        public static ILoggerFactory? GetLoggerFactory(ParseResult parseResult) => Invocation.InstanceAction.GetInstance<ILoggerFactory>(parseResult.CommandResult.Command.Action!);

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

        private sealed class DisposingLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider) : ILoggerFactory
        {
            void IDisposable.Dispose() => serviceProvider.Dispose();

            ILogger ILoggerFactory.CreateLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);

            void ILoggerFactory.AddProvider(ILoggerProvider provider) => loggerFactory.AddProvider(provider);
        }
    }
}