﻿// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// See <see cref="Microsoft.Extensions.Hosting"/> extensions.
/// </summary>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static partial class HostingExtensions
{
    private const string HostingDirectiveName = "config";

    private static readonly char[] Separator = ['='];

    /// <summary>
    /// Uses the host builder for this instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T UseHost<T>(
        this T configuration,
        Action<ParseResult?, Microsoft.Extensions.Hosting.IHostBuilder>? configureHost = default)
        where T : CommandLineConfiguration => UseHost(
            configuration,
#if NET7_0_OR_GREATER
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder,
#else
            static _ => new Microsoft.Extensions.Hosting.HostBuilder(),
#endif
            configureHost);

    /// <summary>
    /// Uses the host builder for this instance.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T UseHost<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.Hosting.IHostBuilder>? configureHost = default)
        where T : CommandLineConfiguration
    {
        if (configuration.RootCommand is RootCommand root)
        {
            root.Add(new Directive(HostingDirectiveName));
        }

        Invocation.BuilderCommandLineAction.SetHandlers(
            configuration.RootCommand,
            parseResult => CreateHostBuilder(configuration, hostBuilderFactory, parseResult),
            static builder => builder.Build(),
            (parseResult, builder) => configureHost?.Invoke(parseResult, builder),
            static (_, host, cancellationToken) => host.StartAsync(cancellationToken),
            static (_, host, cancellationToken) => host.StopAsync(cancellationToken));

        return configuration;

        static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder(T configuration, Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory, ParseResult? parseResult)
        {
            var hostBuilder = hostBuilderFactory(GetUnmatchedTokens(parseResult));

            if (parseResult is not null)
            {
                hostBuilder.Properties[typeof(ParseResult)] = parseResult;
                UpdateHostConfiguration(configuration, parseResult, hostBuilder);
                _ = hostBuilder.ConfigureServices((_, services) => services.AddSingleton(parseResult));
            }

            _ = hostBuilder.UseInvocationLifetime();
            return hostBuilder;

            static string[] GetUnmatchedTokens(ParseResult? parseResult)
            {
                return parseResult?.UnmatchedTokens.ToArray() ?? [];
            }

            static void UpdateHostConfiguration(T configuration, ParseResult parseResult, Microsoft.Extensions.Hosting.IHostBuilder hostBuilder)
            {
                if (configuration.RootCommand is RootCommand root
                    && root.Directives.SingleOrDefault(static d => string.Equals(d.Name, HostingDirectiveName, StringComparison.Ordinal)) is { } directive
                    && parseResult.GetResult(directive) is { } directiveResult)
                {
                    _ = hostBuilder.ConfigureHostConfiguration(config => config.AddInMemoryCollection([.. directiveResult.Values.Select(Parse)]));

                    static KeyValuePair<string, string?> Parse(string s)
                    {
                        var parts = s.Split(Separator, count: 2);
                        var key = parts[0];
                        var value = parts.Length > 1 ? parts[1] : null;
                        return new KeyValuePair<string, string?>(key, value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Use the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.IHostBuilder UseInvocationLifetime(this Microsoft.Extensions.Hosting.IHostBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null) => host
        .ConfigureServices((__, services) => _ = services.AddInvocationLifetime()).ConfigureInvocationLifetime(configureOptions);

    /// <summary>
    /// Use the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.IHostBuilder ConfigureInvocationLifetime(this Microsoft.Extensions.Hosting.IHostBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null) =>
        host.ConfigureServices((_, services) => services.ConfigureInvocationLifetime(configureOptions));

    /// <summary>
    /// Gets the host from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The host.</returns>
    public static Microsoft.Extensions.Hosting.IHost? GetHost(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult);

    /// <summary>
    /// Gets the host from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The host.</returns>
    public static Microsoft.Extensions.Hosting.IHost? GetHost(this Command command) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command);

    /// <summary>
    /// Gets the host from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The host.</returns>
    public static Microsoft.Extensions.Hosting.IHost? GetHost(this Invocation.CommandLineAction action) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action);
}