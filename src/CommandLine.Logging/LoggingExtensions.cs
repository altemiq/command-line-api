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
    /// Adds logging to the <see cref="CommandLineConfiguration"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <returns>The input configuration.</returns>
    public static T AddLogging<T>(this T configuration, Action<ILoggingBuilder> configure)
        where T : CommandLineConfiguration => configuration.AddLogging((_, builder) => configure(builder));

    /// <summary>
    /// Adds logging to the <see cref="CommandLineConfiguration"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The action to configure the builder.</param>
    /// <returns>The input configuration.</returns>
    public static T AddLogging<T>(this T configuration, Action<ParseResult?, ILoggingBuilder> configure)
        where T : CommandLineConfiguration
    {
        LoggerAction.SetHandlers(configuration.RootCommand, configure);
        return configuration;
    }

    /// <summary>
    /// Gets the logger factory.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The logger factory.</returns>
    public static ILoggerFactory GetLoggerFactory(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<ILoggerFactory>(parseResult) ?? throw new InvalidOperationException(Logging.Properties.Resources.FailedToCreateLoggerFactory);

    /// <inheritdoc cref="ILoggerFactory.CreateLogger(string)" />
    public static ILogger CreateLogger(this ParseResult parseResult, string categoryName) => parseResult.GetLoggerFactory().CreateLogger(categoryName);

    /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, Type)" />
    public static ILogger CreateLogger(this ParseResult parseResult, Type type) => parseResult.GetLoggerFactory().CreateLogger(type);

    /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)" />
    public static ILogger<T> CreateLogger<T>(this ParseResult parseResult) => parseResult.GetLoggerFactory().CreateLogger<T>();

    /// <summary>
    /// Gets the log level.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The log level.</returns>
    public static LogLevel GetLogLevel(this ParseResult parseResult) => parseResult.GetValue<VerbosityOptions>(VerbosityOption.OptionName).GetLogLevel();

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
        private static readonly Collections.Concurrent.ConcurrentDictionary<Command, Configurer> Configures = [];

        public static void SetHandlers(Command command, Action<ParseResult?, ILoggingBuilder> configure)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
            _ = Configures.AddOrUpdate(
                command,
                static (input, configure) =>
                {
                    var configurer = new Configurer();
                    configurer.Add(configure);
                    Invocation.InstanceCommandLineAction.SetHandlers(input, Create);
                    return configurer;

                    ILoggerFactory Create(ParseResult parseResult)
                    {
                        return CreateWithCommand(input, parseResult);
                    }
                },
                static (_, configurer, configure) =>
                {
                    configurer.Add(configure);
                    return configurer;
                },
                configure);
#else
            _ = Configures.AddOrUpdate(
                command,
                input =>
                {
                    var configurer = new Configurer();
                    configurer.Add(configure);
                    Invocation.InstanceCommandLineAction.SetHandlers(input, Create);
                    return configurer;

                    ILoggerFactory Create(ParseResult parseResult)
                    {
                        return CreateWithCommand(input, parseResult);
                    }
                },
                (_, configurer) =>
                {
                    configurer.Add(configure);
                    return configurer;
                });
#endif
            static ILoggerFactory CreateWithCommand(Command command, ParseResult? parseResult)
            {
                var serviceCollection = new ServiceCollection();
                _ = serviceCollection.AddLogging(builder =>
                {
                    if (GetOptionResult(parseResult) is { } optionResult)
                    {
                        var value = optionResult.GetValueOrDefault<VerbosityOptions>();
                        _ = builder.SetMinimumLevel(value.GetLogLevel());
                    }

                    if (Configures.TryGetValue(command, out var commandConfigure))
                    {
                        commandConfigure.Configure(parseResult, builder);
                    }

                    static Parsing.OptionResult? GetOptionResult(ParseResult? parseResult)
                    {
                        if (parseResult is null)
                        {
                            return null;
                        }

                        try
                        {
                            return parseResult.RootCommandResult.GetResult(VerbosityOption.OptionName) as Parsing.OptionResult;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                });

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return new DisposingLoggerFactory(loggerFactory, serviceProvider);
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

        private sealed class DisposingLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider) : ILoggerFactory
        {
            void IDisposable.Dispose() => serviceProvider.Dispose();

            ILogger ILoggerFactory.CreateLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);

            void ILoggerFactory.AddProvider(ILoggerProvider provider) => loggerFactory.AddProvider(provider);
        }
    }
}